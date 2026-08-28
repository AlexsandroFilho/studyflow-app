import React, { useState } from "react";
import { Modal } from "../ui/Modal";
import { Button } from "../ui/Button";
import { AlertTriangle } from "lucide-react";

interface ConfirmDeleteModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  description: string;
  onConfirm: () => Promise<void>;
}

export const ConfirmDeleteModal: React.FC<ConfirmDeleteModalProps> = ({
  isOpen,
  onClose,
  title,
  description,
  onConfirm,
}) => {
  const [loading, setLoading] = useState(false);

  const handleConfirm = async () => {
    setLoading(true);
    try {
      await onConfirm();
      onClose();
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title} maxWidth="sm">
      <div className="space-y-4">
        <div className="flex items-start gap-3 p-3.5 rounded-lg bg-red-50 border border-red-200 text-red-600">
          <AlertTriangle className="w-4 h-4 shrink-0 mt-0.5 text-red-500" />
          <p className="text-xs leading-relaxed">{description}</p>
        </div>

        <div className="flex items-center justify-end gap-2 pt-2 border-t border-slate-200">
          <Button variant="ghost" size="sm" onClick={onClose} disabled={loading}>
            Cancelar
          </Button>
          <Button
            variant="danger"
            size="sm"
            onClick={handleConfirm}
            isLoading={loading}
          >
            Excluir Definitivamente
          </Button>
        </div>
      </div>
    </Modal>
  );
};
