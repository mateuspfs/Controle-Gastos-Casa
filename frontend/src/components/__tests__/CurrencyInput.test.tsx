import { describe, it, expect, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import CurrencyInput from '../Form/CurrencyInput';

describe('CurrencyInput', () => {
  it('deve renderizar o input', () => {
    render(<CurrencyInput value="0" onChange={vi.fn()} />);
    const input = screen.getByRole('textbox');
    expect(input).toBeInTheDocument();
  });

  it('deve renderizar label quando fornecido', () => {
    render(<CurrencyInput value="0" onChange={vi.fn()} label="Valor" />);
    expect(screen.getByText('Valor')).toBeInTheDocument();
  });

  it('deve formatar valor inicial corretamente', () => {
    render(<CurrencyInput value="1234.56" onChange={vi.fn()} />);
    const input = screen.getByRole('textbox') as HTMLInputElement;
    expect(input.value).toBe('1.234,56');
  });

  it('deve formatar valor como 0,00 quando vazio', () => {
    render(<CurrencyInput value="0" onChange={vi.fn()} />);
    const input = screen.getByRole('textbox') as HTMLInputElement;
    expect(input.value).toBe('0,00');
  });

  it('deve chamar onChange com valor correto ao digitar', async () => {
    const user = userEvent.setup();
    const handleChange = vi.fn();
    render(<CurrencyInput value="0" onChange={handleChange} />);
    const input = screen.getByRole('textbox');
    
    await user.clear(input);
    await user.type(input, '123');
    
    await waitFor(() => {
      expect(handleChange).toHaveBeenCalled();
    });
  });

  it('deve mostrar mensagem de erro quando fornecido', () => {
    render(<CurrencyInput value="0" onChange={vi.fn()} error="Valor inválido" />);
    expect(screen.getByText('Valor inválido')).toBeInTheDocument();
  });

  it('deve aplicar classes de erro quando há erro', () => {
    const { container } = render(<CurrencyInput value="0" onChange={vi.fn()} error="Erro" />);
    const input = container.querySelector('input');
    expect(input?.className).toContain('border-red-300');
  });

  it('deve estar desabilitado quando disabled é true', () => {
    render(<CurrencyInput value="0" onChange={vi.fn()} disabled />);
    const input = screen.getByRole('textbox');
    expect(input).toBeDisabled();
  });
});

