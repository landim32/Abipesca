import { useNavigate, useSearchParams } from 'react-router-dom';
import { UserEditForm } from 'nauth-react';
import type { UserInfo } from 'nauth-react';
import { toast } from 'sonner';
import { ROUTES } from '../../lib/constants';

export default function UserEditPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const userId = searchParams.get('userId');

  const isEditMode = !!userId;

  const handleSuccess = (user: UserInfo) => {
    const message = isEditMode
      ? `User "${user.name}" updated successfully!`
      : `User "${user.name}" created successfully!`;

    toast.success(message);

    // Navigate to user list
    navigate(ROUTES.SEARCH_USERS);
  };

  const handleError = (error: Error) => {
    console.error('Error saving user:', error);
    toast.error(`Error: ${error.message}`);
  };

  const handleCancel = () => {
    navigate(-1); // Go back to previous page
  };

  return (
    <div className="container mx-auto py-8 px-4">
      <div className="max-w-4xl mx-auto">
        <UserEditForm
          userId={userId ? parseInt(userId) : undefined}
          onSuccess={handleSuccess}
          onError={handleError}
          onCancel={handleCancel}
        />
      </div>
    </div>
  );
}
