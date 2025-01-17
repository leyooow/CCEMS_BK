/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react-hooks/exhaustive-deps */
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
} from '@mui/material';
import EmployeeService from '../../../services/employeeService';
import GroupService from '../../../services/groupService';
import { GroupDTO } from '../../../models/groupDTOs';
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
  NonMonetaryRev,
  MonetaryRev,
  MiscRev,
  RootCauseDisplay,
} from '../../../models/exceptionManagementDTOs';
import NonMonetaryExceptionForm from '../../../components/Exception_Forms/NonMonetaryExceptionForm';
import ExceptionManagement from '../../../services/exceptionManagement';
import MiscellaneousExceptionForm from '../../../components/Exception_Forms/MiscellaneousExceptionForm';
import ToastService from '../../../utils/toast';
import { useNavigate } from 'react-router-dom';


const AddException: React.FC = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [branchOption, setBranchOption] = useState<GroupDTO[]>();
  // const [exceptionCodes, setExceptionCodes] = useState<ExceptionCodeDTO[]>([]);
  const [deviationOptions, setDeviationOptions] = useState<DeviationDTO[]>([]);
  const [selectedDeviation, setSelectedDeviation] = useState<DeviationDTO[]>([]);
  const [formValues, setFormValues] = useState<ExceptionDTO>({
    employeeID: '',
    employeeName: '',
    exceptionItem: {} as ExceptionItemDTO, // Provide a default empty object
    exceptionItemRevs: {
      agingCategory: null,
    } as ExceptionItemRevDTO, // Provide a default empty object
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



  useEffect(() => {


  }, [selectedDeviation, formValues]);


  const handleDeviationChange = (
    _event: React.ChangeEvent<{}>,
    value: DeviationDTO[]
  ) => {
    // Update selected deviations first
    setSelectedDeviation(value);

    // Generate selectedExCodes based on the latest 'value' and convert to integers
    const selectedExCodes = Array.from(
      new Set(value.map((deviation: DeviationDTO) => deviation.id)) // Ensure 'id' is treated as an integer
    );

    // Use functional setState for formValues to ensure the most recent values
    setFormValues((prevFormValues) => ({
      ...prevFormValues,
      selectedExCodes,
    }));

    // console.log(`selectedExCodes: `, selectedExCodes);
  };



  const handleMonetaryInputChange = (field: keyof MonetaryRev, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      exceptionItemRevs: {
        ...prevState.exceptionItemRevs,
        monetaryRevs: {
          ...prevState.exceptionItemRevs.monetaryRevs,
          [field]: value,
        },
      },
    } as ExceptionDTO));
    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }
  };

  const handleNonMonetaryInputChange = (field: keyof NonMonetaryRev, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      exceptionItemRevs: {
        ...prevState.exceptionItemRevs,
        nonMonetaryRevs: {
          ...prevState.exceptionItemRevs.nonMonetaryRevs,
          [field]: value,
        },
      },
    } as ExceptionDTO));
    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }
  };

  const handleMiscInputChange = (field: keyof MiscRev, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      exceptionItemRevs: {
        ...prevState.exceptionItemRevs,
        miscRevs: {
          ...prevState.exceptionItemRevs.miscRevs,
          [field]: value,
        },
      },
    } as ExceptionDTO));
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



  const handleTransactionTypeInputChange = (field: keyof ExceptionDTO, subField: string, value: any) => {
    setFormValues((prevState) => ({
      ...prevState,
      [field]: {
        ...(typeof prevState[field] === 'object' ? prevState[field] : {}), // Spread the existing object if it's an object
        [subField]: value,   // Update the specific subField
      },
    }));

    setFormValues((prevState) => ({
      ...prevState,
      exceptionItemRevs: {
        ...prevState.exceptionItemRevs,
        miscRevs: {} as MiscRev,
        monetaryRevs: {} as MonetaryRev,
        nonMonetaryRevs: {} as NonMonetaryRev,
      } as ExceptionItemRevDTO,
    }));

    setSelectedDeviation([])


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


  const handleInputChange = async (field: keyof Errors, value: any) => {
    setFormValues({
      ...formValues,
      [field]: value,
    });

    if (errors[field]) {
      setErrors({ ...errors, [field]: '' }); // Clear the error on input change
    }

  };
  const fetchDeviationOption = async () => {
    let deviation = '';

    switch (formValues.exceptionItemRevs.type) {
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
      const exceptionData = await ExceptionManagement.getDeviationByClasification(deviation);
      setDeviationOptions(exceptionData.data);
    }
  };

  useEffect(() => {

    fetchDeviationOption();
  }, [formValues.exceptionItemRevs.type]);

  const validateForm = () => {
    let valid = true;
    const newErrors: any = {};
    if (!formValues.employeeID) {
      newErrors.employeeId = 'Employee ID is required.';
      valid = false;
    }
    if (!formValues.exceptionItemRevs.type) {
      newErrors.transactionType = 'Transaction Type is required.';
      valid = false;
    }
    if (!formValues.exceptionItemRevs.rootCause) {
      newErrors.rootCause = 'Root Cause is required.';
      valid = false;
    }
    // if (!formValues.branchName.trim()) {
    //   newErrors.branchName = 'Branch Name is required.';
    //   valid = false;
    // }
    // if (!formValues.exceptionItem.branchCode) {
    //   newErrors.branchCode = 'Branch Code is required.';
    //   valid = false;
    // }
    // if (!formValues.exceptionItem.division.trim()) {
    //   newErrors.division = 'Division is required.';
    //   valid = false;
    // }
    // if (!formValues.exceptionItem.area.trim()) {
    //   newErrors.area = 'Area is required.';
    //   valid = false;
    // }
    // if (!formValues.transactionDate.trim()) {
    //   newErrors.area = 'Transaction Date is required.';
    //   valid = false;
    // }
    if (!formValues.exceptionItemRevs.transactionDate) {
      newErrors.transactionDate = 'Transaction Date is required.';
      valid = false;
    }
    // if (!formValues.exceptionApprover.trim()) {
    //   newErrors.exceptionApprover = 'Exception Approver is required.';
    //   valid = false;
    // }
    if (formValues.exceptionItemRevs.agingCategory === null) {
      newErrors.agingCategory = 'Aging Category is required.';
      valid = false;
    }
    if (!formValues.exceptionItemRevs.personResponsible?.trim()) {
      newErrors.employeeResponsible = 'Employee Responsible is required.';
      valid = false;
    }

    // if (!formValues.otherEmployeesResponsible.trim()) {
    //   newErrors.otherEmployeesResponsible = 'Other Employees Responsible is required.';
    //   valid = false;
    // }
    // if (!formValues.remarks.trim()) {
    //   newErrors.remarks = 'Remarks are required.';
    //   valid = false;
    // }

    setErrors(newErrors);
    return valid;
  };

  const handleSubmit = async () => {
    setIsLoading(true)
    try {
      if (validateForm()) {
        // Proceed only if form validation succeeds
        const updatedFormValues = {
          ...formValues,
          exceptionItem: null
        };

        const result = await ExceptionManagement.saveException(updatedFormValues);

        if (result.success) {
          ToastService.success(result.message);
          navigate('/ExceptionsManagement/Dashboard');
        } else {
          ToastService.error(result.message);
        }
      } else {
        console.log('Validation failed.');
      }
    } catch (error) {
      console.log(`Error in creating exception`, error);
      ToastService.error("Error in creating exception.");
    } finally {
      setIsLoading(false)
    }
  };


  // const isObjectEmpty = (obj: object| null): boolean => {
  //   return Object.keys(obj).length === 0;
  // };

  const handleReset = () => {
    setFormValues({
      employeeID: '',
      employeeName: '',
      exceptionItem: {} as ExceptionItemDTO,
      exceptionItemRevs: {} as ExceptionItemRevDTO,
      selectedExCodes: [],
      actionPlans: [],
      actionPlan: {} as ActionPlan,
      subExceptionItems: [],
      hasPendingUpdate: false,
      hasFormChanges: false,
      request: '',
      approvalRemarks: '',
    });
    handleClearErrors();
  };

  const handleClearErrors = () => {
    setErrors({
      employeeId: '',
      transactionType: '',
      rootCause: '',
      category: '',
      branchName: '',
      branchCode: '',
      division: '',
      area: '',
      exceptionApprover: '',
      agingCategory: '',
      employeeResponsible: '',
      otherEmployeesResponsible: '',
      remarks: '',
      exceptionCodes: '',
      miscs: '',
      monetaries: '',
      nonMonetaries: '',
      actionPlans: '',
      selectedExCodes: '',
      subExceptionItems: '',
      request: '',
      approvalRemarks: ''

    });
  }



  const onBranchOptionChange = async (branchId: any) => {
    try {
      const result = await GroupService.getGroupById(branchId);

      if (result.success) {
        const branchDetails = result.data;
        setFormValues((prevState) => ({
          ...prevState,
          exceptionItemRevs: {
            ...prevState.exceptionItemRevs,
            branchName: branchDetails?.name,
            branchCode: branchDetails?.code,
            division: branchDetails?.division,
            area: branchDetails?.area,
          },
        }));
      } else {
        console.error("Error in getting branch details");
      }
    } catch (error) {
      console.error(`Error: ${error}`);
    }
  };

  const handleValidate = async (employeeId: string) => {
    handleClearErrors();
    setIsLoading(true)
    try {
      const result = await EmployeeService.getEmployeeById(employeeId);

      if (result.success) {

        // const branchIds = await getBranchIds(employeeId);
        const branchAccesses = await GroupService.GetBranchDropdown();

        const distinctBranchAccesses = branchAccesses.filter(
          (branch: GroupDTO, index: number, self: GroupDTO[]) =>
            index === self.findIndex((b) => b.code === branch.code)
        );

        setBranchOption(distinctBranchAccesses);

        console.log(branchOption)

        const fullName = `${result.data.firstName} ${result.data.middleName} ${result.data.lastName}`;
        setFormValues({
          ...formValues,
          employeeName: fullName,
          exceptionItemRevs: {
            ...formValues.exceptionItemRevs,
            personResponsible: fullName
          }
        });
      } else {
        setBranchOption([])
        setErrors({ ...errors, employeeId: 'Employee ID is invalid.' });
      }
    } catch (error) {
      setErrors({ ...errors, employeeId: 'Error validating employee ID.' });
    }
    setIsLoading(false)
  };

  return (

    <Box display="flex">

      <Box width="50%">
        <Typography variant="h5" mb={2} textAlign={'center'}>
          Information
        </Typography>
        <Divider />
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <TextField
              label="Employee ID"
              fullWidth
              value={formValues.employeeID}
              onChange={(e) => handleInputChange('employeeID', e.target.value)}
              error={!!errors.employeeId}
              helperText={errors.employeeId}
            />
          </Grid>
          <Grid item xs={12} sm={6} mt={1}>
            <Button variant="contained" disabled={isLoading} fullWidth onClick={() => handleValidate(formValues.employeeID)}>
              {isLoading ? "Validating..." : "Valdiate"}
            </Button>
          </Grid>
          <Grid item xs={12} sm={12}>
            <TextField
              label="Employee Name"
              fullWidth
              value={formValues.employeeName}
              onChange={(e) => handleInputChange('employeeName', e.target.value)}
              disabled
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="subtitle1">Branch Name</Typography>
            <Select
              displayEmpty
              fullWidth
              // value={formValues.branchName || ""}
              onChange={(e) => {
                const selectedBranchId = e.target.value;
                onBranchOptionChange(selectedBranchId);
              }}
            // error={!!errors.branchName}
            >
              <MenuItem value={0}>-- Select Branch  --</MenuItem>
              {branchOption &&
                branchOption.map((branch: GroupDTO) => (
                  <MenuItem key={branch.id} value={branch.id}>
                    {branch.name}
                  </MenuItem>
                ))}
            </Select>
            {errors.branchName && (
              <Typography color="error" variant="caption">
                {errors.branchName}
              </Typography>
            )}
          </Grid>

          <Grid item xs={12} sm={6} mt={'22px'}>
            <TextField
              label="Branch Code"
              fullWidth
              value={formValues.exceptionItemRevs.branchCode}
              // onChange={(e) => handleInputChange('branchCode', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItem', 'branchCode', e.target.value)}
              InputLabelProps={{ shrink: true }}
              disabled
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              label="Division"
              fullWidth
              value={formValues.exceptionItemRevs.division}
              // onChange={(e) => handleInputChange('division', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItem', 'division', e.target.value)}
              InputLabelProps={{ shrink: true }}
              disabled
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              label="Area"
              fullWidth
              value={formValues.exceptionItemRevs.area}
              // onChange={(e) => handleInputChange('area', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItem', 'area', e.target.value)}
              InputLabelProps={{ shrink: true }}
              disabled
            />
          </Grid>

          <Grid item xs={12}>
            <Typography variant="subtitle1">Transaction Date</Typography>
            <TextField
              type="date"
              fullWidth
              value={formValues.exceptionItemRevs.transactionDate}
              // onChange={(e) => handleInputChange('transactionDate', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'transactionDate', e.target.value)}
              InputLabelProps={{
                shrink: true,
              }}
              error={!!errors.transactionDate}
              helperText={errors.transactionDate}
            />

          </Grid>
          <Grid item xs={12} sm={12}>
            <Typography variant="subtitle1">Transaction Type</Typography>
            <Select
              displayEmpty
              fullWidth
              value={formValues.exceptionItemRevs.type}
              // onChange={(e) => handleInputChange('transactionType', e.target.value)}
              // onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'type', e.target.value)}
              onChange={(e) => {
                handleTransactionTypeInputChange('exceptionItemRevs', 'type', e.target.value);
              }}
              error={!!errors.transactionType}
            >

              <MenuItem value={0}>-- Select Transaction --</MenuItem>
              {Object.entries(TransactionTypeDisplay).map(([key, label]) => (
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

          {formValues.exceptionItemRevs.type === 2 && (

            <Grid item xs={12} sm={12}>
              <Typography variant="subtitle1">Category</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItemRevs.nonMonetaryRevs?.type}
                onChange={(e) => handleDeepNestedInputChange('exceptionItemRevs', 'nonMonetaryRevs', 'type', e.target.value)}
                error={!!errors.transactionType}
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


          {formValues.exceptionItemRevs.type === 3 && (

            <Grid item xs={12} sm={12}>
              <Typography variant="subtitle1">Category</Typography>
              <Select
                displayEmpty
                fullWidth
                value={formValues.exceptionItemRevs.miscRevs?.type}
                onChange={(e) => handleDeepNestedInputChange('exceptionItemRevs', 'miscRevs', 'type', e.target.value)}
                error={!!errors.transactionType}
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



          <Grid item xs={12} sm={6}>
            <Typography variant="subtitle1">Root Cause</Typography>
            <Select
              displayEmpty
              fullWidth
              value={formValues.exceptionItemRevs.rootCause}
              // onChange={(e) => handleInputChange('rootCause', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'rootCause', e.target.value)}
              error={!!errors.rootCause}
            >
              <MenuItem value="">-- Select Root Cause --</MenuItem>



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
              value={formValues.exceptionItemRevs?.deviationApprover}
              disabled={formValues.exceptionItemRevs.rootCause === 1}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'deviationApprover', e.target.value)}
            // onChange={(e) => handleInputChange('exceptionApprover', e.target.value)}
            />
          </Grid>

          <Grid item xs={12} sm={12}>
            <Typography variant="subtitle1">Aging Category</Typography>
            <Select
              displayEmpty
              fullWidth
              value={formValues.exceptionItemRevs.agingCategory}
              // onChange={(e) => handleInputChange('agingCategory', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'agingCategory', e.target.value)}
              error={!!errors.agingCategory}
            >
              <MenuItem value="">-- Select Aging Category --</MenuItem>

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
          {/* <Grid item xs={12}>
            <TextField
              label="Aging Category"
              fullWidth
              value={formValues.agingCategory}
              onChange={(e) => handleInputChange('agingCategory', e.target.value)}
            />
          </Grid> */}
          <Grid item xs={12} sm={12}>
            <TextField
              label="Employee Responsible"
              fullWidth
              value={formValues.exceptionItemRevs.personResponsible}
              // onChange={(e) => handleInputChange('employeeResponsible', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'personResponsible', e.target.value)}
              error={!!errors.employeeResponsible}
              helperText={errors.employeeResponsible}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={12}>
            <TextField
              label="Other Employee's Responsible"
              fullWidth
              value={formValues.exceptionItemRevs.otherPersonResponsible}
              // onChange={(e) => handleInputChange('otherEmployeesResponsible', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'otherPersonResponsible', e.target.value)}
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              label="Remarks"
              multiline
              rows={4}
              fullWidth
              value={formValues.exceptionItemRevs.remarks}
              // onChange={(e) => handleInputChange('remarks', e.target.value)}
              onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'remarks', e.target.value)}
            />
          </Grid>

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Checkbox
                  checked={formValues.exceptionItemRevs.redFlag ?? false}
                  value={formValues.exceptionItemRevs.redFlag}
                  onChange={(e) => handleNestedInputChange('exceptionItemRevs', 'redFlag', e.target.checked)}
                />
              }
              label="Red Flag"
            />
          </Grid>



          <Grid item xs={12} sm={6}>
            <Button variant="outlined" fullWidth onClick={handleReset}>
              Reset
            </Button>
          </Grid>
          <Grid item xs={12} sm={6}>
            <Button variant="contained" color="success" disabled={isLoading} fullWidth onClick={handleSubmit}>
              {isLoading ? "Submitting..." : "Submit"}
            </Button>
          </Grid>
        </Grid>
      </Box>

      <Box width="50%" ml={10}>
        {/* <Typography variant="h5" mb={2} textAlign={'center'}>
          Information
        </Typography> */}

        {formValues.exceptionItemRevs?.type === 1 && (
          <MonetaryExceptionForm
            deviationOption={deviationOptions || []}
            selectedDeviation={selectedDeviation || []}
            onDeviationChange={handleDeviationChange}
            formValues={formValues.exceptionItemRevs?.monetaryRevs ?? ({} as MonetaryRev)}
            onInputChange={handleMonetaryInputChange}
          />
        )}


        {formValues.exceptionItemRevs.type === 2 && (
          <NonMonetaryExceptionForm
            deviationOption={deviationOptions || []}
            selectedDeviation={selectedDeviation || []}
            onDeviationChange={handleDeviationChange}
            formValues={formValues.exceptionItemRevs?.nonMonetaryRevs ?? ({} as NonMonetaryRev)}
            onInputChange={handleNonMonetaryInputChange}
          />
        )}

        {(formValues.exceptionItemRevs.type === 3 && formValues.exceptionItemRevs.miscRevs?.type)
          && (
            <MiscellaneousExceptionForm
              deviationOption={deviationOptions || []}
              selectedDeviation={selectedDeviation || []}
              onDeviationChange={handleDeviationChange}
              formValues={formValues.exceptionItemRevs?.miscRevs ?? ({} as MiscRev)}
              tranTypeCategory={formValues.exceptionItemRevs.miscRevs?.type}
              onInputChange={handleMiscInputChange}
            />
          )}

        <Divider />
      </Box>

    </Box>
  );
};

export default AddException;
