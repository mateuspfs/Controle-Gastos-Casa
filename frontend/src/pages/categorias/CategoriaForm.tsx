import { Input, Select } from '../../components/Form';
import { FinalidadeCategoria } from '../../types/api';

interface CategoriaFormProps {
  descricao: string;
  finalidade: FinalidadeCategoria;
  errors?: { descricao?: string; finalidade?: string };
  onDescricaoChange: (value: string) => void;
  onFinalidadeChange: (value: FinalidadeCategoria) => void;
  disabled?: boolean;
}

// Componente reutilizável de formulário de categoria
export default function CategoriaForm({ 
  descricao, 
  finalidade, 
  errors = {}, 
  onDescricaoChange, 
  onFinalidadeChange, 
  disabled = false 
}: CategoriaFormProps) {
  const finalidadeOptions = [
    { value: FinalidadeCategoria.Despesa, label: 'Despesa' },
    { value: FinalidadeCategoria.Receita, label: 'Receita' },
    { value: FinalidadeCategoria.Ambas, label: 'Ambas' },
  ];

  return (
    <div className="space-y-6">
      <Input
        label="Descrição"
        type="text"
        value={descricao}
        onChange={(e) => onDescricaoChange(e.target.value)}
        placeholder="Digite a descrição da categoria"
        error={errors.descricao}
        disabled={disabled}
        maxLength={150}
      />

      <Select
        label="Finalidade"
        value={finalidade}
        onChange={(e) => onFinalidadeChange(Number(e.target.value) as FinalidadeCategoria)}
        error={errors.finalidade}
        disabled={disabled}
        options={finalidadeOptions}
      />
    </div>
  );
}

