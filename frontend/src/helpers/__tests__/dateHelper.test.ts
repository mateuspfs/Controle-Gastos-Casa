import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { calcularIdade, formatarIdade } from '../dateHelper';

describe('dateHelper', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2024-12-25'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  describe('calcularIdade', () => {
    it('deve calcular idade corretamente para pessoa que já fez aniversário', () => {
      const dataNascimento = new Date('2000-01-15');
      const resultado = calcularIdade(dataNascimento);
      
      expect(resultado).toEqual({ anos: 24, meses: 11 });
    });

    it('deve calcular idade corretamente para pessoa que ainda não fez aniversário', () => {
      const dataNascimento = new Date('2000-12-30');
      const resultado = calcularIdade(dataNascimento);
      
      expect(resultado).toEqual({ anos: 23, meses: 11 });
    });

    it('deve calcular idade corretamente para pessoa nascida no mesmo mês', () => {
      const dataNascimento = new Date('2000-12-20');
      const resultado = calcularIdade(dataNascimento);
      
      expect(resultado).toEqual({ anos: 24, meses: 0 });
    });

    it('deve calcular idade corretamente para string ISO', () => {
      const resultado = calcularIdade('2000-01-15');
      
      expect(resultado).toEqual({ anos: 24, meses: 11 });
    });

    it('deve retornar null para data inválida', () => {
      expect(calcularIdade(null as any)).toBeNull();
      expect(calcularIdade(undefined as any)).toBeNull();
    });

    it('deve calcular idade corretamente para bebê de menos de 1 ano', () => {
      const dataNascimento = new Date('2024-06-15');
      const resultado = calcularIdade(dataNascimento);
      
      expect(resultado).toEqual({ anos: 0, meses: 6 });
    });
  });

  describe('formatarIdade', () => {
    it('deve formatar idade em anos quando tiver 1 ou mais anos', () => {
      expect(formatarIdade({ anos: 1, meses: 0 })).toBe('1 ano(s)');
      expect(formatarIdade({ anos: 5, meses: 3 })).toBe('5 ano(s)');
      expect(formatarIdade({ anos: 24, meses: 11 })).toBe('24 ano(s)');
    });

    it('deve formatar idade em meses quando tiver menos de 1 ano', () => {
      expect(formatarIdade({ anos: 0, meses: 1 })).toBe('1 mês(es)');
      expect(formatarIdade({ anos: 0, meses: 6 })).toBe('6 mês(es)');
      expect(formatarIdade({ anos: 0, meses: 11 })).toBe('11 mês(es)');
    });
  });
});

