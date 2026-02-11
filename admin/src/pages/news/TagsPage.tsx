import { useState, useEffect, useCallback, useMemo } from 'react';
import { useTags } from 'nnews-react';
import type { Tag } from 'nnews-react';
import { cn } from '../../lib/utils';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { toast } from 'sonner';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  Tag as TagIcon,
  Loader2,
  AlertCircle,
  Inbox,
  X,
  Merge,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react';

const PAGE_SIZE = 10;

export function TagsPage() {
  const {
    tags,
    loading,
    error,
    fetchTags,
    createTag,
    updateTag,
    deleteTag,
    mergeTags,
  } = useTags();

  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);

  // Modal state
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTag, setEditingTag] = useState<Tag | null>(null);
  const [modalName, setModalName] = useState('');
  const [modalSlug, setModalSlug] = useState('');
  const [modalLoading, setModalLoading] = useState(false);

  // Delete confirmation state
  const [deleteTarget, setDeleteTarget] = useState<Tag | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  // Merge state
  const [isMergeOpen, setIsMergeOpen] = useState(false);
  const [mergeSource, setMergeSource] = useState<number | ''>('');
  const [mergeTarget, setMergeTarget] = useState<number | ''>('');
  const [mergeLoading, setMergeLoading] = useState(false);

  const loadTags = useCallback(async () => {
    try {
      await fetchTags({ searchTerm: searchTerm || undefined });
    } catch {
      toast.error('Failed to load tags');
    }
  }, [fetchTags, searchTerm]);

  useEffect(() => {
    loadTags();
  }, [loadTags]);

  // Client-side pagination
  const filteredTags = useMemo(() => {
    return tags;
  }, [tags]);

  const totalPages = Math.ceil(filteredTags.length / PAGE_SIZE);
  const paginatedTags = useMemo(() => {
    const start = (currentPage - 1) * PAGE_SIZE;
    return filteredTags.slice(start, start + PAGE_SIZE);
  }, [filteredTags, currentPage]);

  // Reset page when search changes
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm]);

  // Create/Edit Modal handlers
  const openCreateModal = () => {
    setEditingTag(null);
    setModalName('');
    setModalSlug('');
    setIsModalOpen(true);
  };

  const openEditModal = (tag: Tag) => {
    setEditingTag(tag);
    setModalName(tag.title);
    setModalSlug(tag.slug || '');
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingTag(null);
    setModalName('');
    setModalSlug('');
  };

  const handleModalSave = async () => {
    const trimmedName = modalName.trim();
    if (!trimmedName) {
      toast.error('Tag name is required');
      return;
    }

    setModalLoading(true);
    try {
      if (editingTag && editingTag.tagId) {
        await updateTag({
          tagId: editingTag.tagId,
          title: trimmedName,
          slug: modalSlug.trim() || undefined,
        });
        toast.success('Tag updated successfully');
      } else {
        await createTag({
          title: trimmedName,
          slug: modalSlug.trim() || undefined,
        });
        toast.success('Tag created successfully');
      }
      closeModal();
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to save tag';
      toast.error(message);
    } finally {
      setModalLoading(false);
    }
  };

  // Delete handlers
  const openDeleteConfirm = (tag: Tag) => {
    setDeleteTarget(tag);
  };

  const closeDeleteConfirm = () => {
    setDeleteTarget(null);
  };

  const handleDelete = async () => {
    if (!deleteTarget || !deleteTarget.tagId) return;

    setDeleteLoading(true);
    try {
      await deleteTag(deleteTarget.tagId);
      toast.success('Tag deleted successfully');
      closeDeleteConfirm();
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to delete tag';
      toast.error(message);
    } finally {
      setDeleteLoading(false);
    }
  };

  // Merge handlers
  const openMergeModal = () => {
    setMergeSource('');
    setMergeTarget('');
    setIsMergeOpen(true);
  };

  const closeMergeModal = () => {
    setIsMergeOpen(false);
    setMergeSource('');
    setMergeTarget('');
  };

  const handleMerge = async () => {
    if (!mergeSource || !mergeTarget) {
      toast.error('Please select both source and target tags');
      return;
    }

    if (mergeSource === mergeTarget) {
      toast.error('Source and target tags must be different');
      return;
    }

    setMergeLoading(true);
    try {
      await mergeTags(Number(mergeSource), Number(mergeTarget));
      toast.success('Tags merged successfully');
      closeMergeModal();
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to merge tags';
      toast.error(message);
    } finally {
      setMergeLoading(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleModalSave();
    }
  };

  return (
    <div className="max-w-4xl mx-auto">
      <Card>
        <CardHeader className="p-6 pb-0">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <TagIcon className="h-8 w-8 text-blue-600 dark:text-blue-400" />
              <CardTitle>Tags</CardTitle>
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={openMergeModal}
                disabled={tags.length < 2}
                className="inline-flex items-center gap-2 rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:focus:ring-offset-gray-800 disabled:opacity-50 transition-colors"
              >
                <Merge className="h-4 w-4" />
                Merge Tags
              </button>
              <button
                onClick={openCreateModal}
                className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:focus:ring-offset-gray-800 transition-colors"
              >
                <Plus className="h-4 w-4" />
                New Tag
              </button>
            </div>
          </div>
        </CardHeader>

        <CardContent className="p-6">
          {/* Search */}
          <div className="mb-6">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400 dark:text-gray-500" />
              <input
                type="text"
                placeholder="Search tags..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 py-2 pl-10 pr-4 text-sm text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Loading State */}
          {loading && !modalLoading && !deleteLoading && !mergeLoading && (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-blue-600 dark:text-blue-400" />
              <span className="ml-3 text-gray-600 dark:text-gray-400">Loading tags...</span>
            </div>
          )}

          {/* Error State */}
          {error && !loading && (
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <AlertCircle className="h-12 w-12 text-red-500 dark:text-red-400 mb-3" />
              <p className="text-red-600 dark:text-red-400 font-medium">
                Error loading tags
              </p>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                {error.message}
              </p>
              <button
                onClick={loadTags}
                className="mt-4 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
              >
                Try Again
              </button>
            </div>
          )}

          {/* Empty State */}
          {!loading && !error && filteredTags.length === 0 && (
            <div className="flex flex-col items-center justify-center py-12 text-center">
              <Inbox className="h-12 w-12 text-gray-400 dark:text-gray-500 mb-3" />
              <p className="text-gray-600 dark:text-gray-400 font-medium">
                {searchTerm ? 'No tags match your search' : 'No tags found'}
              </p>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                {searchTerm
                  ? 'Try adjusting your search term.'
                  : 'Create your first tag to get started.'}
              </p>
              {!searchTerm && (
                <button
                  onClick={openCreateModal}
                  className="mt-4 inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 transition-colors"
                >
                  <Plus className="h-4 w-4" />
                  New Tag
                </button>
              )}
            </div>
          )}

          {/* Tags List */}
          {!loading && !error && paginatedTags.length > 0 && (
            <>
              <div className="space-y-2">
                {paginatedTags.map((tag) => (
                  <div
                    key={tag.tagId}
                    className="flex items-center justify-between rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 px-4 py-3 hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
                  >
                    <div className="flex items-center gap-3">
                      <TagIcon className="h-5 w-5 text-gray-400 dark:text-gray-500" />
                      <div>
                        <p className="font-medium text-gray-900 dark:text-gray-100">
                          {tag.title}
                        </p>
                        <div className="flex items-center gap-3">
                          {tag.slug && (
                            <p className="text-xs text-gray-500 dark:text-gray-400">
                              slug: {tag.slug}
                            </p>
                          )}
                          {tag.articleCount !== undefined && (
                            <p className="text-xs text-gray-500 dark:text-gray-400">
                              {tag.articleCount} {tag.articleCount === 1 ? 'article' : 'articles'}
                            </p>
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="flex items-center gap-1">
                      <button
                        onClick={() => openEditModal(tag)}
                        className="inline-flex items-center rounded-md p-1.5 text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-colors"
                        title="Edit tag"
                      >
                        <Edit2 className="h-4 w-4" />
                      </button>
                      <button
                        onClick={() => openDeleteConfirm(tag)}
                        className="inline-flex items-center rounded-md p-1.5 text-gray-400 hover:text-red-600 dark:hover:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
                        title="Delete tag"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    </div>
                  </div>
                ))}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="mt-6 flex items-center justify-between border-t border-gray-200 dark:border-gray-700 pt-4">
                  <p className="text-sm text-gray-600 dark:text-gray-400">
                    Page {currentPage} of {totalPages} ({filteredTags.length} total)
                  </p>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                      disabled={currentPage <= 1}
                      className={cn(
                        'inline-flex items-center gap-1 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                        currentPage > 1
                          ? 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
                          : 'text-gray-400 dark:text-gray-600 cursor-not-allowed'
                      )}
                    >
                      <ChevronLeft className="h-4 w-4" />
                      Previous
                    </button>
                    <button
                      onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                      disabled={currentPage >= totalPages}
                      className={cn(
                        'inline-flex items-center gap-1 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                        currentPage < totalPages
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

      {/* Create/Edit Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div
            className="fixed inset-0 bg-black/50"
            onClick={closeModal}
          />
          <div className="relative z-10 w-full max-w-md rounded-xl bg-white dark:bg-gray-800 shadow-2xl border border-gray-200 dark:border-gray-700">
            <div className="flex items-center justify-between border-b border-gray-200 dark:border-gray-700 px-6 py-4">
              <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
                {editingTag ? 'Edit Tag' : 'New Tag'}
              </h2>
              <button
                onClick={closeModal}
                className="rounded-md p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              >
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="space-y-4 px-6 py-4">
              <div>
                <label
                  htmlFor="tag-name"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Name *
                </label>
                <input
                  id="tag-name"
                  type="text"
                  value={modalName}
                  onChange={(e) => setModalName(e.target.value)}
                  onKeyDown={handleKeyDown}
                  placeholder="Enter tag name"
                  autoFocus
                  className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 px-4 py-2 text-sm text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
              <div>
                <label
                  htmlFor="tag-slug"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Slug
                </label>
                <input
                  id="tag-slug"
                  type="text"
                  value={modalSlug}
                  onChange={(e) => setModalSlug(e.target.value)}
                  placeholder="Auto-generated if empty"
                  className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 px-4 py-2 text-sm text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
            </div>
            <div className="flex justify-end gap-3 border-t border-gray-200 dark:border-gray-700 px-6 py-4">
              <button
                onClick={closeModal}
                disabled={modalLoading}
                className="rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleModalSave}
                disabled={modalLoading || !modalName.trim()}
                className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50 transition-colors"
              >
                {modalLoading && <Loader2 className="h-4 w-4 animate-spin" />}
                {editingTag ? 'Update' : 'Create'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deleteTarget && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div
            className="fixed inset-0 bg-black/50"
            onClick={closeDeleteConfirm}
          />
          <div className="relative z-10 w-full max-w-sm rounded-xl bg-white dark:bg-gray-800 shadow-2xl border border-gray-200 dark:border-gray-700">
            <div className="px-6 py-4">
              <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
                Delete Tag
              </h2>
              <p className="mt-2 text-sm text-gray-600 dark:text-gray-400">
                Are you sure you want to delete <strong>"{deleteTarget.title}"</strong>?
                This action cannot be undone.
              </p>
            </div>
            <div className="flex justify-end gap-3 border-t border-gray-200 dark:border-gray-700 px-6 py-4">
              <button
                onClick={closeDeleteConfirm}
                disabled={deleteLoading}
                className="rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleDelete}
                disabled={deleteLoading}
                className="inline-flex items-center gap-2 rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 disabled:opacity-50 transition-colors"
              >
                {deleteLoading && <Loader2 className="h-4 w-4 animate-spin" />}
                Delete
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Merge Tags Modal */}
      {isMergeOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
          <div
            className="fixed inset-0 bg-black/50"
            onClick={closeMergeModal}
          />
          <div className="relative z-10 w-full max-w-md rounded-xl bg-white dark:bg-gray-800 shadow-2xl border border-gray-200 dark:border-gray-700">
            <div className="flex items-center justify-between border-b border-gray-200 dark:border-gray-700 px-6 py-4">
              <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
                Merge Tags
              </h2>
              <button
                onClick={closeMergeModal}
                className="rounded-md p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              >
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="space-y-4 px-6 py-4">
              <p className="text-sm text-gray-600 dark:text-gray-400">
                All articles from the source tag will be moved to the target tag.
                The source tag will be deleted after merging.
              </p>
              <div>
                <label
                  htmlFor="merge-source"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Source Tag (will be deleted)
                </label>
                <select
                  id="merge-source"
                  value={mergeSource}
                  onChange={(e) => setMergeSource(e.target.value ? Number(e.target.value) : '')}
                  className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 px-4 py-2 text-sm text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                >
                  <option value="">Select source tag...</option>
                  {tags
                    .filter((t) => t.tagId && t.tagId !== mergeTarget)
                    .map((tag) => (
                      <option key={tag.tagId} value={tag.tagId}>
                        {tag.title}
                        {tag.articleCount !== undefined ? ` (${tag.articleCount} articles)` : ''}
                      </option>
                    ))}
                </select>
              </div>
              <div>
                <label
                  htmlFor="merge-target"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
                >
                  Target Tag (will receive articles)
                </label>
                <select
                  id="merge-target"
                  value={mergeTarget}
                  onChange={(e) => setMergeTarget(e.target.value ? Number(e.target.value) : '')}
                  className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 px-4 py-2 text-sm text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                >
                  <option value="">Select target tag...</option>
                  {tags
                    .filter((t) => t.tagId && t.tagId !== mergeSource)
                    .map((tag) => (
                      <option key={tag.tagId} value={tag.tagId}>
                        {tag.title}
                        {tag.articleCount !== undefined ? ` (${tag.articleCount} articles)` : ''}
                      </option>
                    ))}
                </select>
              </div>
            </div>
            <div className="flex justify-end gap-3 border-t border-gray-200 dark:border-gray-700 px-6 py-4">
              <button
                onClick={closeMergeModal}
                disabled={mergeLoading}
                className="rounded-lg border border-gray-300 dark:border-gray-600 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleMerge}
                disabled={mergeLoading || !mergeSource || !mergeTarget}
                className="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50 transition-colors"
              >
                {mergeLoading && <Loader2 className="h-4 w-4 animate-spin" />}
                Merge Tags
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
