/**
 * Calcula idade (anos e meses) a partir da data de nascimento
 * @param dataNascimento - String da data no formato ISO ou Date object
 * @returns Objeto com anos e meses ou null se inválida
 */
export const calcularIdade = (dataNascimento: string | Date): { anos: number; meses: number } | null => {
  if (!dataNascimento) return null;
  
  const hoje = new Date();
  const nascimento = typeof dataNascimento === 'string' ? new Date(dataNascimento) : dataNascimento;
  
  let anos = hoje.getFullYear() - nascimento.getFullYear();
  let meses = hoje.getMonth() - nascimento.getMonth();
  
  // Ajusta se ainda não fez aniversário neste mês
  if (hoje.getDate() < nascimento.getDate()) {
    meses--;
    if (meses < 0) {
      meses += 12;
      anos--;
    }
  }
  
  if (meses < 0) {
    meses += 12;
    anos--;
  }
  
  return { anos, meses };
};

/**
 * Formata idade para exibição: se tiver ao menos 1 ano, retorna anos, caso contrário retorna meses
 */
export const formatarIdade = (idade: { anos: number; meses: number }): string => 
  idade.anos >= 1 
    ? `${idade.anos} ano(s)` 
    : `${idade.meses} mês(es)`;

