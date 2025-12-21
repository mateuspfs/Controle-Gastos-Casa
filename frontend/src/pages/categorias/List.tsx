import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoriasApi } from '../../services/api';
import type { CategoriaDto, PagedResultDto } from '../../types/api';
import { FinalidadeCategoria } from '../../types/api';
import { Select } from '../../components/Form';
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

// Função auxiliar para formatar finalidade
const formatarFinalidade = (finalidade: number): string => {
  switch (finalidade) {
    case 1: return 'Despesa';
    case 2: return 'Receita';
    case 3: return 'Ambas';
    default: return 'N/A';
  }
};

// Função auxiliar para obter classe de cor baseada na finalidade
const getFinalidadeColorClass = (finalidade: number): string => {
  switch (finalidade) {
    case 1: return 'text-red-600 dark:text-red-400 font-medium';
    case 2: return 'text-green-600 dark:text-green-400 font-medium';
    case 3: return 'text-slate-700 dark:text-slate-300';
    default: return 'text-slate-700 dark:text-slate-300';
  }
};

// Página de listagem de categorias
export default function CategoriasList() {
  const navigate = useNavigate();
  const [categorias, setCategorias] = useState<CategoriaDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [skip, setSkip] = useState(0);
  const [take] = useState(9);
  const [searchTerm, setSearchTerm] = useState('');
  const [finalidade, setFinalidade] = useState<number | null>(null);
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalItems: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  // Carrega a lista de categorias paginadas
  const loadCategorias = async (currentSkip = skip, currentSearchTerm = searchTerm, currentFinalidade = finalidade) => {
    try {
      setLoading(true);
      setError(null);
      const result = await categoriasApi.getAll(currentSkip, take, currentSearchTerm || undefined, currentFinalidade ?? undefined);
      
      if (result.success && result.data) {
        const pagedData = result.data as PagedResultDto<CategoriaDto>;
        setCategorias(pagedData.items);
        setPagination({
          currentPage: pagedData.currentPage,
          totalItems: pagedData.totalItems,
          totalPages: pagedData.totalPages,
          hasPreviousPage: pagedData.hasPreviousPage,
          hasNextPage: pagedData.hasNextPage,
        });
      } else {
        setError(result.errors.join(', ') || 'Erro ao carregar categorias');
      }
    } catch (err) {
      setError('Erro ao conectar com a API');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Reseta paginação quando busca ou filtro muda
  useEffect(() => {
    setSkip(0);
  }, [searchTerm, finalidade]);

  // Recarrega quando skip, searchTerm ou finalidade mudam
  useEffect(() => {
    loadCategorias(skip, searchTerm, finalidade);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [skip, searchTerm, finalidade]);

  // Handler para navegação de páginas
  const handlePageChange = (newPage: number) => {
    const newSkip = (newPage - 1) * take;
    setSkip(newSkip);
  };

  // Handler para busca
  const handleSearch = (term: string) => {
    setSearchTerm(term);
  };

  // Handler para filtro de finalidade
  const handleFinalidadeChange = (value: string) => {
    const finalidadeValue = value === '' ? null : Number(value);
    setFinalidade(finalidadeValue);
  };

  const finalidadeOptions = [
    { value: '', label: 'Todas' },
    { value: FinalidadeCategoria.Despesa.toString(), label: 'Despesa' },
    { value: FinalidadeCategoria.Receita.toString(), label: 'Receita' },
    { value: FinalidadeCategoria.Ambas.toString(), label: 'Ambas' },
  ];

  return (
    <Container>
      <PageHeader
        title=""
        subtitle="Gerencie as categorias de transações"
        onAction={() => navigate('/categorias/novo')}
        actionLabel="Nova Categoria"
      />

      <div className="mb-6 space-y-4">
        <div className="flex gap-4">
          <div className="flex-1">
            <Search
              placeholder="Buscar por descrição..."
              onSearch={handleSearch}
              value={searchTerm}
            />
          </div>
          <div className="w-48">
            <Select
              value={finalidade !== null ? finalidade.toString() : ''}
              onChange={(e) => handleFinalidadeChange(e.target.value)}
              options={finalidadeOptions}
            />
          </div>
        </div>
      </div>

      {loading && categorias.length === 0 ? (
        <Loading message="Carregando categorias..." />
      ) : error && categorias.length === 0 ? (
        <ErrorMessage message={error} onRetry={() => loadCategorias()} />
      ) : categorias.length === 0 ? (
        <EmptyState
          message={searchTerm ? 'Nenhuma categoria encontrada com esse termo de busca.' : 'Nenhuma categoria cadastrada ainda.'}
          action={{
            label: 'Cadastrar Primeira Categoria',
            onClick: () => navigate('/categorias/novo'),
          }}
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
                <Th>Descrição</Th>
                <Th align="center">Finalidade</Th>
                <Th align="right">Ações</Th>
              </Tr>
            </Thead>
            <Tbody>
              {categorias.map((categoria, index) => (
                <Tr key={categoria.id} index={index}>
                  <Td>{categoria.descricao}</Td>
                  <Td align="center">
                    <span className={getFinalidadeColorClass(categoria.finalidade)}>
                      {formatarFinalidade(categoria.finalidade)}
                    </span>
                  </Td>
                  <Td align="right">
                    <ActionButtons
                      onEdit={() => navigate(`/categorias/editar/${categoria.id}`)}
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
              totalItems={pagination.totalItems}
              pageSize={take}
              onPrevious={() => handlePageChange(pagination.currentPage - 1)}
              onNext={() => handlePageChange(pagination.currentPage + 1)}
              disabledPrevious={!pagination.hasPreviousPage || loading}
              disabledNext={!pagination.hasNextPage || loading}
            />
          </div>
        )}

        </>
      )}
    </Container>
  );
}
