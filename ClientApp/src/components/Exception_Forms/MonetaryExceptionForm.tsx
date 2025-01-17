/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React from 'react';
import {
  Box,
  Typography,
  TextField,
  MenuItem,
  Select,
  Button,
  FormControl,
  InputLabel,
  Autocomplete,
} from '@mui/material';
import { CurrencyDisplay, DeviationDTO, MonetaryRev } from '../../models/exceptionManagementDTOs';

interface MonetaryExceptionFormProps {
  formValues: MonetaryRev;
  // onInputChange: (field: string, value: string | string[]) => void;
  onInputChange: (field: keyof MonetaryRev, value: any) => void;
  onDeviationChange: (e: any, value: DeviationDTO[]) => void;
  deviationOption: DeviationDTO[];
  selectedDeviation: DeviationDTO[];
  currencies?: string[];
  isEdit?: boolean;

}

const MonetaryExceptionForm: React.FC<MonetaryExceptionFormProps> = ({
  formValues,
  onInputChange,
  onDeviationChange,
  deviationOption,
  selectedDeviation,
  isEdit = false,
}) => {
  // const [isDisabled, setIsDisabled] = useState(false)
  
  const handleValidate = async () => {
    // Validation logic
  };

  return (
    <Box
      sx={{
        maxWidth: 600,
        margin: 'auto',
        mt: 5,
        p: 3,
        border: '1px solid #ccc',
        borderRadius: 2,
        boxShadow: 3,
      }}
    >
      <Typography variant="h5" gutterBottom mb={'22px'}>
        Monetary Exception Details
      </Typography>

      {/* BDS User ID and Sequence No with Validate Button */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
        <TextField
          label="BDS User ID"
          variant="outlined"
          fullWidth
          value={formValues.bdstellerId}
          onChange={(e) => onInputChange('bdstellerId', e.target.value)}
        />
        <TextField
          label="Sequence No"
          variant="outlined"
          fullWidth
          value={formValues.sequenceNo}
          onChange={(e) => onInputChange('sequenceNo', e.target.value)}
        />
        {isEdit && (
          <Button variant="contained" color="primary" onClick={handleValidate}>
            Validate
          </Button>
        )}

      </Box>

      {/* Transaction Code and Description */}
      <Box sx={{ mb: 2 }}>
        <TextField
          label="Transaction Code"
          variant="outlined"
          fullWidth
          value={formValues.transCode}
          onChange={(e) => onInputChange('transCode', e.target.value)}
        />
      </Box>
      <Box sx={{ mb: 2 }}>
        <TextField
          label="Transaction Description"
          variant="outlined"
          fullWidth
          value={formValues.transDescription}
          onChange={(e) =>
            onInputChange('transDescription', e.target.value)
          }
        />
      </Box>

      {/* Credit Account Details */}
      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <TextField
          label="Credit Account/GL Reference Number"
          variant="outlined"
          fullWidth
          value={formValues.creditAccountNo}
          onChange={(e) => onInputChange('creditAccountNo', e.target.value)}
        />
        <TextField
          label="Credit Account/GL Account Name"
          variant="outlined"
          fullWidth
          value={formValues.creditAccountName}
          onChange={(e) =>
            onInputChange('creditAccountName', e.target.value)
          }
        />
      </Box>

      {/* Debit Account Details */}
      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <TextField
          label="Debit Account/GL Reference Number"
          variant="outlined"
          fullWidth
          value={formValues.debitAccountNo}
          onChange={(e) => onInputChange('debitAccountNo', e.target.value)}
        />
        <TextField
          label="Debit Account/GL Account Name"
          variant="outlined"
          fullWidth
          value={formValues.debitAccountName}
          onChange={(e) => onInputChange('debitAccountName', e.target.value)}
        />
      </Box>

      {/* Amount and Currency */}
      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <TextField
          label="Amount"
          variant="outlined"
          fullWidth
          value={formValues.amount}
          onChange={(e) => onInputChange('amount', e.target.value)}
        />
        <FormControl fullWidth>
          <InputLabel id="currency-label">Currency</InputLabel>
          <Select
            labelId="currency-label"
            label="Currency"
            value={formValues.currency}
            onChange={(e) => onInputChange('currency', e.target.value as string)}
          >
            <MenuItem value="">
              <em>-- Select --</em>
            </MenuItem>
            {Object.entries(CurrencyDisplay).map(([key, label]) => (
              <MenuItem key={key} value={parseInt(key, 10)}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>

      {/* Exception Code */}
      {!isEdit  && (
      <FormControl fullWidth sx={{ mb: 2 }}>

        {/* <InputLabel id="exception-code-label">Exception Code/s</InputLabel>
        <TextField
          label="Exception Code/s"
          value={Array.isArray(formValues.exceptionId) ? formValues.exceptionId.join(', ') : ''}
          onChange={(e) => onInputChange('exceptionCode', e.target.value.split(',').map(code => code.trim()))}
          variant="outlined"
          fullWidth
        /> */}

        <Autocomplete
          multiple
          options={deviationOption}
          getOptionLabel={(option) => option.category || ''}
          value={selectedDeviation}
          onChange={(_event, value) => onDeviationChange(_event, value)}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Exception Code/s"
              placeholder="Exception Code/s"
              variant="outlined"
              fullWidth
            />
          )}
          renderOption={(props, option) => (
            <li {...props} key={option.id || option.category}> {/* Ensure unique key */}
              {option.category}
            </li>
          )}
        />

      </FormControl>
      )}
    </Box>
  );
};

export default MonetaryExceptionForm;
