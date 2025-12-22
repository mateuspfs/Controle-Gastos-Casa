import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { pessoasApi } from '../../services/api';
import type { PagedResultDto, PessoaTotaisDto, TotaisGeraisDto } from '../../types/api';
import { formatarDataBr, formatarMoeda } from '../../helpers/masks';
import {
  Container,
  Box,
  PageHeader,
  EmptyState,
  Pagination,
  ActionButtons,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Loading,
  ErrorMessage,
  Search,
  ColoredText,
} from '../../components';
import { swal } from '../../utils/swal';

// Página de listagem de pessoas
export default function PessoasList() {
  const navigate = useNavigate();
  const [pessoas, setPessoas] = useState<PessoaTotaisDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [skip, setSkip] = useState(0);
  const [take] = useState(9);
  const [searchTerm, setSearchTerm] = useState('');
  const [totaisGerais, setTotaisGerais] = useState<TotaisGeraisDto | null>(null);
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalItems: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  // Carrega a lista de pessoas com informações de paginação do backend
  const loadPessoas = async (currentSkip = skip, currentSearchTerm = searchTerm) => {
    try {
      setLoading(true);
      setError(null);
      const result = await pessoasApi.getAll(currentSkip, take, currentSearchTerm || undefined);
      
      if (result.success && result.data) {
        const pagedData = result.data as PagedResultDto<PessoaTotaisDto>;
        setPessoas(pagedData.items);
        setPagination({
          currentPage: pagedData.currentPage,
          totalItems: pagedData.totalItems,
          totalPages: pagedData.totalPages,
          hasPreviousPage: pagedData.hasPreviousPage,
          hasNextPage: pagedData.hasNextPage,
        });
      } else {
        setError(result.errors.join(', ') || 'Erro ao carregar pessoas');
      }
    } catch (err) {
      setError('Erro ao conectar com a API');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Reseta paginação quando busca muda
  useEffect(() => {
    setSkip(0);
  }, [searchTerm]);

  // Carrega totais gerais
  const loadTotaisGerais = async () => {
    try {
      const result = await pessoasApi.getTotaisGerais();
      if (result.success && result.data) {
        setTotaisGerais(result.data);
      }
    } catch (err) {
      console.error('Erro ao carregar totais gerais:', err);
    }
  };

  // Recarrega quando skip ou searchTerm mudam
  useEffect(() => {
    loadPessoas(skip, searchTerm);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [skip, searchTerm]);

  // Carrega totais gerais ao montar o componente
  useEffect(() => {
    loadTotaisGerais();
  }, []);

  // Handler para deletar pessoa
  const handleDelete = async (id: number) => {
    const confirmResult = await swal.confirm(
      'Tem certeza que deseja excluir esta pessoa? Esta ação não pode ser desfeita.',
      'Confirmar exclusão',
      'Excluir',
      'Cancelar'
    );

    if (!confirmResult.isConfirmed) return;

    try {
      const result = await pessoasApi.delete(id);
      if (result.success) {
        swal.successToast('Pessoa excluída com sucesso!');
        // Recarrega a lista após deletar
        loadPessoas();
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao excluir pessoa');
      }
    } catch (err) {
      swal.errorToast('Erro ao conectar com a API');
      console.error(err);
    }
  };

  if (error && pessoas.length === 0) {
    return <ErrorMessage message={error} onRetry={() => loadPessoas()} />;
  }

  return (
    <Container>
      <PageHeader
        title=""
        subtitle={`${pagination.totalItems} ${pagination.totalItems === 1 ? 'pessoa encontrada' : 'pessoas encontradas'}`}
        actionLabel="Nova Pessoa"
        onAction={() => navigate('/pessoas/novo')}
      />

      <Search
        placeholder="Buscar por nome..."
        value={searchTerm}
        onSearch={(term) => setSearchTerm(term)}
        className="mb-4"
      />

      {error && (
        <div className="mb-4 rounded-lg bg-red-50 p-4 dark:bg-red-900/20">
          <p className="text-sm font-medium text-red-800 dark:text-red-200">{error}</p>
        </div>
      )}

      {loading && pessoas.length === 0 ? (
        <Loading message="Carregando pessoas..." />
      ) : (

        <>
          {pessoas.length === 0 ? (
            <EmptyState message="Nenhuma pessoa cadastrada" />
          ) : (
            <Box>
              {loading && (
                <div className="absolute inset-0 flex items-center justify-center bg-white/50 backdrop-blur-sm dark:bg-slate-900/50">
                  <div className="h-6 w-6 animate-spin rounded-full border-4 border-slate-200 border-t-brand-600 dark:border-slate-700 dark:border-t-brand-400"></div>
                </div>
              )}
              <div className={loading ? 'opacity-50' : ''}>
                <Table>
                  <Thead>
                    <Tr isHeader>
                      <Th>Nome</Th>
                      <Th align="center">Data de Nascimento</Th>
                      <Th align="center">Idade</Th>
                      <Th align="right">Receitas</Th>
                      <Th align="right">Despesas</Th>
                      <Th align="right">Saldo</Th>
                      <Th align="right">Ações</Th>
                    </Tr>
                  </Thead>
                  <Tbody>
                    {pessoas.map((pessoa, index) => (
                      <Tr key={pessoa.id} index={index}>
                        <Td className="font-medium">{pessoa.nome}</Td>
                        <Td align="center">{formatarDataBr(pessoa.dataNascimento)}</Td>
                        <Td align="center">{pessoa.idade} {pessoa.idade === 1 ? 'ano' : 'anos'}</Td>
                        <Td align="right">
                          <ColoredText 
                            color={pessoa.totalReceitas > 0 ? 'green' : 'default'} 
                            className="inline-flex items-center gap-1 font-semibold"
                          >
                            {pessoa.totalReceitas > 0 && <span>+</span>}
                            <span>{formatarMoeda(pessoa.totalReceitas)}</span>
                          </ColoredText>
                        </Td>
                        <Td align="right">
                          <ColoredText 
                            color={pessoa.totalDespesas > 0 ? 'red' : 'default'} 
                            className="inline-flex items-center gap-1 font-semibold"
                          >
                            {pessoa.totalDespesas > 0 && <span>-</span>}
                            <span>{formatarMoeda(pessoa.totalDespesas)}</span>
                          </ColoredText>
                        </Td>
                        <Td align="right">
                          <ColoredText 
                            color={pessoa.saldo > 0 ? 'green' : pessoa.saldo < 0 ? 'red' : 'default'} 
                            className="inline-flex items-center gap-1 font-semibold"
                          >
                            {pessoa.saldo > 0 && <span>+</span>}
                            {pessoa.saldo < 0 && <span>-</span>}
                            <span>{formatarMoeda(Math.abs(pessoa.saldo))}</span>
                          </ColoredText>
                        </Td>
                        <Td align="right">
                          <ActionButtons
                            onEdit={() => pessoa.id && navigate(`/pessoas/editar/${pessoa.id}`)}
                            onDelete={() => pessoa.id && handleDelete(pessoa.id)}
                          />
                        </Td>
                      </Tr>
                    ))}
                  </Tbody>
                </Table>
              </div>
            </Box>
          )}

          {/* Paginação dos dados */}
          {pagination.totalPages > 0 && (
            <Pagination
              currentPage={pagination.currentPage}
              pageSize={take}
              totalItems={pagination.totalItems}
              onPrevious={() => setSkip((prev) => Math.max(0, prev - take))}
              onNext={() => setSkip((prev) => prev + take)}
              disabledPrevious={!pagination.hasPreviousPage || loading}
              disabledNext={!pagination.hasNextPage || loading}
            />
          )}

          {/* Totais Gerais */}
          {totaisGerais && (
            <Box className="mt-6">
              <div className="p-6">
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100 mb-6 text-center">
                  Totais Gerais
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div className="bg-green-50 dark:bg-green-900/20 rounded-lg p-4 border border-green-200 dark:border-green-800">
                    <p className="text-sm font-medium text-slate-700 dark:text-slate-300 mb-2 text-center">
                      Total de Receitas
                    </p>
                    <div className="text-center">
                      <ColoredText 
                        color={totaisGerais.totalReceitas > 0 ? 'green' : 'default'} 
                        className="text-2xl font-bold inline-flex items-center gap-1"
                      >
                        {totaisGerais.totalReceitas > 0 && <span>+</span>}
                        <span>{formatarMoeda(totaisGerais.totalReceitas)}</span>
                      </ColoredText>
                    </div>
                  </div>
                  <div className="bg-red-50 dark:bg-red-900/20 rounded-lg p-4 border border-red-200 dark:border-red-800">
                    <p className="text-sm font-medium text-slate-700 dark:text-slate-300 mb-2 text-center">
                      Total de Despesas
                    </p>
                    <div className="text-center">
                      <ColoredText 
                        color={totaisGerais.totalDespesas > 0 ? 'red' : 'default'} 
                        className="text-2xl font-bold inline-flex items-center gap-1"
                      >
                        {totaisGerais.totalDespesas > 0 && <span>-</span>}
                        <span>{formatarMoeda(totaisGerais.totalDespesas)}</span>
                      </ColoredText>
                    </div>
                  </div>
                  <div className={`rounded-lg p-4 border ${
                    totaisGerais.saldoLiquido > 0 
                      ? 'bg-green-50 dark:bg-green-900/20 border-green-200 dark:border-green-800' 
                      : totaisGerais.saldoLiquido < 0 
                      ? 'bg-red-50 dark:bg-red-900/20 border-red-200 dark:border-red-800'
                      : 'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700'
                  }`}>
                    <p className="text-sm font-medium text-slate-700 dark:text-slate-300 mb-2 text-center">
                      Saldo Líquido
                    </p>
                    <div className="text-center">
                      <ColoredText 
                        color={totaisGerais.saldoLiquido > 0 ? 'green' : totaisGerais.saldoLiquido < 0 ? 'red' : 'default'} 
                        className="text-2xl font-bold inline-flex items-center gap-1"
                      >
                        {totaisGerais.saldoLiquido > 0 && <span>+</span>}
                        {totaisGerais.saldoLiquido < 0 && <span>-</span>}
                        <span>{formatarMoeda(Math.abs(totaisGerais.saldoLiquido))}</span>
                      </ColoredText>
                    </div>
                  </div>
                </div>
              </div>
            </Box>
          )}
        </>
      )}
    </Container>
  );
}

