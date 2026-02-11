import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { NAuthProvider, useAuth } from 'nauth-react';
import { NNewsProvider } from 'nnews-react';
import { BazzucaProvider } from 'bazzuca-react';
import { Toaster } from 'sonner';
import { useMemo } from 'react';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/auth/RegisterPage';
import { ForgotPasswordPage } from './pages/auth/ForgotPasswordPage';
import { ResetPasswordPage } from './pages/auth/ResetPasswordPage';
import { DashboardPage } from './pages/auth/DashboardPage';
import { ProfilePage } from './pages/auth/ProfilePage';
import { ChangePasswordPage } from './pages/auth/ChangePasswordPage';
import SearchUsersPage from './pages/auth/SearchUsersPage';
import RolesPage from './pages/auth/RolesPage';
import UserEditPage from './pages/auth/UserEditPage';
import { ArticleListPage } from './pages/news/ArticleListPage';
import { ArticleEditPage } from './pages/news/ArticleEditPage';
import { CategoryPage } from './pages/news/CategoryPage';
import { TagsPage } from './pages/news/TagsPage';
import { ClientsPage } from './pages/media/ClientsPage';
import { ClientNetworksPage } from './pages/media/ClientNetworksPage';
import { PostsPage } from './pages/media/PostsPage';
import { PostEditPage } from './pages/media/PostEditPage';
import { PostViewPage } from './pages/media/PostViewPage';
import { CalendarPage } from './pages/media/CalendarPage';
import { ROUTES } from './lib/constants';

function AppContent() {
  const { user } = useAuth();

  const authHeaders = useMemo(() => {
    const token = user?.token;
    if (token) {
      return { Authorization: `Bearer ${token}` };
    }
    return {};
  }, [user?.token]);

  return (
    <NNewsProvider
      config={{
        apiUrl: import.meta.env.VITE_NNEWS_API_URL || '/api/nnews',
        headers: authHeaders,
      }}
    >
      <BazzucaProvider
        config={{
          apiUrl: import.meta.env.VITE_BAZZUCA_API_URL || '/api/bazzuca',
          headers: authHeaders,
        }}
      >
        <Toaster position="bottom-right" richColors />
        <Routes>
          <Route element={<Layout />}>
            {/* Public Routes */}
            <Route path={ROUTES.HOME} element={<HomePage />} />
            <Route path={ROUTES.LOGIN} element={<LoginPage />} />
            <Route path={ROUTES.REGISTER} element={<RegisterPage />} />
            <Route path={ROUTES.FORGOT_PASSWORD} element={<ForgotPasswordPage />} />
            <Route path={`${ROUTES.RESET_PASSWORD}/:hash`} element={<ResetPasswordPage />} />

            {/* Auth Protected Routes */}
            <Route path={ROUTES.DASHBOARD} element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
            <Route path={ROUTES.PROFILE} element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
            <Route path={ROUTES.CHANGE_PASSWORD} element={<ProtectedRoute><ChangePasswordPage /></ProtectedRoute>} />
            <Route path={ROUTES.SEARCH_USERS} element={<ProtectedRoute><SearchUsersPage /></ProtectedRoute>} />
            <Route path={ROUTES.ROLES} element={<ProtectedRoute><RolesPage /></ProtectedRoute>} />
            <Route path={ROUTES.USER_EDIT} element={<ProtectedRoute><UserEditPage /></ProtectedRoute>} />

            {/* News Protected Routes */}
            <Route path={ROUTES.ARTICLES} element={<ProtectedRoute><ArticleListPage /></ProtectedRoute>} />
            <Route path={ROUTES.ARTICLES_NEW} element={<ProtectedRoute><ArticleEditPage /></ProtectedRoute>} />
            <Route path="/articles/:id" element={<ProtectedRoute><ArticleEditPage /></ProtectedRoute>} />
            <Route path={ROUTES.CATEGORIES} element={<ProtectedRoute><CategoryPage /></ProtectedRoute>} />
            <Route path={ROUTES.TAGS} element={<ProtectedRoute><TagsPage /></ProtectedRoute>} />

            {/* Bazzuca Protected Routes */}
            <Route path={ROUTES.CLIENTS} element={<ProtectedRoute><ClientsPage /></ProtectedRoute>} />
            <Route path="/clients/:clientId/networks" element={<ProtectedRoute><ClientNetworksPage /></ProtectedRoute>} />
            <Route path={ROUTES.POSTS} element={<ProtectedRoute><PostsPage /></ProtectedRoute>} />
            <Route path={ROUTES.POSTS_CREATE} element={<ProtectedRoute><PostEditPage /></ProtectedRoute>} />
            <Route path="/posts/:postId/edit" element={<ProtectedRoute><PostEditPage /></ProtectedRoute>} />
            <Route path="/posts/:postId/view" element={<ProtectedRoute><PostViewPage /></ProtectedRoute>} />
            <Route path={ROUTES.CALENDAR} element={<ProtectedRoute><CalendarPage /></ProtectedRoute>} />

            {/* Fallback */}
            <Route path="*" element={<Navigate to={ROUTES.HOME} replace />} />
          </Route>
        </Routes>
      </BazzucaProvider>
    </NNewsProvider>
  );
}

function App() {
  return (
    <BrowserRouter>
      <NAuthProvider
        config={{
          apiUrl: import.meta.env.VITE_API_URL || '/api/nauth',
          enableFingerprinting: true,
          redirectOnUnauthorized: ROUTES.LOGIN,
          onAuthChange: (user) => {
            console.log('Auth state changed:', user);
          },
        }}
      >
        <AppContent />
      </NAuthProvider>
    </BrowserRouter>
  );
}

export default App;
