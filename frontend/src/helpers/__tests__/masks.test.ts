import { describe, it, expect } from 'vitest';
import { formatarDataBr, formatarMoeda } from '../masks';

describe('masks', () => {
  describe('formatarDataBr', () => {
    it('deve formatar data ISO string corretamente', () => {
      // Usa data com hora para evitar problemas de timezone
      expect(formatarDataBr('2024-12-25T12:00:00')).toBe('25/12/2024');
    });

    it('deve formatar objeto Date corretamente', () => {
      const date = new Date(2024, 11, 25);
      expect(formatarDataBr(date)).toBe('25/12/2024');
    });

    it('deve retornar "N/A" para null', () => {
      expect(formatarDataBr(null)).toBe('N/A');
    });

    it('deve retornar "N/A" para undefined', () => {
      expect(formatarDataBr(undefined)).toBe('N/A');
    });

    it('deve retornar "N/A" para data inválida', () => {
      expect(formatarDataBr('data-invalida')).toBe('N/A');
    });
  });

  describe('formatarMoeda', () => {
    it('deve formatar número corretamente', () => {
      const resultado = formatarMoeda(1234.56);
      expect(resultado.replace(/\u00A0/g, ' ')).toBe('R$ 1.234,56');
    });

    it('deve formatar string numérica corretamente', () => {
      const resultado = formatarMoeda('1234.56');
      expect(resultado.replace(/\u00A0/g, ' ')).toBe('R$ 1.234,56');
    });

    it('deve formatar zero corretamente', () => {
      const resultado = formatarMoeda(0);
      expect(resultado.replace(/\u00A0/g, ' ')).toBe('R$ 0,00');
    });

    it('deve retornar "R$ 0,00" para valor inválido', () => {
      const resultado = formatarMoeda('invalido');
      expect(resultado.replace(/\u00A0/g, ' ')).toBe('R$ 0,00');
    });

    it('deve formatar valores grandes corretamente', () => {
      const resultado = formatarMoeda(1234567.89);
      expect(resultado.replace(/\u00A0/g, ' ')).toBe('R$ 1.234.567,89');
    });
  });

});

