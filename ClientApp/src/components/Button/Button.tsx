import React from 'react';
import Button, { ButtonProps } from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import VisibilityIcon from '@mui/icons-material/Visibility';

interface GlobalButtonProps extends Omit<ButtonProps, 'action'> {
  buttonAction?: 'add' | 'edit' | 'delete' | 'view';
  buttonName?: string; // New prop for dynamic button name
}

const GlobalButton: React.FC<GlobalButtonProps> = ({
  buttonAction,
  buttonName,
  variant = 'contained',
  color,
  children,
  ...props
}) => {
  // Map actions to colors
  const getColorForAction = () => {
    switch (buttonAction) {
      case 'add':
        return 'success';
      case 'edit':
        return 'primary';
      case 'delete':
        return 'error';
      case 'view':
        return 'info';
      default:
        return color || 'primary'; // Use passed color or default to 'primary'
    }
  };

  // Map actions to icons
  const renderActionIcon = () => {
    switch (buttonAction) {
      case 'add':
        return <AddIcon style={{ marginRight: 8 }} />;
      case 'edit':
        return <EditIcon style={{ marginRight: 8 }} />;
      case 'delete':
        return <DeleteIcon style={{ marginRight: 8 }} />;
      case 'view':
        return <VisibilityIcon style={{ marginRight: 8 }} />;
      default:
        return null;
    }
  };

  return (
    <Button variant={variant} color={getColorForAction()} {...props}>
      {renderActionIcon()}
      {buttonName || children}
    </Button>
  );
};

export default GlobalButton;
