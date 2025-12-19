import axios from 'axios';

// Configuração base do cliente HTTP para comunicação com a API
const api = axios.create({
  baseURL: '',
});

export default api;
