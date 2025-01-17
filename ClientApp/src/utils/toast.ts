// src/utils/ToastService.ts
import { toast } from 'react-toastify';

const ToastService = {
    success: (message: string, options = {}) => {
        toast.dismiss();
        toast.success(message, { ...options });
    },
    error: (message: string, options = {}) => {
        toast.dismiss();
        toast.error(message, { ...options });
    },
    info: (message: string, options = {}) => {
        toast.dismiss();
        toast.info(message, { ...options });
    },
    warning: (message: string, options = {}) => {
        toast.dismiss();
        toast.warn(message, { ...options });
    },
};

export default ToastService;
