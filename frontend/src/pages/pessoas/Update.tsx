import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { pessoasApi } from '../../services/api';
import { Container, Box, PageHeader, Button, Loading, ErrorMessage } from '../../components';
import PessoaForm from './PessoaForm';
import { swal } from '../../utils/swal';
import { getFieldError } from '../../helpers/validation';

// Página de edição de pessoa
export default function PessoasUpdate() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    dataNascimento: '',
  });
  const [errors, setErrors] = useState<{ nome?: string; dataNascimento?: string }>({});

  // Carrega dados da pessoa ao montar o componente
  useEffect(() => {
    const loadPessoa = async () => {
      if (!id) {
        setError('ID não fornecido');
        setLoadingData(false);
        return;
      }

      try {
        setLoadingData(true);
        setError(null);
        const result = await pessoasApi.getById(parseInt(id, 10));

        if (result.success && result.data) {
          setFormData({
            nome: result.data.nome || '',
            dataNascimento: result.data.dataNascimento ? new Date(result.data.dataNascimento).toISOString().split('T')[0] : '',
          });
        } else {
          setError(result.errors.join(', ') || 'Erro ao carregar pessoa');
        }
      } catch (err) {
        setError('Erro ao conectar com a API');
        console.error(err);
      } finally {
        setLoadingData(false);
      }
    };

    loadPessoa();
  }, [id]);

  // Handler para mudanças nos campos
  const handleNomeChange = (value: string) => {
    setFormData((prev) => ({ ...prev, nome: value }));
    if (errors.nome) {
      setErrors((prev) => ({ ...prev, nome: undefined }));
    }
  };

  const handleDataNascimentoChange = (value: string) => {
    setFormData((prev) => ({ ...prev, dataNascimento: value }));
    if (errors.dataNascimento) {
      setErrors((prev) => ({ ...prev, dataNascimento: undefined }));
    }
  };

  // Handler para submit do formulário
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({}); // Limpa erros anteriores

    if (!id) return;

    setLoading(true);

    try {
      const result = await pessoasApi.update(parseInt(id, 10), {
        nome: formData.nome.trim(),
        dataNascimento: formData.dataNascimento,
      });

      if (result.success) {
        swal.successToast('Pessoa atualizada com sucesso!');
        // Redireciona para a listagem após sucesso
        navigate('/pessoas');
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao atualizar pessoa');
      }
    } catch (err: any) {
      // Trata erros de validação do ASP.NET Core (status 400 com formato padrão)
      if (err.response?.status === 400 && err.response?.data?.errors) {
        const validationError = err.response.data;
        const fieldErrors: { nome?: string; dataNascimento?: string } = {};

        const nomeError = getFieldError(validationError, 'nome');
        const dataNascimentoError = getFieldError(validationError, 'dataNascimento');

        if (nomeError) fieldErrors.nome = nomeError;
        if (dataNascimentoError) fieldErrors.dataNascimento = dataNascimentoError;

        setErrors(fieldErrors);

        // Se houver erros de validação mas nenhum mapeado para os campos conhecidos, mostra toast
        if (!nomeError && !dataNascimentoError) {
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
    navigate('/pessoas');
  };

  if (loadingData) {
    return (
      <Container>
        <Loading message="Carregando pessoa..." />
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <ErrorMessage message={error} onRetry={() => navigate('/pessoas')} />
      </Container>
    );
  }

  return (
    <Container>
      <PageHeader
        title="Editar Pessoa"
        subtitle="Altere os dados da pessoa"
      />

      <Box className="max-w-2xl border-0 shadow-none">
        {loading && (
          <div className="absolute inset-0 flex items-center justify-center bg-white/50 backdrop-blur-sm dark:bg-slate-900/50 z-10">
            <Loading message="Salvando..." />
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <PessoaForm
            nome={formData.nome}
            dataNascimento={formData.dataNascimento}
            errors={errors}
            onNomeChange={handleNomeChange}
            onDataNascimentoChange={handleDataNascimentoChange}
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

