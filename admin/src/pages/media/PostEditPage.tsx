import { useParams, useNavigate } from 'react-router-dom';
import { PostEditor } from 'bazzuca-react';
import { toast } from 'sonner';
import { ROUTES } from '../../lib/constants';
import { ArrowLeft } from 'lucide-react';

export function PostEditPage() {
  const { postId } = useParams<{ postId: string }>();
  const navigate = useNavigate();

  const isEditing = Boolean(postId);

  const handleSave = () => {
    toast.success(
      isEditing ? 'Post updated successfully.' : 'Post created successfully.'
    );
    navigate(ROUTES.POSTS);
  };

  const handleCancel = () => {
    navigate(ROUTES.POSTS);
  };

  const handleBack = () => {
    navigate(ROUTES.POSTS);
  };

  return (
    <div className="max-w-4xl mx-auto">
      {/* Back Button */}
      <button
        onClick={handleBack}
        className="inline-flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors mb-6"
      >
        <ArrowLeft className="w-4 h-4" />
        Back to Posts
      </button>

      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
          {isEditing ? 'Edit Post' : 'Create Post'}
        </h1>
        <p className="mt-2 text-gray-600 dark:text-gray-400">
          {isEditing
            ? 'Update your post content and scheduling options.'
            : 'Compose a new post and schedule it for publishing.'}
        </p>
      </div>

      {/* Post Editor */}
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <PostEditor
          postId={postId}
          onSave={handleSave}
          onCancel={handleCancel}
        />
      </div>
    </div>
  );
}
