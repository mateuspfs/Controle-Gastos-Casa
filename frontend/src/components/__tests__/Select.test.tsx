import { describe, it, expect, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Select from '../Form/Select';

describe('Select', () => {
  const options = [
    { value: 1, label: 'Opção 1' },
    { value: 2, label: 'Opção 2' },
    { value: 3, label: 'Opção 3' },
  ];

  it('deve renderizar o select', () => {
    render(<Select options={options} />);
    const input = screen.getByRole('textbox');
    expect(input).toBeInTheDocument();
  });

  it('deve renderizar label quando fornecido', () => {
    render(<Select options={options} label="Selecione" />);
    expect(screen.getByText('Selecione')).toBeInTheDocument();
  });

  it('deve mostrar placeholder quando fornecido', () => {
    render(<Select options={options} placeholder="Escolha uma opção" />);
    expect(screen.getByPlaceholderText('Escolha uma opção')).toBeInTheDocument();
  });

  it('deve abrir dropdown ao clicar no input', async () => {
    const user = userEvent.setup();
    render(<Select options={options} />);
    const input = screen.getByRole('textbox');
    
    await user.click(input);
    await waitFor(() => {
      expect(screen.getByText('Opção 1')).toBeInTheDocument();
    });
  });

  it('deve filtrar opções ao digitar', async () => {
    const user = userEvent.setup();
    render(<Select options={options} />);
    const input = screen.getByRole('textbox');
    
    await user.click(input);
    await user.type(input, 'Opção 2');
    
    await waitFor(() => {
      expect(screen.getByText('Opção 2')).toBeInTheDocument();
      expect(screen.queryByText('Opção 1')).not.toBeInTheDocument();
    });
  });

  it('deve selecionar opção ao clicar', async () => {
    const user = userEvent.setup();
    const handleChange = vi.fn();
    render(<Select options={options} onChange={handleChange} />);
    const input = screen.getByRole('textbox');
    
    await user.click(input);
    await waitFor(() => {
      expect(screen.getByText('Opção 1')).toBeInTheDocument();
    });
    
    await user.click(screen.getByText('Opção 1'));
    expect(handleChange).toHaveBeenCalled();
  });

  it('deve mostrar valor selecionado quando value é fornecido', () => {
    render(<Select options={options} value={1} />);
    const input = screen.getByRole('textbox') as HTMLInputElement;
    expect(input.value).toBe('Opção 1');
  });

  it('deve mostrar mensagem de erro quando fornecido', () => {
    render(<Select options={options} error="Campo obrigatório" />);
    expect(screen.getByText('Campo obrigatório')).toBeInTheDocument();
  });

  it('deve estar desabilitado quando disabled é true', () => {
    render(<Select options={options} disabled />);
    const input = screen.getByRole('textbox');
    expect(input).toBeDisabled();
  });
});

