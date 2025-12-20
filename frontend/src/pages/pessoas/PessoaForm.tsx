import { Input } from '../../components/Form';
import { calcularIdade, formatarIdade } from '../../helpers/dateHelper';

interface PessoaFormProps {
  nome: string;
  dataNascimento: string;
  errors?: { nome?: string; dataNascimento?: string };
  onNomeChange: (value: string) => void;
  onDataNascimentoChange: (value: string) => void;
  disabled?: boolean;
}

// Componente reutilizável de formulário de pessoa
export default function PessoaForm({ 
  nome, 
  dataNascimento, 
  errors = {}, 
  onNomeChange, 
  onDataNascimentoChange, 
  disabled = false 
}: PessoaFormProps) {
  // Calcula idade automaticamente quando data de nascimento muda (apenas para preview no formulário)
  const idadeCalculada = calcularIdade(dataNascimento);

  return (
    <div className="space-y-6">
      <Input
        label="Nome"
        type="text"
        value={nome}
        onChange={(e) => onNomeChange(e.target.value)}
        placeholder="Digite o nome completo"
        error={errors.nome}
        disabled={disabled}
        maxLength={150}
      />

      <div className="space-y-2">
        <Input
          label="Data de Nascimento"
          type="date"
          value={dataNascimento}
          onChange={(e) => onDataNascimentoChange(e.target.value)}
          error={errors.dataNascimento}
          disabled={disabled}
          max={new Date().toISOString().split('T')[0]}
        />
        
        {/* Exibe idade calculada quando há data de nascimento */}
        {idadeCalculada !== null && (
          <div className="rounded-lg border border-slate-200 bg-slate-50 px-4 py-2.5 dark:border-slate-700 dark:bg-slate-800">
            <p className="text-sm text-slate-600 dark:text-slate-400">
              <span className="font-medium">Idade:</span> {formatarIdade(idadeCalculada)}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
