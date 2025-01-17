/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import {
  Box,
  Typography,
  TextField,
  FormControl,
  Autocomplete,
} from "@mui/material";
import { DeviationDTO } from "../../models/exceptionManagementDTOs";

interface MiscellaneousExceptionFormProps {
  formValues: any;
  tranTypeCategory: any;
  onDeviationChange: (e: any, value: DeviationDTO[]) => void;
  deviationOption: DeviationDTO[];
  selectedDeviation: DeviationDTO[];
  onInputChange: (field: any, value: any) => void;
  isEdit?: boolean;
}

const MiscellaneousExceptionForm: React.FC<MiscellaneousExceptionFormProps> = ({
  formValues,
  onDeviationChange,
  deviationOption,
  selectedDeviation,
  tranTypeCategory,
  onInputChange,
  isEdit = false,
}) => {
  return (
    <Box
      sx={{
        maxWidth: 600,
        margin: "auto",
        mt: 5,
        p: 3,
        border: "1px solid #ccc",
        borderRadius: 2,
        boxShadow: 3,
      }}
    >
      <Typography variant="h5" gutterBottom mb={'22px'}>
        Miscellaneous Exception Details
      </Typography>

      {/* Card Number */}
      {tranTypeCategory === 1 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Card Number."
            variant="outlined"
            fullWidth
            value={formValues.cardNo}
            onChange={(e) => onInputChange("cardNo", e.target.value)}
          />
        </Box>
      )}

      {/* Bank Cert Number */}
      {tranTypeCategory === 2 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Bank Cert Number"
            variant="outlined"
            fullWidth
            value={formValues.bankCertNo}
            onChange={(e) => onInputChange("bankCertNo", e.target.value)}
          />
        </Box>
      )}

      {/* DPAF Number */}
      {tranTypeCategory === 4 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="DPAF Number"
            variant="outlined"
            fullWidth
            value={formValues.dpafno}
            onChange={(e) => onInputChange("dpafno", e.target.value)}
          />
        </Box>
      )}

      {/* Amount */}
      <Box sx={{ mb: 2 }}>
        <TextField
          label="Amount"
          variant="outlined"
          fullWidth
          value={formValues.amount}
          onChange={(e) => onInputChange("amount", e.target.value)}
        />
      </Box>

      {/* Check Number */}
      {tranTypeCategory === 7 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Check Number"
            variant="outlined"
            fullWidth
            value={formValues.checkNo}
            onChange={(e) => onInputChange("checkNo", e.target.value)}
          />
        </Box>
      )}

      {/* Account Number */}
      {tranTypeCategory !== 5 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Account Number"
            variant="outlined"
            fullWidth
            value={formValues.glslaccountNo}
            onChange={(e) => onInputChange("glslaccountNo", e.target.value)}
          />
        </Box>
      )}

      {/* Account Name */}
      {tranTypeCategory !== 5 && (
        <Box sx={{ mb: 2 }}>
          <TextField
            label="Account Name"
            variant="outlined"
            fullWidth
            value={formValues.glslaccountName}
            onChange={(e) => onInputChange("glslaccountName", e.target.value)}
          />
        </Box>
      )}

      {/* Exception Code/s */}
      {!isEdit  && (
        <FormControl fullWidth>
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

export default MiscellaneousExceptionForm;
