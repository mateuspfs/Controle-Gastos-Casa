import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Input from '../Form/Input';

describe('Input', () => {
  it('deve renderizar o input', () => {
    render(<Input />);
    const input = screen.getByRole('textbox');
    expect(input).toBeInTheDocument();
  });

  it('deve renderizar label quando fornecido', () => {
    render(<Input label="Nome" />);
    expect(screen.getByText('Nome')).toBeInTheDocument();
  });

  it('deve mostrar asterisco quando required é true', () => {
    render(<Input label="Nome" required />);
    expect(screen.getByText('*')).toBeInTheDocument();
  });

  it('deve mostrar mensagem de erro quando fornecido', () => {
    render(<Input error="Campo obrigatório" />);
    expect(screen.getByText('Campo obrigatório')).toBeInTheDocument();
  });

  it('deve aplicar classes de erro quando há erro', () => {
    const { container } = render(<Input error="Erro" />);
    const input = container.querySelector('input');
    expect(input?.className).toContain('border-red-300');
  });

  it('deve permitir digitação', async () => {
    const user = userEvent.setup();
    render(<Input />);
    const input = screen.getByRole('textbox');
    
    await user.type(input, 'Texto de teste');
    expect(input).toHaveValue('Texto de teste');
  });

  it('deve chamar onChange quando valor muda', async () => {
    const user = userEvent.setup();
    const handleChange = vi.fn();
    render(<Input onChange={handleChange} />);
    const input = screen.getByRole('textbox');
    
    await user.type(input, 'a');
    expect(handleChange).toHaveBeenCalled();
  });

  it('deve aplicar placeholder quando fornecido', () => {
    render(<Input placeholder="Digite seu nome" />);
    const input = screen.getByPlaceholderText('Digite seu nome');
    expect(input).toBeInTheDocument();
  });

  it('deve estar desabilitado quando disabled é true', () => {
    render(<Input disabled />);
    const input = screen.getByRole('textbox');
    expect(input).toBeDisabled();
  });

  it('deve aplicar className customizada', () => {
    const { container } = render(<Input className="custom-class" />);
    const input = container.querySelector('input');
    expect(input?.className).toContain('custom-class');
  });
});

