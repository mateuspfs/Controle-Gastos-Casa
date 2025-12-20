namespace ControleGastosCasa.Application.Helpers;

// Helper para cálculos relacionados a datas
public static class DateHelper
{
    // Calcula a idade em anos
    public static int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (hoje.DayOfYear < dataNascimento.DayOfYear) idade--;
        
        return idade;
    }

    // Formata a string de idade: se tiver ao menos 1 ano, retorna anos, caso contrário retorna meses
    public static string CalcularIdadeFormatada(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var nascimento = dataNascimento;
        
        int anos = hoje.Year - nascimento.Year;
        int meses = hoje.Month - nascimento.Month;
        
        // Ajusta se ainda não fez aniversário neste mês
        if (hoje.Day < nascimento.Day)
        {
            meses--;
            if (meses < 0)
            {
                meses += 12;
                anos--;
            }
        }
        
        if (meses < 0)
        {
            meses += 12;
            anos--;
        }
        
        return anos >= 1 
            ? $"{anos} ano(s)" 
            : $"{meses} mês(es)";
    }
}

