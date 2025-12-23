import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  transacoesApi,
  pessoasApi,
  categoriasApi,
} from '../../services/api';
import {
  Container,
  Box,
  PageHeader,
  Button,
  Loading,
  ErrorMessage,
} from '../../components';
import TransacaoForm from './TransacaoForm';
import { swal } from '../../utils/swal';
import { TipoTransacao } from '../../types/api';
import type { PessoaDto, CategoriaDto } from '../../types/api';
import { getFieldError } from '../../helpers/validation';

// Página de edição de transação
export default function TransacoesUpdate() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pessoas, setPessoas] = useState<PessoaDto[]>([]);
  const [categorias, setCategorias] = useState<CategoriaDto[]>([]);
  const [formData, setFormData] = useState<{
    descricao: string;
    valor: string;
    tipo: TipoTransacao;
    dataTransacao: string;
    categoriaId: number;
    pessoaId: number;
  }>({
    descricao: '',
    valor: '',
    tipo: TipoTransacao.Despesa,
    dataTransacao: new Date().toISOString().split('T')[0],
    categoriaId: 0,
    pessoaId: 0,
  });
  const [errors, setErrors] = useState<{
    descricao?: string;
    valor?: string;
    tipo?: string;
    dataTransacao?: string;
    categoriaId?: string;
    pessoaId?: string;
  }>({});

  // Carrega dados da transação, pessoas e categorias ao montar o componente
  useEffect(() => {
    const loadData = async () => {
      if (!id) {
        setError('ID não fornecido');
        setLoadingData(false);
        return;
      }

      try {
        setLoadingData(true);
        setError(null);

        const [transacaoResult, pessoasResult, categoriasResult] =
          await Promise.all([
            transacoesApi.getById(parseInt(id, 10)),
            pessoasApi.getAll(0, 0), // Carrega todas as pessoas (take: 0 = todos)
            categoriasApi.getAll(0, 0), // Carrega todas as categorias (take: 0 = todos)
          ]);

        if (transacaoResult.success && transacaoResult.data) {
          const transacao = transacaoResult.data;
          setFormData({
            descricao: transacao.descricao || '',
            valor: transacao.valor.toString() || '0',
            tipo: transacao.tipo,
            dataTransacao: transacao.dataTransacao
              ? new Date(transacao.dataTransacao).toISOString().split('T')[0]
              : new Date().toISOString().split('T')[0],
            categoriaId: transacao.categoriaId || 0,
            pessoaId: transacao.pessoaId || 0,
          });
        } else {
          setError(
            transacaoResult.errors.join(', ') || 'Erro ao carregar transação'
          );
        }

        if (pessoasResult.success && pessoasResult.data) {
          setPessoas(pessoasResult.data as PessoaDto[]);
        }

        if (categoriasResult.success && categoriasResult.data) {
          setCategorias(categoriasResult.data as CategoriaDto[]);
        }
      } catch (err) {
        setError('Erro ao conectar com a API');
        console.error(err);
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, [id]);

  // Handler para mudanças nos campos
  const handleDescricaoChange = (value: string) => {
    setFormData((prev) => ({ ...prev, descricao: value }));
    if (errors.descricao) {
      setErrors((prev) => ({ ...prev, descricao: undefined }));
    }
  };

  const handleValorChange = (value: string) => {
    setFormData((prev) => ({ ...prev, valor: value }));
    if (errors.valor) {
      setErrors((prev) => ({ ...prev, valor: undefined }));
    }
  };

  const handleTipoChange = (value: TipoTransacao) => {
    setFormData((prev) => ({ ...prev, tipo: value, categoriaId: 0 })); // Reseta categoria ao mudar tipo
    if (errors.tipo) {
      setErrors((prev) => ({ ...prev, tipo: undefined }));
    }
  };

  const handleDataTransacaoChange = (value: string) => {
    setFormData((prev) => ({ ...prev, dataTransacao: value }));
    if (errors.dataTransacao) {
      setErrors((prev) => ({ ...prev, dataTransacao: undefined }));
    }
  };

  const handleCategoriaIdChange = (value: number) => {
    setFormData((prev) => ({ ...prev, categoriaId: value }));
    if (errors.categoriaId) {
      setErrors((prev) => ({ ...prev, categoriaId: undefined }));
    }
  };

  const handlePessoaIdChange = (value: number) => {
    setFormData((prev) => ({ ...prev, pessoaId: value }));
    if (errors.pessoaId) {
      setErrors((prev) => ({ ...prev, pessoaId: undefined }));
    }
  };

  // Handler para submit do formulário
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});

    if (!id) return;

    setLoading(true);

    try {
      // Validação no valor antes de enviar para a api
      const valorNumero = parseFloat(formData.valor);
      if (isNaN(valorNumero) || valorNumero <= 0) {
        setErrors({ valor: 'O valor deve ser maior que zero.' });
        setLoading(false);
        return;
      }

      if (formData.pessoaId === 0) {
        setErrors({ pessoaId: 'Pessoa é obrigatória.' });
        setLoading(false);
        return;
      }

      if (formData.categoriaId === 0) {
        setErrors({ categoriaId: 'Categoria é obrigatória.' });
        setLoading(false);
        return;
      }

      const result = await transacoesApi.update(parseInt(id, 10), {
        descricao: formData.descricao.trim(),
        valor: valorNumero,
        tipo: formData.tipo,
        dataTransacao: formData.dataTransacao,
        categoriaId: formData.categoriaId,
        pessoaId: formData.pessoaId,
      });

      if (result.success) {
        swal.successToast('Transação atualizada com sucesso!');
        navigate('/transacoes');
      } else {
        swal.errorToast(
          result.errors.join(', ') || 'Erro ao atualizar transação'
        );
      }
    } catch (err: any) {
      // Trata erros de validação do ASP.NET Core (status 400 com formato padrão)
      if (err.response?.status === 400 && err.response?.data?.errors) {
        const validationError = err.response.data;
        const fieldErrors: typeof errors = {};

        const descricaoError = getFieldError(validationError, 'descricao');
        const valorError = getFieldError(validationError, 'valor');
        const tipoError = getFieldError(validationError, 'tipo');
        const dataTransacaoError = getFieldError(
          validationError,
          'dataTransacao'
        );
        const categoriaIdError = getFieldError(validationError, 'categoriaId');
        const pessoaIdError = getFieldError(validationError, 'pessoaId');

        if (descricaoError) fieldErrors.descricao = descricaoError;
        if (valorError) fieldErrors.valor = valorError;
        if (tipoError) fieldErrors.tipo = tipoError;
        if (dataTransacaoError) fieldErrors.dataTransacao = dataTransacaoError;
        if (categoriaIdError) fieldErrors.categoriaId = categoriaIdError;
        if (pessoaIdError) fieldErrors.pessoaId = pessoaIdError;

        setErrors(fieldErrors);

        // Se houver erros de validação mas nenhum mapeado para os campos conhecidos, mostra toast
        if (
          !descricaoError &&
          !valorError &&
          !tipoError &&
          !dataTransacaoError &&
          !categoriaIdError &&
          !pessoaIdError
        ) {
          const allErrors = Object.values(validationError.errors).flat();
          if (allErrors.length > 0) {
            swal.errorToast(allErrors.join(', '));
          }
        }
      } else {
        swal.errorToast('Erro ao conectar com a API');
        console.error(err);
      }
    } finally {
      setLoading(false);
    }
  };

  // Handler para cancelar (voltar para listagem)
  const handleCancel = () => {
    navigate('/transacoes');
  };

  if (loadingData) {
    return (
      <Container>
        <Loading message="Carregando transação..." />
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <ErrorMessage message={error} onRetry={() => navigate('/transacoes')} />
      </Container>
    );
  }

  return (
    <Container>
      <PageHeader
        title="Editar Transação"
        subtitle="Altere os dados da transação"
      />

      <Box className="max-w-2xl border-0 shadow-none">
        {loading && (
          <div className="absolute inset-0 flex items-center justify-center bg-white/50 backdrop-blur-sm dark:bg-slate-900/50 z-10">
            <Loading message="Salvando..." />
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <TransacaoForm
            descricao={formData.descricao}
            valor={formData.valor}
            tipo={formData.tipo}
            dataTransacao={formData.dataTransacao}
            categoriaId={formData.categoriaId}
            pessoaId={formData.pessoaId}
            pessoas={pessoas}
            categorias={categorias}
            errors={errors}
            onDescricaoChange={handleDescricaoChange}
            onValorChange={handleValorChange}
            onTipoChange={handleTipoChange}
            onDataTransacaoChange={handleDataTransacaoChange}
            onCategoriaIdChange={handleCategoriaIdChange}
            onPessoaIdChange={handlePessoaIdChange}
            disabled={loading}
          />

          <div className="flex gap-4 pt-4">
            <Button
              type="submit"
              variant="primary"
              disabled={loading}
              className="flex-1"
            >
              Salvar
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={handleCancel}
              disabled={loading}
              className="flex-1"
            >
              Cancelar
            </Button>
          </div>
        </form>
      </Box>
    </Container>
  );
}

