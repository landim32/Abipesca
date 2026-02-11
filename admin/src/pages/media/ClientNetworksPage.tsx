import { useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { SocialNetworkList, SocialNetworkModal } from 'bazzuca-react';
import { toast } from 'sonner';
import { ROUTES } from '../../lib/constants';
import { ArrowLeft, Share2, Plus } from 'lucide-react';

export function ClientNetworksPage() {
  const { clientId } = useParams<{ clientId: string }>();
  const navigate = useNavigate();
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedNetwork, setSelectedNetwork] = useState<any>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const refresh = useCallback(() => {
    setRefreshKey((prev) => prev + 1);
  }, []);

  const handleCreate = () => {
    setSelectedNetwork(null);
    setModalOpen(true);
  };

  const handleEdit = (network: any) => {
    setSelectedNetwork(network);
    setModalOpen(true);
  };

  const handleDelete = () => {
    refresh();
    toast.success('Social network removed successfully.');
  };

  const handleSave = () => {
    setModalOpen(false);
    setSelectedNetwork(null);
    refresh();
    toast.success(
      selectedNetwork
        ? 'Social network updated successfully.'
        : 'Social network added successfully.'
    );
  };

  const handleClose = () => {
    setModalOpen(false);
    setSelectedNetwork(null);
  };

  const handleBack = () => {
    navigate(ROUTES.CLIENTS);
  };

  return (
    <div className="max-w-6xl mx-auto">
      {/* Back Button */}
      <button
        onClick={handleBack}
        className="inline-flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors mb-6"
      >
        <ArrowLeft className="w-4 h-4" />
        Back to Clients
      </button>

      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
        <div>
          <div className="flex items-center gap-3 mb-2">
            <div className="w-10 h-10 bg-blue-100 dark:bg-blue-900/30 rounded-lg flex items-center justify-center">
              <Share2 className="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
            <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
              Social Networks
            </h1>
          </div>
          <p className="text-gray-600 dark:text-gray-400">
            Manage social network connections for this client.
          </p>
        </div>
        <button
          onClick={handleCreate}
          className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
        >
          <Plus className="w-4 h-4" />
          Add Network
        </button>
      </div>

      {/* Social Network List */}
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700">
        <SocialNetworkList
          key={refreshKey}
          clientId={clientId!}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      </div>

      {/* Social Network Modal */}
      <SocialNetworkModal
        open={modalOpen}
        onClose={handleClose}
        onSave={handleSave}
        clientId={clientId!}
        network={selectedNetwork}
      />
    </div>
  );
}
