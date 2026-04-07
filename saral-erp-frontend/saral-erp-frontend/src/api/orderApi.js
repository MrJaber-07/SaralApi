import axios from 'axios';

const API = axios.create({
    baseURL: 'https://localhost:7163/api' // Match your backend port
});

export const productApi = {
    getAll: () => API.get('/Product'),
    getById: (id) => API.get(`/Product/${id}`),
    create: (data) => API.post('/Product', data),
    update: (id, data) => API.put(`/Product/${id}`, data),
    delete: (id) => API.delete(`/Product/${id}`)
};

export const orderApi = {
    getAll: () => API.get('/Orders'),
    placeOrder: (data, key) => API.post('/Orders', data, { headers: { 'X-Idempotency-Key': key } }),
    delete: (id) => API.delete(`/Orders/${id}`)
};