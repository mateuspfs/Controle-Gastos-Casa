import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

/**
 * Formata uma data no padrão brasileiro (dd/mm/aaaa)
 * @param data - String da data no formato ISO ou Date object
 * @returns String formatada no padrão brasileiro ou 'N/A' se inválida
 */
export const formatarDataBr = (data: string | Date | null | undefined): string => {
  if (!data) return 'N/A';
  
  try {
    const date = typeof data === 'string' ? new Date(data) : data;
    return format(date, 'dd/MM/yyyy', { locale: ptBR });
  } catch {
    return 'N/A';
  }
};

