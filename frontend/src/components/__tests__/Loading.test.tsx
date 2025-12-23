import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import Loading from '../Loading';

describe('Loading', () => {
  it('deve renderizar mensagem padrão', () => {
    render(<Loading />);
    expect(screen.getByText('Carregando...')).toBeInTheDocument();
  });

  it('deve renderizar mensagem customizada', () => {
    render(<Loading message="Aguarde..." />);
    expect(screen.getByText('Aguarde...')).toBeInTheDocument();
  });

  it('deve renderizar spinner', () => {
    const { container } = render(<Loading />);
    const spinner = container.querySelector('.animate-spin');
    expect(spinner).toBeInTheDocument();
  });
});

