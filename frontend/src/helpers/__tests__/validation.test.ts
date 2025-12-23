import { describe, it, expect } from 'vitest';
import { getFieldError } from '../validation';

describe('validation', () => {
  describe('getFieldError', () => {
    it('deve retornar erro do campo quando encontrado (case-insensitive)', () => {
      const validationError = {
        errors: {
          'Nome': ['O campo Nome é obrigatório'],
          'Email': ['Email inválido']
        }
      };

      expect(getFieldError(validationError, 'nome')).toBe('O campo Nome é obrigatório');
      expect(getFieldError(validationError, 'NOME')).toBe('O campo Nome é obrigatório');
      expect(getFieldError(validationError, 'Email')).toBe('Email inválido');
    });

    it('deve retornar undefined quando campo não encontrado', () => {
      const validationError = {
        errors: {
          'Nome': ['O campo Nome é obrigatório']
        }
      };

      expect(getFieldError(validationError, 'Email')).toBeUndefined();
    });

    it('deve retornar undefined quando não há erros', () => {
      const validationError = {
        errors: {}
      };

      expect(getFieldError(validationError, 'Nome')).toBeUndefined();
    });

    it('deve retornar primeiro erro quando há múltiplos erros', () => {
      const validationError = {
        errors: {
          'Nome': ['Erro 1', 'Erro 2', 'Erro 3']
        }
      };

      expect(getFieldError(validationError, 'nome')).toBe('Erro 1');
    });

    it('deve retornar undefined quando array de erros está vazio', () => {
      const validationError = {
        errors: {
          'Nome': []
        }
      };

      expect(getFieldError(validationError, 'nome')).toBeUndefined();
    });
  });
});

