/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable no-constant-binary-expression */
/* eslint-disable @typescript-eslint/no-unused-expressions */
/* eslint-disable @typescript-eslint/no-empty-object-type */
/* eslint-disable @typescript-eslint/no-explicit-any */
// /* eslint-disable @typescript-eslint/no-unused-vars */
import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Grid,
  MenuItem,
  Select,
  TextField,
  Typography,
  Checkbox,
  FormControlLabel,
  Divider,
  Chip,
  Autocomplete,
} from '@mui/material';
import MonetaryExceptionForm from '../../../components/Exception_Forms/MonetaryExceptionForm';
import {
  AgingCategoryDisplay,
  ExceptionDTO,
  ExceptionItemDTO,
  ExceptionItemRevDTO,
  ActionPlan,
  TransactionTypeDisplay,
  NonMonetaryTypesDisplay,
  ExceptionCategoryDisplay,
  DeviationDTO,
  ActionPlans,
  RootCauseDisplay,
} from '../../../models/exceptionManagementDTOs';
import NonMonetaryExceptionForm from '../../../components/Exception_Forms/NonMonetaryExceptionForm';
import ExceptionManagement from '../../../services/exceptionManagement';
import MiscellaneousExceptionForm from '../../../components/Exception_Forms/MiscellaneousExceptionForm';
import ToastService from '../../../utils/toast';
import { useNavigate, useParams } from 'react-router-dom';


