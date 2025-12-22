import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { transacoesApi, pessoasApi, categoriasApi } from '../../services/api';
import type { TransacaoDto, PessoaDto, CategoriaDto } from '../../types/api';
import { TipoTransacao } from '../../types/api';
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
  Button,
  Input,
  Select,
  TotaisGerais,
} from '../../components';
import { swal } from '../../utils/swal';

// Função auxiliar para formatar tipo
const formatarTipo = (tipo: number): string => {
  switch (tipo) {
    case TipoTransacao.Despesa:
      return 'Despesa';
    case TipoTransacao.Receita:
      return 'Receita';
    default:
      return 'N/A';
  }
};

// Função auxiliar para obter classe de cor baseada no tipo
const getTipoColorClass = (tipo: number): string => {
  switch (tipo) {
    case TipoTransacao.Despesa:
      return 'text-red-600 dark:text-red-400 font-medium';
    case TipoTransacao.Receita:
      return 'text-green-600 dark:text-green-400 font-medium';
    default:
      return 'text-slate-700 dark:text-slate-300';
  }
};

// Página de listagem de transações
export default function TransacoesList() {
  const navigate = useNavigate();
  const [transacoes, setTransacoes] = useState<TransacaoDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [skip, setSkip] = useState(0);
  const [take] = useState(9);
  const [dataInicio, setDataInicio] = useState<string>('');
  const [dataFim, setDataFim] = useState<string>('');
  const [pessoaId, setPessoaId] = useState<number>(0);
  const [categoriaId, setCategoriaId] = useState<number>(0);
  const [tipo, setTipo] = useState<number | undefined>(undefined);
  const [pessoas, setPessoas] = useState<PessoaDto[]>([]);
  const [categorias, setCategorias] = useState<CategoriaDto[]>([]);
  const [loadingData, setLoadingData] = useState(true);
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalItems: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  // Carrega pessoas e categorias para os filtros
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoadingData(true);
        const [pessoasResult, categoriasResult] = await Promise.all([
          pessoasApi.getAll(0, 0),
          categoriasApi.getAll(0, 0),
        ]);

        if (pessoasResult.success && pessoasResult.data) {
          setPessoas(pessoasResult.data as PessoaDto[]);
        }

        if (categoriasResult.success && categoriasResult.data) {
          setCategorias(categoriasResult.data as CategoriaDto[]);
        }
      } catch (err) {
        console.error('Erro ao carregar dados:', err);
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, []);

  // Carrega a lista de transações com informações de paginação do backend
  const loadTransacoes = async (
    currentSkip = skip,
    currentDataInicio = dataInicio,
    currentDataFim = dataFim,
    currentPessoaId = pessoaId,
    currentCategoriaId = categoriaId,
    currentTipo = tipo
  ) => {
    try {
      setLoading(true);
      setError(null);
      const result = await transacoesApi.getAll(
        currentSkip,
        take,
        currentDataInicio || undefined,
        currentDataFim || undefined,
        currentPessoaId > 0 ? currentPessoaId : undefined,
        currentCategoriaId > 0 ? currentCategoriaId : undefined,
        currentTipo
      );

      if (result.success && result.data) {
        setTransacoes(result.data.items);
        setPagination({
          currentPage: result.data.currentPage,
          totalItems: result.data.totalItems,
          totalPages: result.data.totalPages,
          hasPreviousPage: result.data.hasPreviousPage,
          hasNextPage: result.data.hasNextPage,
        });
      } else {
        setError(result.errors.join(', ') || 'Erro ao carregar transações');
      }
    } catch (err) {
      setError('Erro ao conectar com a API');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Reseta paginação quando filtros mudam
  useEffect(() => {
    setSkip(0);
  }, [dataInicio, dataFim, pessoaId, categoriaId, tipo]);

  // Recarrega quando skip ou filtros mudam
  useEffect(() => {
    if (!loadingData) {
      loadTransacoes(skip, dataInicio, dataFim, pessoaId, categoriaId, tipo);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [skip, dataInicio, dataFim, pessoaId, categoriaId, tipo, loadingData]);

  // Handler para navegação de páginas
  const handlePageChange = (newPage: number) => {
    const newSkip = (newPage - 1) * take;
    setSkip(newSkip);
  };

  // Handler para limpar filtros
  const handleLimparFiltros = () => {
    setDataInicio('');
    setDataFim('');
    setPessoaId(0);
    setCategoriaId(0);
    setTipo(undefined);
    setSkip(0);
  };

  // Handler para deletar transação
  const handleDelete = async (id: number) => {
    const confirm = await swal.confirm(
      'Tem certeza?',
      'Esta ação não pode ser desfeita!'
    );

    if (!confirm) return;

    try {
      const result = await transacoesApi.delete(id);
      if (result.success) {
        swal.successToast('Transação excluída com sucesso!');
        loadTransacoes(skip, dataInicio, dataFim, pessoaId, categoriaId, tipo);
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao excluir transação');
      }
    } catch (err) {
      swal.errorToast('Erro ao conectar com a API');
      console.error(err);
    }
  };

  return (
    <Container>
      <PageHeader
        title="Transações"
        subtitle="Gerencie suas transações financeiras"
        onAction={() => navigate('/transacoes/novo')}
        actionLabel="Nova Transação"
      />

      {/* Filtros */}
      <Box className="mb-6 border-0 shadow-none bg-transparent dark:bg-transparent">
        <div className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <Input
              type="date"
              label="Data de Início"
              value={dataInicio}
              onChange={(e) => setDataInicio(e.target.value)}
            />
            <Input
              type="date"
              label="Data do Fim"
              value={dataFim}
              onChange={(e) => setDataFim(e.target.value)}
              min={dataInicio || undefined}
            />
            <Select
              label="Pessoa"
              value={pessoaId}
              onChange={(e) => setPessoaId(Number(e.target.value))}
              options={[
                { value: 0, label: 'Todas as pessoas' },
                ...pessoas.map((pessoa) => ({
                  value: pessoa.id!,
                  label: pessoa.nome,
                })),
              ]}
            />
            <Select
              label="Categoria"
              value={categoriaId}
              onChange={(e) => setCategoriaId(Number(e.target.value))}
              options={[
                { value: 0, label: 'Todas as categorias' },
                ...categorias.map((categoria) => ({
                  value: categoria.id!,
                  label: categoria.descricao,
                })),
              ]}
            />
            <Select
              label="Tipo"
              value={tipo ?? ''}
              onChange={(e) => setTipo(e.target.value ? Number(e.target.value) : undefined)}
              options={[
                { value: '', label: 'Todos os tipos' },
                { value: TipoTransacao.Despesa, label: 'Despesa' },
                { value: TipoTransacao.Receita, label: 'Receita' },
              ]}
            />
            <div className="flex flex-col">
              <label className="mb-1.5 block text-sm font-medium text-slate-700 dark:text-slate-300 opacity-0 pointer-events-none">
                &nbsp;
              </label>
              <Button
                variant="outline"
                onClick={handleLimparFiltros}
                className="w-full h-[42px]"
              >
                Limpar Filtros
              </Button>
            </div>
          </div>
        </div>
      </Box>

      {loading && transacoes.length === 0 ? (
        <Loading message="Carregando transações..." />
      ) : error && transacoes.length === 0 ? (
        <ErrorMessage message={error} onRetry={() => loadTransacoes()} />
      ) : transacoes.length === 0 ? (
        <EmptyState
          message={
            dataInicio || dataFim || pessoaId > 0 || categoriaId > 0 || tipo !== undefined
              ? 'Nenhuma transação encontrada com os filtros selecionados.'
              : 'Nenhuma transação cadastrada ainda.'
          }
        />
      ) : (
        <>
          {loading && (
            <div className="absolute inset-0 flex items-center justify-center bg-white/50 backdrop-blur-sm dark:bg-slate-900/50 z-10">
              <Loading />
            </div>
          )}
          <Box>
            <Table>
              <Thead>
                <Tr isHeader>
                  <Th>Data</Th>
                  <Th>Descrição</Th>
                  <Th>Pessoa</Th>
                  <Th>Categoria</Th>
                  <Th align="center">Tipo</Th>
                  <Th align="right">Valor</Th>
                  <Th align="right">Ações</Th>
                </Tr>
              </Thead>
              <Tbody>
                {transacoes.map((transacao, index) => (
                  <Tr key={transacao.id} index={index}>
                    <Td>{formatarDataBr(transacao.dataTransacao)}</Td>
                    <Td>{transacao.descricao}</Td>
                    <Td>{transacao.pessoa?.nome || '-'}</Td>
                    <Td>{transacao.categoria?.descricao || '-'}</Td>
                    <Td align="center">
                      <span className={getTipoColorClass(transacao.tipo)}>
                        {formatarTipo(transacao.tipo)}
                      </span>
                    </Td>
                    <Td align="right">
                      <span className={getTipoColorClass(transacao.tipo)}>
                        {formatarTipo(transacao.tipo) === 'Despesa' ? '-' : '+'}
                        {formatarMoeda(transacao.valor)}
                      </span>
                    </Td>
                    <Td align="right">
                      <ActionButtons
                        onEdit={() => navigate(`/transacoes/editar/${transacao.id}`)}
                        onDelete={() => handleDelete(transacao.id!)}
                      />
                    </Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Box>

          {pagination.totalPages > 1 && (
            <div className="mt-6">
              <Pagination
                currentPage={pagination.currentPage}
                pageSize={take}
                totalItems={pagination.totalItems}
                onPrevious={() => handlePageChange(pagination.currentPage - 1)}
                onNext={() => handlePageChange(pagination.currentPage + 1)}
                disabledPrevious={!pagination.hasPreviousPage || loading}
                disabledNext={!pagination.hasNextPage || loading}
              />
            </div>
          )}

          {/* Totais Gerais */}
          <TotaisGerais 
            dataInicio={dataInicio || undefined}
            dataFim={dataFim || undefined}
            pessoaId={pessoaId > 0 ? pessoaId : undefined}
            categoriaId={categoriaId > 0 ? categoriaId : undefined}
            tipo={tipo}
          />
        </>
      )}
    </Container>
  );
}