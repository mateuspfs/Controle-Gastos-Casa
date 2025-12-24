import React from 'react';
import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Pagination from '../Pagination';
import { ThemeProvider } from '../../hooks/useTheme';

// Helper para renderizar componentes com ThemeProvider
const renderWithTheme = (component: React.ReactElement) => {
  return render(<ThemeProvider>{component}</ThemeProvider>);
};

describe('Pagination', () => {
  it('deve renderizar botões de navegação', () => {
    renderWithTheme(
      <Pagination
        currentPage={1}
        pageSize={10}
        totalItems={50}
        onPrevious={vi.fn()}
        onNext={vi.fn()}
      />
    );
    expect(screen.getByText('Anterior')).toBeInTheDocument();
    expect(screen.getByText('Próxima')).toBeInTheDocument();
  });

  it('deve exibir página atual e total de páginas', () => {
    renderWithTheme(
      <Pagination
        currentPage={2}
        pageSize={10}
        totalItems={50}
        onPrevious={vi.fn()}
        onNext={vi.fn()}
      />
    );
    expect(screen.getByText('Página 2 de 5')).toBeInTheDocument();
  });

  it('deve exibir apenas página atual quando totalPages é 0', () => {
    renderWithTheme(
      <Pagination
        currentPage={1}
        pageSize={10}
        totalItems={0}
        onPrevious={vi.fn()}
        onNext={vi.fn()}
      />
    );
    expect(screen.getByText('Página 1')).toBeInTheDocument();
  });

  it('deve chamar onPrevious quando botão Anterior é clicado', async () => {
    const user = userEvent.setup();
    const handlePrevious = vi.fn();
    renderWithTheme(
      <Pagination
        currentPage={2}
        pageSize={10}
        totalItems={50}
        onPrevious={handlePrevious}
        onNext={vi.fn()}
      />
    );
    
    await user.click(screen.getByText('Anterior'));
    expect(handlePrevious).toHaveBeenCalledTimes(1);
  });

  it('deve chamar onNext quando botão Próxima é clicado', async () => {
    const user = userEvent.setup();
    const handleNext = vi.fn();
    renderWithTheme(
      <Pagination
        currentPage={1}
        pageSize={10}
        totalItems={50}
        onPrevious={vi.fn()}
        onNext={handleNext}
      />
    );
    
    await user.click(screen.getByText('Próxima'));
    expect(handleNext).toHaveBeenCalledTimes(1);
  });

  it('deve desabilitar botão Anterior quando disabledPrevious é true', () => {
    renderWithTheme(
      <Pagination
        currentPage={1}
        pageSize={10}
        totalItems={50}
        onPrevious={vi.fn()}
        onNext={vi.fn()}
        disabledPrevious
      />
    );
    const previousButton = screen.getByText('Anterior');
    expect(previousButton).toBeDisabled();
  });

  it('deve desabilitar botão Próxima quando disabledNext é true', () => {
    renderWithTheme(
      <Pagination
        currentPage={5}
        pageSize={10}
        totalItems={50}
        onPrevious={vi.fn()}
        onNext={vi.fn()}
        disabledNext
      />
    );
    const nextButton = screen.getByText('Próxima');
    expect(nextButton).toBeDisabled();
  });
});

