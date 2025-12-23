import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Button from '../Button';

describe('Button', () => {
  it('deve renderizar o texto do botão', () => {
    render(<Button>Clique aqui</Button>);
    expect(screen.getByText('Clique aqui')).toBeInTheDocument();
  });

  it('deve aplicar variante primary por padrão', () => {
    const { container } = render(<Button>Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('bg-brand-600');
  });

  it('deve aplicar variante secondary quando especificado', () => {
    const { container } = render(<Button variant="secondary">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('bg-slate-200');
  });

  it('deve aplicar variante danger quando especificado', () => {
    const { container } = render(<Button variant="danger">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('bg-red-600');
  });

  it('deve aplicar variante outline quando especificado', () => {
    const { container } = render(<Button variant="outline">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('border');
  });

  it('deve aplicar tamanho sm quando especificado', () => {
    const { container } = render(<Button size="sm">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('px-3');
    expect(button?.className).toContain('text-xs');
  });

  it('deve aplicar tamanho md por padrão', () => {
    const { container } = render(<Button>Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('px-4');
    expect(button?.className).toContain('text-sm');
  });

  it('deve aplicar tamanho lg quando especificado', () => {
    const { container } = render(<Button size="lg">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('px-6');
    expect(button?.className).toContain('text-base');
  });

  it('deve estar desabilitado quando disabled é true', () => {
    render(<Button disabled>Teste</Button>);
    const button = screen.getByText('Teste');
    expect(button).toBeDisabled();
  });

  it('deve chamar onClick quando clicado', async () => {
    const user = userEvent.setup();
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>Clique</Button>);
    
    await user.click(screen.getByText('Clique'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('deve aplicar className customizada', () => {
    const { container } = render(<Button className="custom-class">Teste</Button>);
    const button = container.querySelector('button');
    expect(button?.className).toContain('custom-class');
  });
});

