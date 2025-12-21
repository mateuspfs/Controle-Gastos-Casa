import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { pessoasApi } from '../../services/api';
import type { PagedResultDto, PessoaDto } from '../../types/api';
import { formatarDataBr } from '../../helpers/masks';
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
} from '../../components';
import { swal } from '../../utils/swal';

// Página de listagem de pessoas
export default function PessoasList() {
  const navigate = useNavigate();
  const [pessoas, setPessoas] = useState<PessoaDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [skip, setSkip] = useState(0);
  const [take] = useState(9);
  const [searchTerm, setSearchTerm] = useState('');
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
        const pagedData = result.data as PagedResultDto<PessoaDto>;
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

  // Recarrega quando skip ou searchTerm mudam
  useEffect(() => {
    loadPessoas(skip, searchTerm);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [skip, searchTerm]);

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
                      <Th align="right">Ações</Th>
                    </Tr>
                  </Thead>
                  <Tbody>
                    {pessoas.map((pessoa, index) => (
                      <Tr key={pessoa.id} index={index}>
                        <Td className="font-medium">{pessoa.nome}</Td>
                        <Td align="center">{formatarDataBr(pessoa.dataNascimento)}</Td>
                        <Td align="center">{pessoa.idade ?? 'N/A'}</Td>
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
        </>
      )}
    </Container>
  );
}

