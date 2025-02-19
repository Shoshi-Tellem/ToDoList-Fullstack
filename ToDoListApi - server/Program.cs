using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList_Fullstack;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
Console.WriteLine($"Connection string: {connectionString}");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty;
    });
// }

app.MapGet("/", () => "toDoListServer api is running ");

app.MapGet("Items", async (ToDoDbContext context) =>
{
    return Results.Ok(await context.Items.ToListAsync());
});

app.MapGet("Items/{id}", async (int id, ToDoDbContext context) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem == null)
        return Results.NotFound();
    return Results.Ok(existItem);
});

app.MapPost("Items", async (Item item, ToDoDbContext context) =>
{
    var existItem = await context.Items.FindAsync(item.Id);
    if (existItem != null)
        return Results.Conflict("An item with the same Id already exists.");
    await context.Items.AddAsync(item);
    await context.SaveChangesAsync();
    return Results.Created($"/Items/{item.Id}", item);
});

app.MapPut("Items/{id}", async (int id, ToDoDbContext context) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem == null)
        return Results.NotFound();
    existItem.IsComplete = !existItem.IsComplete;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("Items/{id}", async (int id, ToDoDbContext context) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem == null)
        return Results.NotFound();
    context.Items.Remove(existItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();