import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom/vitest';
import TotaisGerais from '../TotaisGerais';
import { transacoesApi } from '../../services/api';

// Mock da API
vi.mock('../../services/api', () => ({
  transacoesApi: {
    getTotaisGerais: vi.fn(),
  },
}));

describe('TotaisGerais', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('deve exibir loading inicialmente', () => {
    vi.mocked(transacoesApi.getTotaisGerais).mockImplementation(
      () => new Promise(() => {}) // Promise que nunca resolve
    );

    render(<TotaisGerais />);
    expect(screen.getByText('Carregando totais...')).toBeInTheDocument();
  });

  it('deve exibir totais quando carregados com sucesso', async () => {
    const mockTotais = {
      totalReceitas: 1000,
      totalDespesas: 500,
      saldoLiquido: 500,
    };

    vi.mocked(transacoesApi.getTotaisGerais).mockResolvedValue({
      success: true,
      data: mockTotais,
      errors: [],
    });

    render(<TotaisGerais />);

    await waitFor(() => {
      expect(screen.getByText('Totais Gerais')).toBeInTheDocument();
    });

    expect(screen.getByText('Total de Receitas')).toBeInTheDocument();
    expect(screen.getByText('Total de Despesas')).toBeInTheDocument();
    expect(screen.getByText('Saldo Líquido')).toBeInTheDocument();
  });

  it('deve chamar API com filtros quando fornecidos', async () => {
    const mockTotais = {
      totalReceitas: 1000,
      totalDespesas: 500,
      saldoLiquido: 500,
    };

    vi.mocked(transacoesApi.getTotaisGerais).mockResolvedValue({
      success: true,
      data: mockTotais,
      errors: [],
    });

    render(
      <TotaisGerais
        dataInicio="2024-01-01"
        dataFim="2024-12-31"
        pessoaId={1}
        categoriaId={2}
        tipo={1}
      />
    );

    await waitFor(() => {
      expect(transacoesApi.getTotaisGerais).toHaveBeenCalledWith(
        '2024-01-01',
        '2024-12-31',
        1,
        2,
        1
      );
    });
  });

  it('não deve renderizar nada quando não há totais', async () => {
    vi.mocked(transacoesApi.getTotaisGerais).mockResolvedValue({
      success: false,
      data: null,
      errors: ['Erro ao carregar'],
    });

    const { container } = render(<TotaisGerais />);

    await waitFor(() => {
      expect(screen.queryByText('Totais Gerais')).not.toBeInTheDocument();
    });

    expect(container.firstChild).toBeNull();
  });

  it('deve exibir valores formatados corretamente', async () => {
    const mockTotais = {
      totalReceitas: 1234.56,
      totalDespesas: 789.12,
      saldoLiquido: 445.44,
    };

    vi.mocked(transacoesApi.getTotaisGerais).mockResolvedValue({
      success: true,
      data: mockTotais,
      errors: [],
    });

    render(<TotaisGerais />);

    await waitFor(() => {
      expect(screen.getByText('R$ 1.234,56')).toBeInTheDocument();
      expect(screen.getByText('R$ 789,12')).toBeInTheDocument();
      expect(screen.getByText('R$ 445,44')).toBeInTheDocument();
    });
  });
});

