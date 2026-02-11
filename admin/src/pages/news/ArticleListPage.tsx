import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useArticles } from 'nnews-react';
import type { Article, ArticleStatus } from 'nnews-react';
import { ROUTES } from '../../lib/constants';
import { cn } from '../../lib/utils';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { toast } from 'sonner';
import {
  Plus,
  ChevronLeft,
  ChevronRight,
  FileText,
  Loader2,
  AlertCircle,
  Inbox,
  Trash2,
} from 'lucide-react';

const PAGE_SIZE = 10;

const statusLabels: Record<number, string> = {
  0: 'Draft',
  1: 'Published',
  2: 'Archived',
  3: 'Scheduled',
  4: 'Review',
};

const statusColors: Record<number, string> = {
  0: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
  1: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
  2: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
  3: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
  4: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
};

export function ArticleListPage() {
  const navigate = useNavigate();
  const { articles, loading, error, fetchArticles, deleteArticle } = useArticles();
  const [currentPage, setCurrentPage] = useState(1);

  const loadArticles = useCallback(
    async (page: number) => {
      try {
        await fetchArticles({ page, pageSize: PAGE_SIZE });
      } catch {
        toast.error('Failed to load articles');
      }
    },
    [fetchArticles]
  );

  useEffect(() => {
    loadArticles(currentPage);
  }, [currentPage, loadArticles]);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleArticleClick = (article: Article) => {
    navigate(ROUTES.ARTICLES_EDIT(String(article.articleId)));
  };

  const handleDelete = async (e: React.MouseEvent, articleId: number) => {
    e.stopPropagation();
    if (!confirm('Are you sure you want to delete this article?')) return;

    try {
      await deleteArticle(articleId);
      toast.success('Article deleted successfully');
    } catch {
      toast.error('Failed to delete article');
    }
  };

  const formatDate = (date?: string | Date) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  return (
    <div className="max-w-6xl mx-auto">
      <Card>
        <CardHeader className="p-6 pb-0">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <FileText className="h-8 w-8 text-blue-600 dark:text-blue-400" />
              <CardTitle>Articles</CardTitle>
            </div>
            <button
              onClick={() => navigate(ROUTES.ARTICLES_NEW)}
              className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:focus:ring-offset-gray-800 transition-colors"
            >
              <Plus className="h-4 w-4" />
              New Article
            </button>
          </div>
        </CardHeader>

        <CardContent className="p-6">
          {/* Loading State */}
          {loading && (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-blue-600 dark:text-blue-400" />
              <span className="ml-3 text-gray-600 dark:text-gray-400">Loading articles...</span>
            </div>
          )}

          {/* Error State */}
          {error && !loading && (
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <AlertCircle className="h-12 w-12 text-red-500 dark:text-red-400 mb-3" />
              <p className="text-red-600 dark:text-red-400 font-medium">
                Error loading articles
              </p>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                {error.message}
              </p>
              <button
                onClick={() => loadArticles(currentPage)}
                className="mt-4 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
              >
                Try Again
              </button>
            </div>
          )}

          {/* Empty State */}
          {!loading && !error && articles && articles.items.length === 0 && (
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <Inbox className="h-12 w-12 text-gray-400 dark:text-gray-500 mb-3" />
              <p className="text-gray-600 dark:text-gray-400 font-medium">
                No articles found
              </p>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                Create your first article to get started.
              </p>
              <button
                onClick={() => navigate(ROUTES.ARTICLES_NEW)}
                className="mt-4 inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
              >
                <Plus className="h-4 w-4" />
                New Article
              </button>
            </div>
          )}

          {/* Articles List */}
          {!loading && !error && articles && articles.items.length > 0 && (
            <>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b border-gray-200 dark:border-gray-700">
                      <th className="pb-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400">
                        Title
                      </th>
                      <th className="pb-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400">
                        Category
                      </th>
                      <th className="pb-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400">
                        Status
                      </th>
                      <th className="pb-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400">
                        Date
                      </th>
                      <th className="pb-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400">
                        Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-100 dark:divide-gray-700">
                    {articles.items.map((article) => (
                      <tr
                        key={article.articleId}
                        onClick={() => handleArticleClick(article)}
                        className="cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
                      >
                        <td className="py-4 pr-4">
                          <div className="flex items-center gap-3">
                            {article.imageUrl ? (
                              <img
                                src={article.imageUrl}
                                alt={article.title}
                                className="h-10 w-10 rounded-md object-cover flex-shrink-0"
                              />
                            ) : (
                              <div className="h-10 w-10 rounded-md bg-gray-100 dark:bg-gray-700 flex items-center justify-center flex-shrink-0">
                                <FileText className="h-5 w-5 text-gray-400 dark:text-gray-500" />
                              </div>
                            )}
                            <span className="font-medium text-gray-900 dark:text-gray-100 truncate max-w-xs">
                              {article.title}
                            </span>
                          </div>
                        </td>
                        <td className="py-4 pr-4 text-sm text-gray-600 dark:text-gray-400">
                          {article.category?.title || '-'}
                        </td>
                        <td className="py-4 pr-4">
                          <span
                            className={cn(
                              'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium',
                              statusColors[article.status] || statusColors[0]
                            )}
                          >
                            {statusLabels[article.status] || 'Unknown'}
                          </span>
                        </td>
                        <td className="py-4 pr-4 text-sm text-gray-600 dark:text-gray-400">
                          {formatDate(article.dateAt || article.createdAt)}
                        </td>
                        <td className="py-4 text-right">
                          <button
                            onClick={(e) => handleDelete(e, article.articleId)}
                            className="inline-flex items-center rounded-md p-1.5 text-gray-400 hover:text-red-600 dark:hover:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
                            title="Delete article"
                          >
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Pagination */}
              {articles.totalPages > 1 && (
                <div className="mt-6 flex items-center justify-between border-t border-gray-200 dark:border-gray-700 pt-4">
                  <p className="text-sm text-gray-600 dark:text-gray-400">
                    Page {articles.page} of {articles.totalPages} ({articles.totalCount} total)
                  </p>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => handlePageChange(currentPage - 1)}
                      disabled={!articles.hasPrevious}
                      className={cn(
                        'inline-flex items-center gap-1 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                        articles.hasPrevious
                          ? 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
                          : 'text-gray-400 dark:text-gray-600 cursor-not-allowed'
                      )}
                    >
                      <ChevronLeft className="h-4 w-4" />
                      Previous
                    </button>
                    <button
                      onClick={() => handlePageChange(currentPage + 1)}
                      disabled={!articles.hasNext}
                      className={cn(
                        'inline-flex items-center gap-1 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                        articles.hasNext
                          ? 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
                          : 'text-gray-400 dark:text-gray-600 cursor-not-allowed'
                      )}
                    >
                      Next
                      <ChevronRight className="h-4 w-4" />
                    </button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
