import React, { useState, useEffect } from "react";
import {
  Grid,
  Button,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField,
} from "@mui/material";
import {
  DailyCategory,
  WeeklyCategory,
  MonthlyCategory,
  GenerateMainReportsViewModel,
  DailyCategoryLabels,
  MonthlyCategoryLabels,
  WeeklyCategoryLabels,
  ReportCoverage,
  ReportCoverageLabels,
  DropdownReturn,
} from "../../../models/reportGenerateDTO";
import ReportGenerateService from "../../../services/ReportGenerateService";
import ToastService from "../../../utils/toast";

const GenerateMainReports: React.FC = () => {
  const [formData, setFormData] = useState<GenerateMainReportsViewModel>({
    dailyCategory: undefined,
    weeklyCategory: undefined,
    monthlyCategory: undefined,
    regularReportName: undefined,
    reportCoverage: 1,
    selectedBranches: [],
    dateCoverage: new Date(),
    dateFrom: new Date(),
    dateTo: new Date(),
  });

  const FormDataSelectOnChange = (
    event: SelectChangeEvent<
      | DailyCategory
      | WeeklyCategory
      | MonthlyCategory
      | ReportCoverage
      | string[]
      | null
    >
  ) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name as string]: value,
    }));
    setOpen(false); 
  };
  // const FormDataTextOnChange = (
  //   event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  // ) => {
  //   const { name, value } = event.target;
  //   setFormData((prevData) => ({
  //     ...prevData,
  //     [name as string]: value,
  //   }));
  // };

  const FormDataTextOnChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name as string]: value,
    }));
  };
  
  const [open, setOpen] = useState(false);
  const [dropdownOptions, setDropdownOptions] = useState<DropdownReturn[]>([]);

  const populateGroupsDropDownList = async () => {
    try {
      const response = await ReportGenerateService.populateGroupsDropDownList();

      // Assuming response.data is the array that you need
      const dropdownData: DropdownReturn[] = response.data.map((item: any) => ({
        value: item.value,
        text: item.text,
        isSelected: item.isSelected ?? false,
      }));

      setDropdownOptions(dropdownData);
    } catch (error) {
      console.error("Error fetching dropdown data:", error);
    }
  };

  useEffect(() => {
    populateGroupsDropDownList();
  }, []);
  const FormReset = () => {
    setFormData({
      dailyCategory: undefined,
      weeklyCategory: undefined,
      monthlyCategory: undefined,
      regularReportName: undefined,
      reportCoverage: 1,
      selectedBranches: [],
      dateCoverage: undefined,
      dateFrom: undefined,
      dateTo: undefined,
    });
  };
  const FormSubmit = async () => {
    const result = await ReportGenerateService.generateReport(formData);
    if (result.data.success) {
      ToastService.success(result.data.message);
    } else {
      ToastService.error(result.data.message);
    }
  };

  return (
    <>
      <h1 style={{ color: "#1976d2" }}>Reports Generation</h1>
      <Grid container spacing={2}>
        <Grid item xs={4} sm={4}>
          <InputLabel id="coverage-label">Coverage</InputLabel>
          <Select
            variant="outlined"
            fullWidth
            name="reportCoverage"
            value={formData.reportCoverage}
            onChange={FormDataSelectOnChange}
            renderValue={
              formData.reportCoverage ? undefined : () => "Select Coverage"
            }
          >
            {Object.values(ReportCoverage)
              .filter((value) => typeof value === "number")
              .map((status) => (
                <MenuItem key={status} value={status}>
                  {ReportCoverageLabels[status as ReportCoverage]}
                </MenuItem>
              ))}
          </Select>
        </Grid>
        <Grid item xs={6} sm={4}>
          {formData.reportCoverage === 1 && (
            <>
              <InputLabel id="daily-category-label">Category</InputLabel>
              <Select
                variant="outlined"
                fullWidth
                name="dailyCategory"
                value={formData.dailyCategory}
                onChange={FormDataSelectOnChange}
                renderValue={
                  formData.dailyCategory ? undefined : () => "Select Category"
                }
              >
                {Object.values(DailyCategory)
                  .filter((value) => typeof value === "number")
                  .map((status) => (
                    <MenuItem key={status} value={status}>
                      {DailyCategoryLabels[status as DailyCategory]}
                    </MenuItem>
                  ))}
              </Select>
            </>
          )}
          {formData.reportCoverage === 2 && (
            <>
              <InputLabel id="weekly-category-label">Category</InputLabel>
              <Select
                variant="outlined"
                fullWidth
                name="weeklyCategory"
                value={formData.weeklyCategory}
                onChange={FormDataSelectOnChange}
                renderValue={
                  formData.weeklyCategory ? undefined : () => "Select Category"
                }
              >
                {Object.values(WeeklyCategory)
                  .filter((value) => typeof value === "number")
                  .map((status) => (
                    <MenuItem key={status} value={status}>
                      {WeeklyCategoryLabels[status as WeeklyCategory]}
                    </MenuItem>
                  ))}
              </Select>
            </>
          )}
          {formData.reportCoverage === 3 && (
            <>
              <InputLabel id="monthly-category-label">Category</InputLabel>
              <Select
                variant="outlined"
                fullWidth
                name="monthlyCategory"
                value={formData.monthlyCategory}
                onChange={FormDataSelectOnChange}
                renderValue={
                  formData.monthlyCategory ? undefined : () => "Select Category"
                }
              >
                {Object.values(MonthlyCategory)
                  .filter((value) => typeof value === "number")
                  .map((status) => (
                    <MenuItem key={status} value={status}>
                      {MonthlyCategoryLabels[status as MonthlyCategory]}
                    </MenuItem>
                  ))}
              </Select>
            </>
          )}
        </Grid>
      </Grid>
      <Grid container spacing={2}>
        <Grid item xs={6} sm={4}>
          <InputLabel>Covered Branch</InputLabel>
          <Select
            variant="outlined"
            fullWidth
            name="selectedBranches"
            multiple
            value={formData.selectedBranches} // State to keep track of selected values
            onChange={FormDataSelectOnChange} // Handle the selection change
            open={open} // Control the open state of the dropdown
            onClose={() => setOpen(false)} // Close the dropdown when it loses focus
            onOpen={() => setOpen(true)} // Open the dropdown when it gains focus
          >
            {dropdownOptions.map((option: DropdownReturn) => (
              <MenuItem key={option.value} value={option.value}>
                {option.text}
              </MenuItem>
            ))}
          </Select>
        </Grid>
        
        {formData.reportCoverage === 1 && (
          <Grid item xs={4} sm={3}>
            <InputLabel>Date</InputLabel>
            <TextField
              variant="outlined"
              autoComplete="off"
              fullWidth
              type="date"
              name="dateFrom"
              value={formData.dateFrom || ""}
              onChange={FormDataTextOnChange}
            />
          </Grid>
        )}
        {formData.reportCoverage !== 1 && (
          <>
            <Grid item xs={4} sm={3}>
              <InputLabel>Date From</InputLabel>
              <TextField
                variant="outlined"
                autoComplete="off"
                fullWidth
                type="date"
                name="dateFrom"
                value={formData.dateFrom || ""}
                onChange={FormDataTextOnChange}
              />
            </Grid>
            <Grid item xs={4} sm={3}>
              <InputLabel>Date To</InputLabel>
              <TextField
                variant="outlined"
                autoComplete="off"
                fullWidth
                type="date"
                name="dateTo"
                value={formData.dateTo || ""}
                onChange={FormDataTextOnChange}
              />
            </Grid>
          </>
        )}
      </Grid>
      <Grid container spacing={1}>
        <Grid item xs={3} sm={2}>
          <Button
            type="button"
            onClick={FormReset}
            variant="contained"
            color="success"
            size="large"
            fullWidth
          >
            Reset
          </Button>
        </Grid>
        <Grid item xs={3} sm={2}>
          <Button
            type="button"
            onClick={FormSubmit}
            variant="contained"
            color="success"
            size="large"
            fullWidth
          >
            Generate
          </Button>
        </Grid>
      </Grid>
    </>
  );
};

export default GenerateMainReports;
