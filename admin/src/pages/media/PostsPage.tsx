import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { PostList, PostCalendar, useSocialNetworks } from 'bazzuca-react';
import { toast } from 'sonner';
import { ROUTES } from '../../lib/constants';
import { Plus, FileText, List, CalendarDays } from 'lucide-react';

type ViewMode = 'list' | 'calendar';

export function PostsPage() {
  const navigate = useNavigate();
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [selectedClientId, setSelectedClientId] = useState<string>('');
  const { clients } = useSocialNetworks();

  const handleCreate = () => {
    navigate(ROUTES.POSTS_CREATE);
  };

  const handleEdit = (post: any) => {
    navigate(ROUTES.POSTS_EDIT(post.id));
  };

  const handleView = (post: any) => {
    navigate(ROUTES.POSTS_VIEW(post.id));
  };

  const handlePublish = (post: any) => {
    toast.success(`Post "${post.title || 'Untitled'}" published successfully.`);
  };

  return (
    <div className="max-w-6xl mx-auto">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
        <div>
          <div className="flex items-center gap-3 mb-2">
            <div className="w-10 h-10 bg-green-100 dark:bg-green-900/30 rounded-lg flex items-center justify-center">
              <FileText className="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
            <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
              Posts
            </h1>
          </div>
          <p className="text-gray-600 dark:text-gray-400">
            Create, schedule, and manage social media posts across all platforms.
          </p>
        </div>
        <button
          onClick={handleCreate}
          className="inline-flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium"
        >
          <Plus className="w-4 h-4" />
          Create Post
        </button>
      </div>

      {/* Toolbar: View Toggle + Client Filter */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
        {/* View Toggle */}
        <div className="inline-flex rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 p-1">
          <button
            onClick={() => setViewMode('list')}
            className={`inline-flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-md transition-colors ${
              viewMode === 'list'
                ? 'bg-green-600 text-white'
                : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white'
            }`}
          >
            <List className="w-4 h-4" />
            List
          </button>
          <button
            onClick={() => setViewMode('calendar')}
            className={`inline-flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-md transition-colors ${
              viewMode === 'calendar'
                ? 'bg-green-600 text-white'
                : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white'
            }`}
          >
            <CalendarDays className="w-4 h-4" />
            Calendar
          </button>
        </div>

        {/* Client Filter */}
        <div className="flex items-center gap-2">
          <label
            htmlFor="client-filter"
            className="text-sm font-medium text-gray-700 dark:text-gray-300"
          >
            Client:
          </label>
          <select
            id="client-filter"
            value={selectedClientId}
            onChange={(e) => setSelectedClientId(e.target.value)}
            className="px-3 py-2 text-sm bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg text-gray-900 dark:text-white focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            <option value="">All Clients</option>
            {clients?.map((client: any) => (
              <option key={client.id} value={client.id}>
                {client.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Content */}
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
        {viewMode === 'list' ? (
          <PostList
            clientId={selectedClientId || undefined}
            onEdit={handleEdit}
            onView={handleView}
            onPublish={handlePublish}
          />
        ) : (
          <PostCalendar
            clientId={selectedClientId || undefined}
            onPostClick={handleView}
          />
        )}
      </div>
    </div>
  );
}
