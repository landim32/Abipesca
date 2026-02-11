import { Link } from 'react-router-dom';
import { useAuth } from 'nauth-react';
import { ROUTES, APP_NAME } from '../lib/constants';
import {
  Shield,
  Lock,
  Users,
  Newspaper,
  Share2,
  CheckCircle,
  ArrowRight,
} from 'lucide-react';

export function HomePage() {
  const { isAuthenticated } = useAuth();

  const features = [
    {
      icon: <Lock className="w-6 h-6" />,
      title: 'Authentication',
      description:
        'Complete authentication system with login, register, password recovery, role management, and user administration.',
      color: 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
    },
    {
      icon: <Newspaper className="w-6 h-6" />,
      title: 'News Management',
      description:
        'Full-featured news CMS with articles, categories, tags, and AI-assisted content creation.',
      color: 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400',
    },
    {
      icon: <Share2 className="w-6 h-6" />,
      title: 'Social Media Management',
      description:
        'Manage clients, social networks, schedule posts, and view content calendars across platforms.',
      color: 'bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400',
    },
    {
      icon: <Users className="w-6 h-6" />,
      title: 'User & Role Management',
      description:
        'Search users, manage roles and permissions, edit user profiles, and control access across modules.',
      color: 'bg-orange-100 dark:bg-orange-900/30 text-orange-600 dark:text-orange-400',
    },
    {
      icon: <Shield className="w-6 h-6" />,
      title: 'Protected Routes',
      description:
        'Secure route protection with automatic redirects and session management.',
      color: 'bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400',
    },
    {
      icon: <CheckCircle className="w-6 h-6" />,
      title: 'Unified Dashboard',
      description:
        'Single admin interface combining all modules with a consistent experience and dark mode support.',
      color: 'bg-teal-100 dark:bg-teal-900/30 text-teal-600 dark:text-teal-400',
    },
  ];

  return (
    <div className="max-w-6xl mx-auto">
      {/* Hero Section */}
      <div className="text-center py-20">
        <div className="flex justify-center mb-6">
          <div className="w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg">
            <Shield className="w-12 h-12 text-white" />
          </div>
        </div>
        <h1 className="text-5xl font-bold mb-4 bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
          {APP_NAME}
        </h1>
        <p className="text-xl text-gray-600 dark:text-gray-400 mb-8 max-w-2xl mx-auto">
          Unified administration platform for authentication, news management, and social media operations.
        </p>
        <div className="flex gap-4 justify-center">
          {isAuthenticated ? (
            <Link
              to={ROUTES.DASHBOARD}
              className="inline-flex items-center gap-2 px-8 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
            >
              Go to Dashboard
              <ArrowRight className="w-4 h-4" />
            </Link>
          ) : (
            <>
              <Link
                to={ROUTES.REGISTER}
                className="inline-flex items-center gap-2 px-8 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
              >
                Get Started
                <ArrowRight className="w-4 h-4" />
              </Link>
              <Link
                to={ROUTES.LOGIN}
                className="px-8 py-3 bg-white dark:bg-gray-800 text-gray-900 dark:text-white border border-gray-300 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors font-medium"
              >
                Sign In
              </Link>
            </>
          )}
        </div>
      </div>

      {/* Features Grid */}
      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6 mb-20">
        {features.map((feature, index) => (
          <div
            key={index}
            className="p-6 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow"
          >
            <div
              className={`w-12 h-12 ${feature.color} rounded-lg flex items-center justify-center mb-4`}
            >
              {feature.icon}
            </div>
            <h3 className="text-lg font-semibold mb-2 dark:text-white">
              {feature.title}
            </h3>
            <p className="text-gray-600 dark:text-gray-400">
              {feature.description}
            </p>
          </div>
        ))}
      </div>

      {/* CTA Section */}
      <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl p-12 text-white text-center mb-12">
        <h2 className="text-3xl font-bold mb-6">
          Everything You Need in One Place
        </h2>
        <div className="grid md:grid-cols-3 gap-6 mb-8">
          <div className="flex items-center gap-2 justify-center">
            <CheckCircle className="w-5 h-5" />
            <span>TypeScript Support</span>
          </div>
          <div className="flex items-center gap-2 justify-center">
            <CheckCircle className="w-5 h-5" />
            <span>Responsive Design</span>
          </div>
          <div className="flex items-center gap-2 justify-center">
            <CheckCircle className="w-5 h-5" />
            <span>Dark Mode</span>
          </div>
        </div>
        {!isAuthenticated && (
          <Link
            to={ROUTES.REGISTER}
            className="inline-block px-8 py-3 bg-white text-blue-600 rounded-lg hover:bg-gray-100 transition-colors font-medium"
          >
            Create Your Account
          </Link>
        )}
      </div>
    </div>
  );
}
