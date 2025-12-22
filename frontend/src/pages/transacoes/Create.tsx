import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { transacoesApi, pessoasApi, categoriasApi } from '../../services/api';
import { Container, Box, PageHeader, Button, Loading } from '../../components';
import TransacaoForm from './TransacaoForm';
import { swal } from '../../utils/swal';
import { TipoTransacao } from '../../types/api';
import type { PessoaDto, CategoriaDto } from '../../types/api';
import { getFieldError } from '../../helpers/validation';
import { desformatarMoeda } from '../../helpers/masks';

// Página de cadastro de transação
export default function TransacoesCreate() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
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

  // Carrega pessoas e categorias ao montar o componente
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
    setLoading(true);

    try {
      // Validação antes de enviar
      const valorDesformatado = desformatarMoeda(formData.valor);
      const valorNumero = parseFloat(valorDesformatado);
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

      const result = await transacoesApi.create({
        descricao: formData.descricao.trim(),
        valor: valorNumero,
        tipo: formData.tipo,
        dataTransacao: formData.dataTransacao,
        categoriaId: formData.categoriaId,
        pessoaId: formData.pessoaId,
      });

      if (result.success) {
        swal.successToast('Transação cadastrada com sucesso!');
        navigate('/transacoes');
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao cadastrar transação');
      }
    } catch (err: any) {
      // Trata erros de validação do ASP.NET Core (status 400 com formato padrão)
      if (err.response?.status === 400 && err.response?.data?.errors) {
        const validationError = err.response.data;
        const fieldErrors: typeof errors = {};

        const descricaoError = getFieldError(validationError, 'descricao');
        const valorError = getFieldError(validationError, 'valor');
        const tipoError = getFieldError(validationError, 'tipo');
        const dataTransacaoError = getFieldError(validationError, 'dataTransacao');
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

  // Voltar para listagem
  const handleCancel = () => {
    navigate('/transacoes');
  };

  if (loadingData) {
    return (
      <Container>
        <Loading message="Carregando dados..." />
      </Container>
    );
  }

  return (
    <Container>
      <PageHeader
        title="Nova Transação"
        subtitle="Preencha os dados para cadastrar uma nova transação"
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

