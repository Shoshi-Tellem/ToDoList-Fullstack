import axios from 'axios';

const apiUrl = "http://localhost:5215"

export default {
  getTasks: async () => {
    console.log('getTasks')
    const result = await axios.get(`${apiUrl}/items`)
    return result.data
  },

  addTask: async (name) => {
    console.log('addTask', { name })
    const result = await axios.post(`${apiUrl}/items`, { name })
    return result.data
  },

  setCompleted: async (id) => {
    console.log('setCompleted', { id, isComplete })
    const result = await axios.put(`${apiUrl}/items/${id}`)
    return result.data
  },

  deleteTask: async (id) => {
    console.log('deleteTask', id)
    const result = await axios.delete(`${apiUrl}/items/${id}`)
    return result.data
  }
};