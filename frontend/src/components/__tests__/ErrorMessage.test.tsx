import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ErrorMessage from '../ErrorMessage';

describe('ErrorMessage', () => {
  it('deve renderizar mensagem de erro', () => {
    render(<ErrorMessage message="Erro ao carregar dados" />);
    expect(screen.getByText('Erro ao carregar dados')).toBeInTheDocument();
  });

  it('deve renderizar botão de retry quando onRetry é fornecido', () => {
    const handleRetry = vi.fn();
    render(<ErrorMessage message="Erro" onRetry={handleRetry} />);
    expect(screen.getByText('Tentar novamente')).toBeInTheDocument();
  });

  it('não deve renderizar botão de retry quando onRetry não é fornecido', () => {
    render(<ErrorMessage message="Erro" />);
    expect(screen.queryByText('Tentar novamente')).not.toBeInTheDocument();
  });

  it('deve chamar onRetry quando botão é clicado', async () => {
    const user = userEvent.setup();
    const handleRetry = vi.fn();
    render(<ErrorMessage message="Erro" onRetry={handleRetry} />);
    
    const button = screen.getByText('Tentar novamente');
    await user.click(button);
    expect(handleRetry).toHaveBeenCalledTimes(1);
  });
});

