import { Input, Select, CurrencyInput } from '../../components/Form';
import { TipoTransacao } from '../../types/api';
import type { PessoaDto, CategoriaDto } from '../../types/api';

interface TransacaoFormProps {
  descricao: string;
  valor: string;
  tipo: TipoTransacao;
  dataTransacao: string;
  categoriaId: number;
  pessoaId: number;
  pessoas: PessoaDto[];
  categorias: CategoriaDto[];
  errors?: {
    descricao?: string;
    valor?: string;
    tipo?: string;
    dataTransacao?: string;
    categoriaId?: string;
    pessoaId?: string;
  };
  onDescricaoChange: (value: string) => void;
  onValorChange: (value: string) => void;
  onTipoChange: (value: TipoTransacao) => void;
  onDataTransacaoChange: (value: string) => void;
  onCategoriaIdChange: (value: number) => void;
  onPessoaIdChange: (value: number) => void;
  disabled?: boolean;
}

// Componente reutilizável de formulário de transação
export default function TransacaoForm({
  descricao,
  valor,
  tipo,
  dataTransacao,
  categoriaId,
  pessoaId,
  pessoas,
  categorias,
  errors = {},
  onDescricaoChange,
  onValorChange,
  onTipoChange,
  onDataTransacaoChange,
  onCategoriaIdChange,
  onPessoaIdChange,
  disabled = false,
}: TransacaoFormProps) {
  const tipoOptions = [
    { value: TipoTransacao.Despesa, label: 'Despesa' },
    { value: TipoTransacao.Receita, label: 'Receita' },
  ];

  // Filtra categorias baseado no tipo de transação
  const categoriasFiltradas = categorias.filter((cat) => {
    if (tipo === TipoTransacao.Despesa) {
      return cat.finalidade === 1 || cat.finalidade === 3; // Despesa ou Ambas
    } else {
      return cat.finalidade === 2 || cat.finalidade === 3; // Receita ou Ambas
    }
  });

  const pessoaOptions = pessoas.map((p) => ({
    value: p.id!,
    label: p.nome,
  }));

  const categoriaOptions = categoriasFiltradas.map((c) => ({
    value: c.id!,
    label: c.descricao,
  }));

  return (
    <div className="space-y-6">
      <Input
        label="Descrição"
        type="text"
        value={descricao}
        onChange={(e) => onDescricaoChange(e.target.value)}
        placeholder="Digite a descrição da transação"
        error={errors.descricao}
        disabled={disabled}
        maxLength={200}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <CurrencyInput
          label="Valor"
          value={valor}
          onChange={onValorChange}
          error={errors.valor}
          disabled={disabled}
          required
        />

        <Select
          label="Tipo"
          value={tipo}
          onChange={(e) => onTipoChange(Number(e.target.value) as TipoTransacao)}
          error={errors.tipo}
          disabled={disabled}
          options={tipoOptions}
        />
      </div>

      <Input
        label="Data da Transação"
        type="date"
        value={dataTransacao}
        onChange={(e) => onDataTransacaoChange(e.target.value)}
        error={errors.dataTransacao}
        disabled={disabled}
        max={new Date().toISOString().split('T')[0]}
      />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Select
          label="Pessoa"
          value={pessoaId || ''}
          onChange={(e) => onPessoaIdChange(Number(e.target.value))}
          error={errors.pessoaId}
          disabled={disabled}
          options={[
            { value: '', label: 'Selecione uma pessoa' },
            ...pessoaOptions,
          ]}
        />

        <Select
          label="Categoria"
          value={categoriaId || ''}
          onChange={(e) => onCategoriaIdChange(Number(e.target.value))}
          error={errors.categoriaId}
          disabled={disabled}
          options={[
            { value: '', label: 'Selecione uma categoria' },
            ...categoriaOptions,
          ]}
        />
      </div>
    </div>
  );
}

