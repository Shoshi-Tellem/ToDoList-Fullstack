using Microsoft.EntityFrameworkCore;
using ToDoList_Fullstack;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        throw;
    }
});

app.MapGet("items", async (ToDoDbContext context) =>
{
    return Results.Ok(await context.Items.ToListAsync());
});

app.MapGet("items/{id}", async (int id, ToDoDbContext context) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem == null)
        return Results.NotFound();
    return Results.Ok(existItem);
});

app.MapPost("items", async (Item item, ToDoDbContext context) => {
    await context.Items.AddAsync(item);
    await context.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("items", async (Item item, ToDoDbContext context) => {
    var existItem = await context.Items.FindAsync(item.Id);
    if (existItem == null)
        return Results.NotFound();
    context.Items.Update(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("items", async (int id, ToDoDbContext context) => {
    var existItem = await context.Items.FindAsync(id);
    if (existItem == null)
        return Results.NotFound();
    context.Items.Remove(existItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();