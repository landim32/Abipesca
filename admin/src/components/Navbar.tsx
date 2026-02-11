import { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from 'nauth-react';
import { APP_NAME, ROUTES } from '../lib/constants';
import { UserMenu } from './UserMenu';
import {
  Shield,
  LayoutDashboard,
  Search,
  ShieldCheck,
  Newspaper,
  Tag,
  FolderOpen,
  ChevronDown,
  Users,
  FileText,
  Calendar,
} from 'lucide-react';

export function Navbar() {
  const { isAuthenticated } = useAuth();
  const [newsOpen, setNewsOpen] = useState(false);
  const [bazzucaOpen, setBazzucaOpen] = useState(false);
  const newsRef = useRef<HTMLDivElement>(null);
  const bazzucaRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (newsRef.current && !newsRef.current.contains(event.target as Node)) {
        setNewsOpen(false);
      }
      if (bazzucaRef.current && !bazzucaRef.current.contains(event.target as Node)) {
        setBazzucaOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800 sticky top-0 z-40">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center gap-6">
            <Link to={ROUTES.HOME} className="flex items-center gap-2 text-xl font-bold">
              <Shield className="w-8 h-8 text-blue-600 dark:text-blue-400" />
              <span className="bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
                {APP_NAME}
              </span>
            </Link>

            {isAuthenticated && (
              <>
                <Link
                  to={ROUTES.DASHBOARD}
                  className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  <LayoutDashboard className="w-4 h-4" />
                  Dashboard
                </Link>

                <Link
                  to={ROUTES.SEARCH_USERS}
                  className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  <Search className="w-4 h-4" />
                  Users
                </Link>

                <Link
                  to={ROUTES.ROLES}
                  className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  <ShieldCheck className="w-4 h-4" />
                  Roles
                </Link>

                {/* News Dropdown */}
                <div ref={newsRef} className="relative">
                  <button
                    onClick={() => { setNewsOpen(!newsOpen); setBazzucaOpen(false); }}
                    className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    <Newspaper className="w-4 h-4" />
                    News
                    <ChevronDown className={`w-3 h-3 transition-transform ${newsOpen ? 'rotate-180' : ''}`} />
                  </button>
                  {newsOpen && (
                    <div className="absolute left-0 mt-1 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50">
                      <Link
                        to={ROUTES.ARTICLES}
                        onClick={() => setNewsOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors rounded-t-lg"
                      >
                        <FileText className="w-4 h-4" />
                        Articles
                      </Link>
                      <Link
                        to={ROUTES.CATEGORIES}
                        onClick={() => setNewsOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors"
                      >
                        <FolderOpen className="w-4 h-4" />
                        Categories
                      </Link>
                      <Link
                        to={ROUTES.TAGS}
                        onClick={() => setNewsOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors rounded-b-lg"
                      >
                        <Tag className="w-4 h-4" />
                        Tags
                      </Link>
                    </div>
                  )}
                </div>

                {/* Bazzuca Dropdown */}
                <div ref={bazzucaRef} className="relative">
                  <button
                    onClick={() => { setBazzucaOpen(!bazzucaOpen); setNewsOpen(false); }}
                    className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    <Users className="w-4 h-4" />
                    Bazzuca
                    <ChevronDown className={`w-3 h-3 transition-transform ${bazzucaOpen ? 'rotate-180' : ''}`} />
                  </button>
                  {bazzucaOpen && (
                    <div className="absolute left-0 mt-1 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50">
                      <Link
                        to={ROUTES.CLIENTS}
                        onClick={() => setBazzucaOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors rounded-t-lg"
                      >
                        <Users className="w-4 h-4" />
                        Clients
                      </Link>
                      <Link
                        to={ROUTES.POSTS}
                        onClick={() => setBazzucaOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors"
                      >
                        <FileText className="w-4 h-4" />
                        Posts
                      </Link>
                      <Link
                        to={ROUTES.CALENDAR}
                        onClick={() => setBazzucaOpen(false)}
                        className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-700 dark:text-white transition-colors rounded-b-lg"
                      >
                        <Calendar className="w-4 h-4" />
                        Calendar
                      </Link>
                    </div>
                  )}
                </div>
              </>
            )}
          </div>

          <div className="flex items-center gap-4">
            {isAuthenticated ? (
              <UserMenu />
            ) : (
              <div className="flex items-center gap-2">
                <Link
                  to={ROUTES.LOGIN}
                  className="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  Login
                </Link>
                <Link
                  to={ROUTES.REGISTER}
                  className="px-4 py-2 text-sm font-medium bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Sign Up
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