const EditException: React.FC = () => {
  const navigate = useNavigate();
  // const [branchOption, setBranchOption] = useState<GroupDTO[]>();
  // const [deviation, setDeviation] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  // const [exceptionCodes, setExceptionCodes] = useState<ExceptionCodeDTO[]>([]);
  const [deviationOptions, setDeviationOptions] = useState<DeviationDTO[]>([]);
  const [selectedDeviation, setSelectedDeviation] = useState<DeviationDTO[]>([]);
  const [formValues, setFormValues] = useState<ExceptionDTO>({
    employeeID: '',
    employeeName: '',
    exceptionItem: {} as ExceptionItemDTO, // Provide a default empty object
    exceptionItemRevs: {} as ExceptionItemRevDTO, // Provide a default empty object
    selectedExCodes: [],
    actionPlans: [] as ActionPlans[],
    actionPlan: {} as ActionPlan, // Provide a default empty object
    subExceptionItems: [],
    hasPendingUpdate: false, // Default value for boolean
    hasFormChanges: false, // Default value for boolean
    request: '',
    approvalRemarks: '',
  });

  type Errors = {
    employeeId: string;
    transactionType: string;
    rootCause: string;
    branchName: string;
    branchCode: string;
    division: string;
    category: string;
    area: string;
    exceptionApprover: string;
    agingCategory: any;
    employeeResponsible: string;
    otherEmployeesResponsible: string;
    remarks: string;
    [key: string]: string;

  };

  const [errors, setErrors] = useState<Errors>({
    employeeId: '',
    transactionType: '',
    category: '',
    rootCause: '',
    branchName: '',
    branchCode: '',
    division: '',
    area: '',
    exceptionApprover: '',
    agingCategory: '',
    employeeResponsible: '',
    otherEmployeesResponsible: '',
    remarks: '',
  });


  const { refNo } = useParams<{ refNo: string }>();

  useEffect(() => {
    if (refNo) {
      fetchBranchDetails();
    }

  }, [refNo]);


  const fetchBranchDetails = async () => {
    try {
      const result = await ExceptionManagement.getExceptionUpdateDetails(refNo);
      setFormValues(result.data.data);
      console.log('codes: ', formValues.selectedExCodes)

      fetchCurrentDeviation(result.data.data.selectedExCodes);
      console.log(result.data.data.selectedExCodes);
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  }

  // const selectedDeviation = () => {

  // }

  const handleDeviationChange = (
    _event: React.ChangeEvent<{}>,
    value: DeviationDTO[]
  ) => {

    setSelectedDeviation(value);

    const selectedExCodes = Array.from(
      new Set(value.map((deviation: DeviationDTO) => deviation.id))
    );

    setFormValues((prevFormValues) => ({
      ...prevFormValues,
      selectedExCodes,
    }));

    console.log(`selectedExCodes: `, formValues.selectedExCodes);
    console.log(`selectedDeviation: `, selectedDeviation);
  };



  const handleMonetaryInputChange = (field: any, value: any) => {
    setFormValues((prevState) => {
      const updatednMonetaries = prevState.exceptionItem?.monetaries?.map((monetary, index) =>
        index === 0 // Assuming you want to update the first item in the array
          ? { ...monetary, [field]: value }
          : monetary
      );

      return {
        ...prevState,
        exceptionItem: {
          ...prevState.exceptionItem,
          monetaries: updatednMonetaries,
        },
      } as ExceptionDTO;
    });

    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }
  };

  const handleNonMonetaryInputChange = (field: any, value: any) => {
    setFormValues((prevState) => {
      const updatednonMonetaries = prevState.exceptionItem?.nonMonetaries?.map((nonMonetary, index) =>
        index === 0 // Assuming you want to update the first item in the array
          ? { ...nonMonetary, [field]: value }
          : nonMonetary
      );

      return {
        ...prevState,
        exceptionItem: {
          ...prevState.exceptionItem,
          nonMonetaries: updatednonMonetaries,
        },
      } as ExceptionDTO;
    });

    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }
  };

  const handleMiscInputChange = (field: any, value: any) => {
    setFormValues((prevState) => {
      const updatedMiscs = prevState.exceptionItem?.miscs?.map((misc, index) =>
        index === 0 // Assuming you want to update the first item in the array
          ? { ...misc, [field]: value }
          : misc
      );

      return {
        ...prevState,
        exceptionItem: {
          ...prevState.exceptionItem,
          miscs: updatedMiscs,
        },
      } as ExceptionDTO;
    });

    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }
  };


  const handleNestedInputChange = (field: keyof ExceptionDTO, subField: string, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      [field]: {
        ...(typeof prevState[field] === 'object' ? prevState[field] : {}), // Spread the existing object if it's an object
        [subField]: value,   // Update the specific subField
      },
    }));




  };


  const handleDeepNestedInputChange = (field: keyof ExceptionDTO, subField: string, nestedField: string, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      [field]: {
        ...(typeof prevState[field] === 'object' ? prevState[field] : {}),
        [subField]: {
          ...(prevState[field] as any)[subField], // Spread the existing nested object
          [nestedField]: value, // Update the specific nested field
        },
      },
    }));
  };


  // const handleInputChange = async (field: keyof Errors, value: any) => {
  //   setFormValues({
  //     ...formValues,
  //     [field]: value,
  //   });

  //   if (errors[field]) {
  //     setErrors({ ...errors, [field]: '' }); // Clear the error on input change
  //   }

  // };



  const fetchCurrentDeviation = async (ids: number[]) => {



    const currentDeviation = await ExceptionManagement.getDeviationByIds(ids)

    console.log('Current Deviation', currentDeviation.data)
    setSelectedDeviation(currentDeviation.data)

  }


  const fetchDeviationOption = async () => {
    let deviation = '';

    switch (formValues.exceptionItem?.type) {
      case 1:
        deviation = 'Monetary';
        break;
      case 2:
        deviation = 'Non-Monetary';
        break;
      case 3:
        deviation = 'Miscellaneous';
        break;
      default:
        deviation = '';
    }

    if (deviation) {
      // setDeviation(deviation);
      const exceptionData = await ExceptionManagement.getDeviationByClasification(deviation);
      setDeviationOptions(exceptionData.data);

    }


  };

  const validateForm = () => {
    let valid = true;
    const newErrors: any = {};
    if (!formValues.exceptionItem?.employeeId) {
      newErrors.employeeId = 'Employee ID is required.';
      valid = false;
    }
    if (!formValues.exceptionItem?.type) {
      newErrors.transactionType = 'Transaction Type is required.';
      valid = false;
    }
    if (!formValues.exceptionItem?.rootCause) {
      newErrors.rootCause = 'Root Cause is required.';
      valid = false;
    }

    if (!formValues.exceptionItem?.transactionDate) {
      newErrors.transactionDate = 'Transaction Date is required.';
      valid = false;
    }

    if (formValues.exceptionItem?.agingCategory === null) {
      newErrors.agingCategory = 'Aging Category is required.';
      valid = false;
    }
    if (!formValues.exceptionItem?.personResponsible?.trim()) {
      newErrors.employeeResponsible = 'Employee Responsible is required.';
      valid = false;
    }


    setErrors(newErrors);
    return valid;
  };

  const handleSubmit = async () => {

    setIsLoading(true)
    // console.log(formValues.exceptionItem.monetaries)
    try {
      if (validateForm()) {
        // console.log(formValues);
        const result = await ExceptionManagement.updateException(formValues);

        // console.log(`result: `, result)

        if (result.success) {
          ToastService.success(result.message)
          navigate('/ExceptionsManagement/Dashboard')

        } else {
          ToastService.error(result.message)
        }

      } else {
        console.log('Validation failed.');
      }

    } catch (error) {
      console.log(`Error in creating exception`, error)
      ToastService.error("Error in creating exception.")
    } finally {
      setIsLoading(false)
    }

  };

  // const isObjectEmpty = (obj: object| null): boolean => {
  //   return Object.keys(obj).length === 0;
  // };

  // const handleReset = () => {
  //   setFormValues({
  //     employeeID: '',
  //     employeeName: '',
  //     exceptionItem: {} as ExceptionItemDTO,
  //     exceptionItemRevs: {} as ExceptionItemRevDTO,
  //     selectedExCodes: [],
  //     actionPlans: [],
  //     actionPlan: {} as ActionPlan,
  //     subExceptionItems: [],
  //     hasPendingUpdate: false,
  //     hasFormChanges: false,
  //     request: '',
  //     approvalRemarks: '',
  //   });
  //   handleClearErrors();
  // };

  // const handleClearErrors = () => {
  //   setErrors({
  //     employeeId: '',
  //     transactionType: '',
  //     rootCause: '',
  //     category: '',
  //     branchName: '',
  //     branchCode: '',
  //     division: '',
  //     area: '',
  //     exceptionApprover: '',
  //     agingCategory: '',
  //     employeeResponsible: '',
  //     otherEmployeesResponsible: '',
  //     remarks: '',
  //     exceptionCodes: '',
  //     miscs: '',
  //     monetaries: '',
  //     nonMonetaries: '',
  //     actionPlans: '',
  //     selectedExCodes: '',
  //     subExceptionItems: '',
  //     request: '',
  //     approvalRemarks: ''

  //   });
  // }

  // const getSelectedDeviation = async () => {
  //   try {
  //     const deviationCodes = formValues.exceptionItem?.exceptionCodes;
  //     if (deviationCodes) {

  //       array.forEach(element => {

  //       });


  //     } else {
  //       console.log("error in getting user branch id's")
  //       return [];
  //     }
  //   } catch (error) {
  //     console.log(`error: ${error}`);
  //   }
  // }
  const StatusUI = () => {
    if (formValues != null) {
      if (formValues.exceptionItem?.status === 1) {
        return (
          <Typography variant="h5">
            Update Exception Details - <Chip label="Open" color="primary" />
          </Typography>
        );
      } else if (formValues.exceptionItem?.status === 2) {
        return (
          <Typography variant="h5">
            Exception Details - <Chip label="Closed" color="success" />
          </Typography>
        );
      } else if (formValues.exceptionItem?.status === 0) {
        return (
          <Typography variant="h5">
            <Chip label="Update Exception Request" color="primary" /> - <Chip label="Pending Approval" color="warning" />
          </Typography>
        );
      }
    }
    return null;
  };

  return (
    <>
      <StatusUI />
      <Divider sx={{ mb: 2, mt: 2 }} />

      <Box display="flex">

        <Box width="50%">
          <Typography variant="h5" mb={2} textAlign={'center'}>
            Information
          </Typography>
          <Divider />
          <Grid container spacing={2}>
            <Grid item xs={12} sm={12}>
              <TextField
                label="Employee ID"
                fullWidth
                value={formValues.exceptionItem?.employeeId}
                disabled
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            {/* <Grid item xs={12} sm={6} mt={1}>
            <Button variant="contained" fullWidth>
              Validate
            </Button>
          </Grid> */}

            <Grid item xs={12} sm={6}>
              <TextField
                label="Branch Name"
                fullWidth
                value={formValues.exceptionItem?.branchName}
                InputLabelProps={{ shrink: true }}
                disabled
              />

            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Branch Code"
                fullWidth
                value={formValues.exceptionItem?.branchCode}
                InputLabelProps={{ shrink: true }}
                disabled
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Division"
                fullWidth
                value={formValues.exceptionItem?.division}
                InputLabelProps={{ shrink: true }}
                disabled
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Area"
                fullWidth
                value={formValues.exceptionItem?.area}
                InputLabelProps={{ shrink: true }}
                disabled
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Transaction Date"
                type="date"
                fullWidth
                value={formValues.exceptionItem?.transactionDate
                  ? formValues.exceptionItem.transactionDate.split("T")[0]
                  : ""}
                onChange={(e) =>
                  handleNestedInputChange('exceptionItem', 'transactionDate', e.target.value)
                }
                InputLabelProps={{
                  shrink: true,
                }}
                error={!!errors.transactionDate}
                helperText={errors.transactionDate || ""}
              />
            </Grid>



            <Grid item xs={12} sm={6}>
              <Typography variant="subtitle1">Root Cause</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItem?.rootCause || ""}
                onChange={(e) =>
                  handleNestedInputChange('exceptionItem', 'rootCause', e.target.value)
                }
                error={!!errors.rootCause}
              >
                <MenuItem value="">
                  -- Select Root Cause --
                </MenuItem>
                {Object.entries(RootCauseDisplay).map(([key, label]) => (
                  <MenuItem key={key} value={parseInt(key, 10)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
              {errors.rootCause && (
                <Typography color="error" variant="caption">
                  {errors.rootCause}
                </Typography>
              )}
            </Grid>


            <Grid item xs={12} sm={6} mt={'22px'}>
              <TextField
                label="Exception Approver"
                fullWidth
                value={formValues.exceptionItem?.deviationApprover}
                disabled={formValues.exceptionItem?.rootCause === 1}
                onChange={(e) => handleNestedInputChange('exceptionItem', 'deviationApprover', e.target.value)}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item xs={12} sm={12}>
              <Typography variant="subtitle1">Aging Category</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItem?.agingCategory ?? ""}
                onChange={(e) =>
                  handleNestedInputChange('exceptionItem', 'agingCategory', e.target.value)
                }
                error={!!errors.agingCategory}
              >
                <MenuItem value="">
                  -- Select Aging Category --
                </MenuItem>
                {Object.entries(AgingCategoryDisplay).map(([key, label]) => (
                  <MenuItem key={key} value={parseInt(key, 10)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
              {errors.agingCategory && (
                <Typography color="error" variant="caption">
                  {errors.agingCategory}
                </Typography>
              )}
            </Grid>


            <Grid item xs={12} sm={12}>
              <TextField
                label="Employee Responsible"
                fullWidth
                value={formValues.exceptionItem?.personResponsible}
                // onChange={(e) => handleInputChange('employeeResponsible', e.target.value)}
                onChange={(e) => handleNestedInputChange('exceptionItem', 'personResponsible', e.target.value)}
                error={!!errors.employeeResponsible}
                helperText={errors.employeeResponsible}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} sm={12}>
              <TextField
                label="Other Employee's Responsible"
                fullWidth
                value={formValues.exceptionItem?.otherPersonResponsible}
                InputLabelProps={{ shrink: true }}
                onChange={(e) => handleNestedInputChange('exceptionItem', 'otherPersonResponsible', e.target.value)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Remarks"
                multiline
                rows={4}
                fullWidth
                value={formValues.exceptionItem?.remarks}
                // onChange={(e) => handleInputChange('remarks', e.target.value)}
                onChange={(e) => handleNestedInputChange('exceptionItem', 'remarks', e.target.value)}
              />
            </Grid>

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formValues.exceptionItem?.redFlag ?? false}
                    value={formValues.exceptionItem?.redFlag ? "on" : "off"}
                    disabled
                  // onChange={(e) => handleNestedInputChange('exceptionItem', 'redFlag', e.target.checked ? "on" : "off")}
                  />
                }
                label="Red Flag"
              />
            </Grid>


          </Grid>
        </Box>
        <Box width="50%" ml={10}>

          <Typography variant="h5" mt={2} mb={2} textAlign={'center'}>
            Transaction
          </Typography>
          <Divider />
          {/* 
          <Grid item xs={12} sm={12}>
            <TextField
              label="Transaction Type"
              fullWidth
              value={deviation}
              disabled
            />

          </Grid> */}

          <Grid item xs={12} sm={12}>
            <Typography variant="subtitle1">Transaction Type</Typography>
            <Select
              displayEmpty
              fullWidth
              value={formValues.exceptionItem?.type || ""}
              onChange={(e) => handleNestedInputChange('exceptionItem', 'type', e.target.value)}
              disabled
            >
              <MenuItem value="">
                -- Select Transaction Type --
              </MenuItem>
              {Object.entries(TransactionTypeDisplay).map(([key, label]) => (
                <MenuItem key={key} value={parseInt(key, 10)}>
                  {label}
                </MenuItem>
              ))}
            </Select>
          </Grid>


          {formValues.exceptionItem?.type === 2 && (

            <Grid item xs={12} sm={12} mt={2}>
              <Typography variant="subtitle1">Category</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItem.nonMonetaries?.[0].category}
                onChange={(e) => handleDeepNestedInputChange('exceptionItem', 'nonMonetaryRevs', 'type', e.target.value)}
                error={!!errors.transactionType}
                disabled

              >
                <MenuItem value={0}>-- Select Category --</MenuItem>
                {Object.entries(NonMonetaryTypesDisplay).map(([key, label]) => (
                  <MenuItem key={key} value={parseInt(key, 10)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
              {errors.transactionType && (
                <Typography color="error" variant="caption">
                  {errors.transactionType}
                </Typography>
              )}
            </Grid>
          )}


          {formValues.exceptionItem?.type === 3 && (

            <Grid item xs={12} sm={12} mt={2}>
              <Typography variant="subtitle1">Category</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItem.miscs?.[0]?.category}
                onChange={(e) => handleDeepNestedInputChange('exceptionItem', 'miscRevs', 'type', e.target.value)}
                error={!!errors.transactionType}
                disabled
              >
                <MenuItem value={0}>-- Select Category --</MenuItem>
                {Object.entries(ExceptionCategoryDisplay).map(([key, label]) => (
                  <MenuItem key={key} value={parseInt(key, 10)}>
                    {label}
                  </MenuItem>
                ))}
              </Select>
              {errors.transactionType && (
                <Typography color="error" variant="caption">
                  {errors.transactionType}
                </Typography>
              )}
            </Grid>
          )}



          {formValues.exceptionItem?.type === 1 && (
            <MonetaryExceptionForm
              deviationOption={deviationOptions || []}
              selectedDeviation={selectedDeviation || []}
              onDeviationChange={handleDeviationChange}
              formValues={formValues.exceptionItem?.monetaries?.[0] ?? ({} as any)}
              onInputChange={handleMonetaryInputChange}
              isEdit={true}
            />
          )}

          {formValues.exceptionItem?.type === 2 && (
            <NonMonetaryExceptionForm
              isEdit={true}
              deviationOption={deviationOptions || []}
              selectedDeviation={selectedDeviation || []}
              onDeviationChange={handleDeviationChange}
              formValues={formValues.exceptionItem?.nonMonetaries?.[0] ?? ({} as any)}
              onInputChange={handleNonMonetaryInputChange}
            />
          )}

          {(formValues.exceptionItem?.type === 3 && formValues.exceptionItem.miscs?.[0]?.category)
            && (
              <MiscellaneousExceptionForm
                isEdit={true}
                deviationOption={deviationOptions || []}
                selectedDeviation={selectedDeviation || []}
                onDeviationChange={handleDeviationChange}
                formValues={formValues.exceptionItem?.miscs?.[0] ?? ({} as any)}
                tranTypeCategory={formValues.exceptionItem.miscs?.[0]?.category}
                onInputChange={handleMiscInputChange}
              />
            )}


          <Typography variant="h5" mt={2} mb={2} textAlign={'center'}>
            Deviation
          </Typography>
          <Autocomplete
            multiple
            options={deviationOptions || []}
            getOptionLabel={(option) => option.category || ''}
            value={selectedDeviation}
            onChange={handleDeviationChange}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Exception Code/s"
                placeholder="Exception Code/s"
                variant="outlined"
                fullWidth
                onFocus={() => {
                  if (!deviationOptions || deviationOptions.length === 0) {
                    fetchDeviationOption(); // Call the fetch function
                  }
                }}
              />
            )}
            renderOption={(props, option) => (
              <li {...props} key={option.id || option.category}> {/* Ensure unique key */}
                {option.category}
              </li>
            )}
          />


          <Divider />

        </Box>


      </Box>
      <Grid item xs={12} sm={12} display="flex" justifyContent="space-between" mt={2}>
        <Button variant="contained" color="primary" onClick={() => navigate('/ExceptionsManagement/Details/' + refNo)} style={{ width: '20%' }}>
          Back
        </Button>
        <Button variant="contained" color="success" disabled={isLoading} onClick={handleSubmit} style={{ width: '40%' }}>
        {isLoading ? "Submitting..." : "Submit"}
        </Button>
      </Grid>
    </>

  );
};

export default EditException;
