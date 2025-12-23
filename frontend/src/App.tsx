import { useMemo } from 'react';
import {
  Navigate,
  Route,
  Routes,
  useLocation,
} from 'react-router-dom';
import Sidebar from './components/Sidebar';
import PessoasList from './pages/pessoas/List';
import PessoasCreate from './pages/pessoas/Create';
import PessoasUpdate from './pages/pessoas/Update';
import TransacoesList from './pages/transacoes/List';
import TransacoesCreate from './pages/transacoes/Create';
import TransacoesUpdate from './pages/transacoes/Update';
import CategoriasList from './pages/categorias/List';
import CategoriasCreate from './pages/categorias/Create';
import CategoriasUpdate from './pages/categorias/Update';

function App() {
  const location = useLocation();

  const path = location.pathname;
  // Título dinâmico baseado na rota atual
  const title = useMemo(() => {
    if (path === '/pessoas/novo') return 'Cadastro de Pessoa';
    if (path.startsWith('/pessoas/editar')) return 'Edição de Pessoa';
    if (path.startsWith('/pessoas')) return 'Listagem de pessoas';
    if (path === '/categorias/novo') return 'Cadastro de Categoria';
    if (path.startsWith('/categorias/editar')) return 'Edição de Categoria';
    if (path.startsWith('/categorias')) return 'Categorias';
    if (path === '/transacoes/novo') return 'Cadastro de Transação';
    if (path.startsWith('/transacoes/editar')) return 'Edição de Transação';
    if (path.startsWith('/transacoes')) return 'Transações';
    return 'Visão geral';
  }, [path]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 text-slate-900 transition-colors dark:bg-slate-950 dark:text-slate-100 dark:[background-image:none]">
      <Sidebar />

      <div className="flex flex-1 flex-col md:ml-72">
          <header className="sticky top-0 z-10 flex items-center justify-between border-b border-[#0000bf]/20 bg-[#0000bf] backdrop-blur-md shadow-sm px-6 py-4 dark:border-slate-800 dark:bg-slate-900 dark:backdrop-blur-none">
            <div>
              <p className="text-sm text-blue-100 dark:text-slate-400">
                Controle de Gastos
              </p>
              <h1 className="text-2xl font-semibold text-white dark:text-slate-100">{title}</h1>
            </div>
          </header>

          <main className="flex-1 p-6">
            <section className="rounded-2xl border border-slate-200/60 bg-white/90 p-6 shadow-md backdrop-blur-sm dark:border-slate-800 dark:bg-slate-900 dark:shadow-none dark:backdrop-blur-none">
              {/* Rotas da aplicação */}
              <Routes>
                <Route path="/" element={<Navigate to="/pessoas" replace />} />
                <Route path="/pessoas" element={<PessoasList />} />
                <Route path="/pessoas/novo" element={<PessoasCreate />} />
                <Route path="/pessoas/editar/:id" element={<PessoasUpdate />} />
                <Route path="/transacoes" element={<TransacoesList />} />
                <Route path="/transacoes/novo" element={<TransacoesCreate />} />
                <Route path="/transacoes/editar/:id" element={<TransacoesUpdate />} />
                <Route path="/categorias" element={<CategoriasList />} />
                <Route path="/categorias/novo" element={<CategoriasCreate />} />
                <Route path="/categorias/editar/:id" element={<CategoriasUpdate />} />
              </Routes>
            </section>
          </main>
      </div>
    </div>
  );
}

export default App;
