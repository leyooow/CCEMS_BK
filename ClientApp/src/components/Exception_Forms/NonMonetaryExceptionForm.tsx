/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Button, TextField, Typography, Box, Autocomplete, FormControl } from "@mui/material";
import { DeviationDTO } from "../../models/exceptionManagementDTOs";

interface MonetaryExceptionFormProps {
  formValues: any;
  onInputChange: (field: any, value: any) => void;
  onDeviationChange: (e: any, value: DeviationDTO[]) => void;
  deviationOption: DeviationDTO[];
  selectedDeviation: DeviationDTO[];
  currencies?: string[];
  isEdit?: boolean;
}

const NonMonetaryExceptionForm: React.FC<MonetaryExceptionFormProps> = ({
  formValues,
  onInputChange,
  onDeviationChange,
  deviationOption,
  selectedDeviation,
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
        Non-Monetary Exception Details
      </Typography>

      {/* CIF Number Field with Button */}
      <Box sx={{ display: "flex", alignItems: "center", gap: 2, mb: 2 }}>
        <TextField
          label="CIF Number"
          variant="outlined"
          fullWidth
          value={formValues.cifnumber}
          onChange={(e) => onInputChange("cifnumber", e.target.value)}
        />
        <Button variant="contained" color="primary" sx={{ whiteSpace: "nowrap" }}>
          Validate
        </Button>
      </Box>

      {/* Customer Name Field */}
      <Box sx={{ mb: 2 }}>
        <TextField
          label="Customer Name"
          variant="outlined"
          fullWidth
          value={formValues.customerName}
          onChange={(e) => onInputChange("customerName", e.target.value)}
        />
      </Box>

      {/* Customer Account Number Field */}
      <Box sx={{ mb: 2 }}>
        <TextField
          label="Customer Account Number"
          variant="outlined"
          fullWidth
          value={formValues.customerAccountNo}
          onChange={(e) => onInputChange("customerAccountNo", e.target.value)}
        />
      </Box>

      {/* Exception Code Field */}
      {!isEdit && (
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

export default NonMonetaryExceptionForm;
