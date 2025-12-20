import { type ReactNode } from 'react';

// Box com borda, sombra e estilização padronizada
export default function Box({ children, className = '', variant = 'default' }: { children: ReactNode; className?: string; variant?: 'default' | 'dashed' }) {
  const baseClasses = 'rounded-xl border bg-white/95 backdrop-blur-sm dark:bg-slate-900';
  const variantClasses = {
    default: 'border-slate-200/80 shadow-lg dark:border-slate-800 dark:shadow-none',
    dashed: 'border-dashed border-slate-200/60 dark:border-slate-800',
  };

  return (
    <div className={`relative ${baseClasses} ${variantClasses[variant]} ${className}`}>
      {children}
    </div>
  );
}

