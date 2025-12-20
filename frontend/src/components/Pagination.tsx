import Button from './Button';

// Componente de paginação padronizado
interface PaginationProps {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  onPrevious: () => void;
  onNext: () => void;
  disabledPrevious?: boolean;
  disabledNext?: boolean;
}

export default function Pagination({
  currentPage,
  pageSize,
  totalItems,
  onPrevious,
  onNext,
  disabledPrevious = false,
  disabledNext = false,
}: PaginationProps) {
  const totalPages = totalItems > 0 ? Math.ceil(totalItems / pageSize) : 0;

  return (
    <div className="flex items-center justify-between">
      <Button
        variant="outline"
        size="sm"
        onClick={onPrevious}
        disabled={disabledPrevious}
      >
        Anterior
      </Button>
      <div className="text-center">
        <span className="text-sm font-medium text-slate-700 dark:text-slate-400">
          Página {currentPage}
          {totalPages > 0 && ` de ${totalPages}`}
        </span>
      </div>
      <Button
        variant="outline"
        size="sm"
        onClick={onNext}
        disabled={disabledNext}
      >
        Próxima
      </Button>
    </div>
  );
}

