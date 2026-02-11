import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useArticles, useCategories, ArticleEditor } from 'nnews-react';
import type { Article, ArticleInput, ArticleUpdate } from 'nnews-react';
import { ROUTES } from '../../lib/constants';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { toast } from 'sonner';
import { ArrowLeft, FileEdit, FilePlus, Loader2, AlertCircle } from 'lucide-react';

export function ArticleEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { getArticleById, createArticle, updateArticle, loading: saving } = useArticles();
  const { categories, fetchCategories } = useCategories();

  const [article, setArticle] = useState<Article | null>(null);
  const [loadingArticle, setLoadingArticle] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);

  const isEditMode = Boolean(id);

  const loadArticle = useCallback(async () => {
    if (!id) return;

    setLoadingArticle(true);
    setLoadError(null);

    try {
      const data = await getArticleById(Number(id));
      setArticle(data);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to load article';
      setLoadError(message);
      toast.error('Failed to load article');
    } finally {
      setLoadingArticle(false);
    }
  }, [id, getArticleById]);

  useEffect(() => {
    fetchCategories().catch(() => {
      // Categories are optional, don't block the editor
    });
  }, [fetchCategories]);

  useEffect(() => {
    if (isEditMode) {
      loadArticle();
    }
  }, [isEditMode, loadArticle]);

  const handleSave = async (data: ArticleInput | ArticleUpdate) => {
    try {
      if (isEditMode && 'articleId' in data) {
        await updateArticle(data as ArticleUpdate);
        toast.success('Article updated successfully');
      } else {
        await createArticle(data as ArticleInput);
        toast.success('Article created successfully');
      }
      navigate(ROUTES.ARTICLES);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to save article';
      toast.error(message);
    }
  };

  const handleCancel = () => {
    navigate(ROUTES.ARTICLES);
  };

  return (
    <div className="max-w-6xl mx-auto">
      {/* Back Button */}
      <button
        onClick={handleCancel}
        className="mb-4 inline-flex items-center gap-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 transition-colors"
      >
        <ArrowLeft className="h-4 w-4" />
        Back to Articles
      </button>

      <Card>
        <CardHeader className="p-6 pb-0">
          <div className="flex items-center gap-3">
            {isEditMode ? (
              <FileEdit className="h-8 w-8 text-blue-600 dark:text-blue-400" />
            ) : (
              <FilePlus className="h-8 w-8 text-green-600 dark:text-green-400" />
            )}
            <CardTitle>{isEditMode ? 'Edit Article' : 'New Article'}</CardTitle>
          </div>
        </CardHeader>

        <CardContent className="p-6">
          {/* Loading State */}
          {loadingArticle && (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-blue-600 dark:text-blue-400" />
              <span className="ml-3 text-gray-600 dark:text-gray-400">Loading article...</span>
            </div>
          )}

          {/* Error State */}
          {loadError && !loadingArticle && (
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <AlertCircle className="h-12 w-12 text-red-500 dark:text-red-400 mb-3" />
              <p className="text-red-600 dark:text-red-400 font-medium">
                Error loading article
              </p>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                {loadError}
              </p>
              <div className="mt-4 flex gap-3">
                <button
                  onClick={loadArticle}
                  className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
                >
                  Try Again
                </button>
                <button
                  onClick={handleCancel}
                  className="rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  Go Back
                </button>
              </div>
            </div>
          )}

          {/* Editor */}
          {!loadingArticle && !loadError && (
            <ArticleEditor
              article={isEditMode ? article : undefined}
              categories={categories}
              onSave={handleSave}
              onCancel={handleCancel}
              loading={saving}
            />
          )}
        </CardContent>
      </Card>
    </div>
  );
}
