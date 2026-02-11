export const APP_NAME = 'Abipesca Admin';
export const APP_DESCRIPTION = 'Sistema administrativo unificado';

export const ROUTES = {
  // Geral
  HOME: '/',
  LOGIN: '/login',

  // NAuth
  REGISTER: '/register',
  FORGOT_PASSWORD: '/forgot-password',
  RESET_PASSWORD: '/reset-password',
  DASHBOARD: '/dashboard',
  PROFILE: '/profile',
  CHANGE_PASSWORD: '/change-password',
  SEARCH_USERS: '/users',
  ROLES: '/roles',
  USER_EDIT: '/user-edit',

  // NNews
  ARTICLES: '/articles',
  ARTICLES_NEW: '/articles/new',
  ARTICLES_EDIT: (id: string) => `/articles/${id}`,
  ARTICLES_AI: '/articles/ai',
  CATEGORIES: '/categories',
  TAGS: '/tags',

  // Bazzuca
  CLIENTS: '/clients',
  CLIENT_NETWORKS: (clientId: string) => `/clients/${clientId}/networks`,
  POSTS: '/posts',
  POSTS_CREATE: '/posts/create',
  POSTS_EDIT: (postId: string) => `/posts/${postId}/edit`,
  POSTS_VIEW: (postId: string) => `/posts/${postId}/view`,
  CALENDAR: '/calendar',
} as const;

export const EXTERNAL_LINKS = {
  TERMS: '/terms',
  PRIVACY: '/privacy',
  DOCS: 'https://github.com/emaginebr',
} as const;
