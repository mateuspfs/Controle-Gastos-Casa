import { useMemo } from 'react';
import {
  Navigate,
  Route,
  Routes,
  useLocation,
} from 'react-router-dom';
import Sidebar from './components/Sidebar';
import ThemeToggle from './components/ThemeToggle';
import PessoasList from './pages/pessoas/List';
import TransacoesList from './pages/transacoes/List';
import CategoriasList from './pages/categorias/List';

function App() {
  const location = useLocation();

  const path = location.pathname;
  // Título dinâmico baseado na rota atual
  const title = useMemo(() => {
    if (path.startsWith('/pessoas')) return 'Listagem de pessoas';
    if (path.startsWith('/transacoes')) return 'Transações';
    if (path.startsWith('/categorias')) return 'Categorias';
    return 'Visão geral';
  }, [path]);

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900 transition-colors dark:bg-slate-950 dark:text-slate-100">
      <div className="flex min-h-screen">
        <Sidebar />

        <div className="flex flex-1 flex-col">
          <header className="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white/70 px-6 py-4 backdrop-blur dark:border-slate-800 dark:bg-slate-900/70">
            <div>
              <p className="text-sm text-slate-500 dark:text-slate-400">
                Controle de Gastos
              </p>
              <h1 className="text-2xl font-semibold">{title}</h1>
            </div>
            <ThemeToggle />
          </header>

          <main className="flex-1 p-6">
            <section className="rounded-2xl border border-dashed border-slate-200 bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900">
              {/* Rotas da aplicação */}
              <Routes>
                <Route path="/" element={<Navigate to="/pessoas" replace />} />
                <Route path="/pessoas" element={<PessoasList />} />
                <Route path="/transacoes" element={<TransacoesList />} />
                <Route path="/categorias" element={<CategoriasList />} />
              </Routes>
            </section>
          </main>
        </div>
      </div>
    </div>
  );
}

export default App;
