import { useEffect, useState } from 'react';
import { Box, ColoredText } from './index';
import { formatarMoeda } from '../helpers/masks';
import { transacoesApi } from '../services/api';
import type { TotaisGeraisDto } from '../types/api';

interface TotaisGeraisProps {
  dataInicio?: string;
  dataFim?: string;
  pessoaId?: number;
  categoriaId?: number;
  tipo?: number;
}

// Componente reutilizável para exibir totais gerais (receitas, despesas e saldo líquido)
export default function TotaisGerais({ 
  dataInicio, 
  dataFim, 
  pessoaId, 
  categoriaId, 
  tipo 
}: TotaisGeraisProps) {
  const [totais, setTotais] = useState<TotaisGeraisDto | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadTotais = async () => {
      try {
        setLoading(true);
        const result = await transacoesApi.getTotaisGerais(
          dataInicio,
          dataFim,
          pessoaId && pessoaId > 0 ? pessoaId : undefined,
          categoriaId && categoriaId > 0 ? categoriaId : undefined,
          tipo
        );
        if (result.success && result.data) {
          setTotais(result.data);
        } else {
          console.error('Erro ao carregar totais gerais:', result.errors.join(', '));
          setTotais(null);
        }
      } catch (err) {
        console.error('Erro ao conectar com a API para totais gerais:', err);
        setTotais(null);
      } finally {
        setLoading(false);
      }
    };

    loadTotais();
  }, [dataInicio, dataFim, pessoaId, categoriaId, tipo]);

  if (loading) {
    return (
      <Box className="mt-6 p-6">
        <div className="text-center text-slate-600 dark:text-slate-400">
          Carregando totais...
        </div>
      </Box>
    );
  }

  if (!totais) {
    return null;
  }

  return (
    <Box className="mt-6 p-6">
      <h3 className="text-xl font-bold text-center text-slate-900 dark:text-slate-100 mb-6">
        Totais Gerais
      </h3>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="flex flex-col items-center justify-center p-4 rounded-lg border border-green-300 bg-green-50 dark:border-green-700 dark:bg-green-900/20">
          <p className="text-sm text-slate-600 dark:text-slate-400 mb-2">Total de Receitas</p>
          <ColoredText 
            color={totais.totalReceitas > 0 ? 'green' : 'default'} 
            className="text-2xl font-bold inline-flex items-center gap-2"
          >
            {totais.totalReceitas > 0 && <span>+</span>}
            <span>{formatarMoeda(totais.totalReceitas)}</span>
          </ColoredText>
        </div>
        <div className="flex flex-col items-center justify-center p-4 rounded-lg border border-red-300 bg-red-50 dark:border-red-700 dark:bg-red-900/20">
          <p className="text-sm text-slate-600 dark:text-slate-400 mb-2">Total de Despesas</p>
          <ColoredText 
            color={totais.totalDespesas > 0 ? 'red' : 'default'} 
            className="text-2xl font-bold inline-flex items-center gap-2"
          >
            {totais.totalDespesas > 0 && <span>-</span>}
            <span>{formatarMoeda(totais.totalDespesas)}</span>
          </ColoredText>
        </div>
        <div className={`flex flex-col items-center justify-center p-4 rounded-lg border ${
          totais.saldoLiquido > 0 
            ? 'border-green-300 bg-green-50 dark:border-green-700 dark:bg-green-900/20' 
            : totais.saldoLiquido < 0 
            ? 'border-red-300 bg-red-50 dark:border-red-700 dark:bg-red-900/20' 
            : 'border-slate-300 bg-white dark:border-slate-600 dark:bg-slate-800'
        }`}>
          <p className="text-sm text-slate-600 dark:text-slate-400 mb-2">Saldo Líquido</p>
          <ColoredText 
            color={totais.saldoLiquido > 0 ? 'green' : totais.saldoLiquido < 0 ? 'red' : 'default'} 
            className="text-2xl font-bold inline-flex items-center gap-2"
          >
            {totais.saldoLiquido > 0 && <span>+</span>}
            {totais.saldoLiquido < 0 && <span>-</span>}
            <span>{formatarMoeda(Math.abs(totais.saldoLiquido))}</span>
          </ColoredText>
        </div>
      </div>
    </Box>
  );
}

