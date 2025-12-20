import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoriasApi } from '../../services/api';
import { Container, Box, PageHeader, Button, Loading } from '../../components';
import CategoriaForm from './CategoriaForm';
import { swal } from '../../utils/swal';
import { FinalidadeCategoria } from '../../types/api';
import { getFieldError } from '../../helpers/validation';

// Página de cadastro de categoria
export default function CategoriasCreate() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    descricao: '',
    finalidade: FinalidadeCategoria.Despesa,
  });
  const [errors, setErrors] = useState<{ descricao?: string; finalidade?: string }>({});

  // Handler para mudanças nos campos
  const handleDescricaoChange = (value: string) => {
    setFormData((prev) => ({ ...prev, descricao: value }));
    if (errors.descricao) {
      setErrors((prev) => ({ ...prev, descricao: undefined }));
    }
  };

  const handleFinalidadeChange = (value: FinalidadeCategoria) => {
    setFormData((prev) => ({ ...prev, finalidade: value }));
    if (errors.finalidade) {
      setErrors((prev) => ({ ...prev, finalidade: undefined }));
    }
  };

  // Handler para submit do formulário
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});
    setLoading(true);

    try {
      const result = await categoriasApi.create({
        descricao: formData.descricao.trim(),
        finalidade: formData.finalidade,
      });

      if (result.success) {
        swal.successToast('Categoria cadastrada com sucesso!');
        // Redireciona para a listagem após sucesso
        navigate('/categorias');
      } else {
        swal.errorToast(result.errors.join(', ') || 'Erro ao cadastrar categoria');
      }
    } catch (err: any) {
      // Trata erros de validação do ASP.NET Core (status 400 com formato padrão)
      if (err.response?.status === 400 && err.response?.data?.errors) {
        const validationError = err.response.data;
        const fieldErrors: { descricao?: string; finalidade?: string } = {};

        const descricaoError = getFieldError(validationError, 'descricao');
        const finalidadeError = getFieldError(validationError, 'finalidade');

        if (descricaoError) fieldErrors.descricao = descricaoError;
        if (finalidadeError) fieldErrors.finalidade = finalidadeError;

        setErrors(fieldErrors);

        // Se houver erros de validação mas nenhum mapeado para os campos conhecidos, mostra toast
        if (!descricaoError && !finalidadeError) {
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
    navigate('/categorias');
  };

  return (
    <Container>
      <PageHeader
        title="Nova Categoria"
        subtitle="Preencha os dados para cadastrar uma nova categoria"
      />

      <Box className="max-w-2xl border-0 shadow-none">
        {loading && (
          <div className="absolute inset-0 flex items-center justify-center bg-white/50 backdrop-blur-sm dark:bg-slate-900/50 z-10">
            <Loading message="Salvando..." />
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <CategoriaForm
            descricao={formData.descricao}
            finalidade={formData.finalidade}
            errors={errors}
            onDescricaoChange={handleDescricaoChange}
            onFinalidadeChange={handleFinalidadeChange}
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

