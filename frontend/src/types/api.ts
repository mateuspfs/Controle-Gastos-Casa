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

export interface PessoaTotaisDto {
  id: number;
  nome: string;
  dataNascimento: string;
  idade: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export const FinalidadeCategoria = {
  Despesa: 1,
  Receita: 2,
  Ambas: 3,
} as const;

export type FinalidadeCategoria = typeof FinalidadeCategoria[keyof typeof FinalidadeCategoria];

export interface CategoriaDto {
  id?: number;
  descricao: string;
  finalidade: FinalidadeCategoria;
}

export const TipoTransacao = {
  Despesa: 1,
  Receita: 2,
} as const;

export type TipoTransacao = typeof TipoTransacao[keyof typeof TipoTransacao];

export interface TransacaoDto {
  id?: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  dataTransacao: string;
  categoriaId: number;
  pessoaId: number;
  pessoa?: PessoaDto;
  categoria?: CategoriaDto;
}

export interface TotaisGeraisDto {
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
}

