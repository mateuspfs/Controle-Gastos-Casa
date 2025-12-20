// Tipos TypeScript correspondentes aos DTOs e helpers da API

// Formato padrão de erro de validação do ASP.NET Core
export interface AspNetValidationError {
  type: string;
  title: string;
  status: number;
  errors: Record<string, string[]>;
  traceId?: string;
}

export interface ApiResult<T> {
  success: boolean;
  errors: string[];
  data: T | null;
}

// DTO de resposta paginada com todas as informações calculadas no backend
export interface PagedResultDto<T> {
  items: T[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PessoaDto {
  id?: number;
  nome: string;
  dataNascimento: string;
  idade?: string;
}

