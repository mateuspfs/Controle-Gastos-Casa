import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import ColoredText from '../ColoredText';

describe('ColoredText', () => {
  it('deve renderizar o texto', () => {
    render(<ColoredText>Texto de teste</ColoredText>);
    expect(screen.getByText('Texto de teste')).toBeInTheDocument();
  });

  it('deve aplicar cor verde quando especificado', () => {
    const { container } = render(<ColoredText color="green">Verde</ColoredText>);
    const span = container.querySelector('span');
    expect(span?.className).toContain('text-green-600');
  });

  it('deve aplicar cor vermelha quando especificado', () => {
    const { container } = render(<ColoredText color="red">Vermelho</ColoredText>);
    const span = container.querySelector('span');
    expect(span?.className).toContain('text-red-600');
  });

  it('deve aplicar cor padrão quando não especificado', () => {
    const { container } = render(<ColoredText>Padrão</ColoredText>);
    const span = container.querySelector('span');
    expect(span?.className).toContain('text-slate-700');
  });

  it('deve aplicar className customizada', () => {
    const { container } = render(<ColoredText className="custom-class">Teste</ColoredText>);
    const span = container.querySelector('span');
    expect(span?.className).toContain('custom-class');
  });

  it('deve renderizar números corretamente', () => {
    render(<ColoredText>{100}</ColoredText>);
    expect(screen.getByText('100')).toBeInTheDocument();
  });
});

