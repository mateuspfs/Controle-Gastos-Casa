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
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 text-slate-900 transition-colors dark:bg-slate-950 dark:text-slate-100 dark:[background-image:none]">
      <div className="flex min-h-screen">
        <Sidebar />

        <div className="flex flex-1 flex-col">
          <header className="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200/80 bg-white/80 backdrop-blur-md shadow-sm px-6 py-4 dark:border-slate-800 dark:bg-slate-900 dark:backdrop-blur-none">
            <div>
              <p className="text-sm text-slate-600 dark:text-slate-400">
                Controle de Gastos
              </p>
              <h1 className="text-2xl font-semibold text-slate-800 dark:text-slate-100">{title}</h1>
            </div>
            <ThemeToggle />
          </header>

          <main className="flex-1 p-6">
            <section className="rounded-2xl border border-slate-200/60 bg-white/90 p-6 shadow-md backdrop-blur-sm dark:border-slate-800 dark:bg-slate-900 dark:shadow-none dark:backdrop-blur-none">
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
