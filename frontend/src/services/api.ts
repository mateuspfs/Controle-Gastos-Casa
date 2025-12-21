import axios from 'axios';
import type { ApiResult, PessoaDto, PagedResultDto, CategoriaDto, TransacaoDto } from '../types/api';

// Configuração base do cliente HTTP para comunicação com a API
const api = axios.create({
  baseURL: import.meta.env.DEV ? 'http://localhost:5027' : '', // URL do backend em desenvolvimento
  headers: {
    'Content-Type': 'application/json',
  },
});

// Serviço de API para pessoas
export const pessoasApi = {
  // Lista todas as pessoas com paginação e busca
  // Quando take = 0, retorna ApiResult<IReadOnlyList<PessoaDto>>
  // Caso contrário, retorna ApiResult<PagedResultDto<PessoaDto>>
  getAll: async (skip = 0, take = 20, searchTerm?: string): Promise<ApiResult<PagedResultDto<PessoaDto>> | ApiResult<PessoaDto[]>> => {
    const params: { skip: number; take: number; searchTerm?: string } = { skip, take };
    if (searchTerm) {
      params.searchTerm = searchTerm;
    }
    const response = await api.get<ApiResult<PagedResultDto<PessoaDto>> | ApiResult<PessoaDto[]>>('/pessoas', { params });
    return response.data;
  },

  // Busca pessoa por ID
  getById: async (id: number): Promise<ApiResult<PessoaDto>> => {
    const response = await api.get<ApiResult<PessoaDto>>(`/pessoas/${id}`);
    return response.data;
  },

  // Cria nova pessoa (enviando apenas nome e dataNascimento, idade é calculada no backend)
  create: async (pessoa: Omit<PessoaDto, 'id' | 'idade'>): Promise<ApiResult<PessoaDto>> => {
    try {
      const response = await api.post<ApiResult<PessoaDto>>('/pessoas', pessoa);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Atualiza pessoa existente
  update: async (id: number, pessoa: Omit<PessoaDto, 'id' | 'idade'>): Promise<ApiResult<PessoaDto>> => {
    try {
      const response = await api.put<ApiResult<PessoaDto>>(`/pessoas/${id}`, pessoa);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Deleta pessoa por ID
  delete: async (id: number): Promise<ApiResult<boolean>> => {
    const response = await api.delete<ApiResult<boolean>>(`/pessoas/${id}`);
    return response.data;
  },
};

// Serviço de API para categorias
export const categoriasApi = {
  // Lista todas as categorias com paginação, busca e filtro por finalidade
  // Quando take = 0, retorna ApiResult<IReadOnlyList<CategoriaDto>>
  // Caso contrário, retorna ApiResult<PagedResultDto<CategoriaDto>>
  getAll: async (skip = 0, take = 20, searchTerm?: string, finalidade?: number): Promise<ApiResult<PagedResultDto<CategoriaDto>> | ApiResult<CategoriaDto[]>> => {
    const params: { skip: number; take: number; searchTerm?: string; finalidade?: number } = { skip, take };
    if (searchTerm) {
      params.searchTerm = searchTerm;
    }
    if (finalidade !== undefined && finalidade !== null) {
      params.finalidade = finalidade;
    }
    const response = await api.get<ApiResult<PagedResultDto<CategoriaDto>> | ApiResult<CategoriaDto[]>>('/categorias', { params });
    return response.data;
  },

  // Busca categoria por ID
  getById: async (id: number): Promise<ApiResult<CategoriaDto>> => {
    const response = await api.get<ApiResult<CategoriaDto>>(`/categorias/${id}`);
    return response.data;
  },

  // Cria nova categoria
  create: async (categoria: Omit<CategoriaDto, 'id'>): Promise<ApiResult<CategoriaDto>> => {
    try {
      const response = await api.post<ApiResult<CategoriaDto>>('/categorias', categoria);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Atualiza categoria existente
  update: async (id: number, categoria: Omit<CategoriaDto, 'id'>): Promise<ApiResult<CategoriaDto>> => {
    try {
      const response = await api.put<ApiResult<CategoriaDto>>(`/categorias/${id}`, categoria);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },
};

// Serviço de API para transações
export const transacoesApi = {
  // Lista todas as transações com paginação e filtros
  getAll: async (
    skip = 0, 
    take = 20, 
    dataInicio?: string, 
    dataFim?: string,
    pessoaId?: number,
    categoriaId?: number,
    tipo?: number
  ): Promise<ApiResult<PagedResultDto<TransacaoDto>>> => {
    const params: { 
      skip: number; 
      take: number; 
      dataInicio?: string; 
      dataFim?: string;
      pessoaId?: number;
      categoriaId?: number;
      tipo?: number;
    } = { skip, take };
    if (dataInicio) {
      params.dataInicio = dataInicio;
    }
    if (dataFim) {
      params.dataFim = dataFim;
    }
    if (pessoaId !== undefined && pessoaId !== null && pessoaId > 0) {
      params.pessoaId = pessoaId;
    }
    if (categoriaId !== undefined && categoriaId !== null && categoriaId > 0) {
      params.categoriaId = categoriaId;
    }
    if (tipo !== undefined && tipo !== null) {
      params.tipo = tipo;
    }
    const response = await api.get<ApiResult<PagedResultDto<TransacaoDto>>>('/transacoes', { params });
    return response.data;
  },

  // Busca transação por ID
  getById: async (id: number): Promise<ApiResult<TransacaoDto>> => {
    const response = await api.get<ApiResult<TransacaoDto>>(`/transacoes/${id}`);
    return response.data;
  },

  // Cria nova transação
  create: async (transacao: Omit<TransacaoDto, 'id'>): Promise<ApiResult<TransacaoDto>> => {
    try {
      const response = await api.post<ApiResult<TransacaoDto>>('/transacoes', transacao);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Atualiza transação existente
  update: async (id: number, transacao: Omit<TransacaoDto, 'id'>): Promise<ApiResult<TransacaoDto>> => {
    try {
      const response = await api.put<ApiResult<TransacaoDto>>(`/transacoes/${id}`, transacao);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  // Deleta transação por ID
  delete: async (id: number): Promise<ApiResult<boolean>> => {
    const response = await api.delete<ApiResult<boolean>>(`/transacoes/${id}`);
    return response.data;
  },
};

export default api;
