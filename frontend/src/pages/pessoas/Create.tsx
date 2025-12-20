import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { pessoasApi } from '../../services/api';
import { Container, Box, PageHeader, Button, Loading } from '../../components';
import PessoaForm from './PessoaForm';
import { swal } from '../../utils/swal';

// Função auxiliar para buscar erro de campo
const getFieldError = (validationError: any, fieldName: string): string | undefined => {
  const keys = Object.keys(validationError.errors);
  const key = keys.find(k => k.toLowerCase() === fieldName.toLowerCase());
  if (key) {
    const errors = validationError.errors[key];
    return errors && errors.length > 0 ? errors[0] : undefined;
  }
  return undefined;
};

// Página de cadastro de pessoa
export default function PessoasCreate() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    nome: '',
    dataNascimento: '',
  });
  const [errors, setErrors] = useState<{ nome?: string; dataNascimento?: string }>({});

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
    setErrors({});
    setLoading(true);

    try {
      const result = await pessoasApi.create({
        nome: formData.nome.trim(),
        dataNascimento: formData.dataNascimento,
      });

      if (result.success) {
        swal.successToast('Pessoa cadastrada com sucesso!');
        // Redireciona para a listagem após sucesso
        navigate('/pessoas');
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao cadastrar pessoa');
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

  // Voltar para listagem
  const handleCancel = () => {
    navigate('/pessoas');
  };

  return (
    <Container>
      <PageHeader
        title="Nova Pessoa"
        subtitle="Preencha os dados para cadastrar uma nova pessoa"
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

