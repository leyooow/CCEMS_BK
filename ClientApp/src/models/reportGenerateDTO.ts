export enum ReportCoverage {
    Daily = 1,
    Weekly = 2,
    Monthly = 3
    }
export enum DailyCategory {
    DailyExceptionReport = 10, 
    RedFlag = 20, 
}
  
export enum WeeklyCategory {
    Escalation = 30, 
    NewAccounts = 40, 
}
  
  export enum MonthlyCategory {
    AllOutstanding1 = 50, 
    AllOutstanding2 = 60, 
  }
  
  export enum RegularReports {
    DailyExceptionReport = 10, 
    RedFlagReport = 20, 
    EscalationReport = 30, 
    NewAccountsReport = 40, 
    AllOutstandingReport1 = 50, 
    AllOutstandingReport2 = 60, 
  }
  
  // Validation Interfaces
  export interface FutureDatedValidation {
    // Implementation or placeholder for future-dated validation
  }
  
  export interface DateRangeValidation {
    // Implementation or placeholder for date-range validation
  }

  // GenerateMainReportsViewModel Interface
  export interface GenerateMainReportsViewModel {
    dailyCategory: DailyCategory | null | undefined;
    weeklyCategory: WeeklyCategory | null | undefined;
    monthlyCategory: MonthlyCategory | null | undefined;
    regularReportName: RegularReports | null | undefined;
    reportCoverage: ReportCoverage | null | undefined;
    selectedBranches: string[];
    dateCoverage: Date | null | undefined; // Includes [FutureDatedValidation] and [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    dateFrom: Date | null | undefined; // Includes [DateRangeValidation] and [FutureDatedValidation]
    dateTo: Date | null | undefined; // Includes [DateRangeValidation] and [FutureDatedValidation]
  }
  export interface DropdownReturn {
    value: string;
    text: string;
    isSelected: boolean;
  }

export const ReportCoverageLabels: Record<ReportCoverage, string> = {
    [ReportCoverage.Daily]: "Daily",
    [ReportCoverage.Weekly]: "Weekly",
    [ReportCoverage.Monthly]: "Monthly",
};

// DailyCategory Display Names
export const DailyCategoryLabels: Record<DailyCategory, string> = {
    [DailyCategory.DailyExceptionReport]: "Daily Exception Report",
    [DailyCategory.RedFlag]: "Red Flag Report",
  };
  
  // WeeklyCategory Display Names
  export const WeeklyCategoryLabels: Record<WeeklyCategory, string> = {
    [WeeklyCategory.Escalation]: "Escalation Report",
    [WeeklyCategory.NewAccounts]: "Weekly New Accounts Report",
  };
  
  // MonthlyCategory Display Names
  export const MonthlyCategoryLabels: Record<MonthlyCategory, string> = {
    [MonthlyCategory.AllOutstanding1]: "All Outstanding Exceptions (Monetary & Misc)",
    [MonthlyCategory.AllOutstanding2]: "All Outstanding Exceptions (Non-Monetary)",
  };
  
  // RegularReports Display Names
  export const RegularReportsLabels: Record<RegularReports, string> = {
    [RegularReports.DailyExceptionReport]: "Daily Exception Report",
    [RegularReports.NewAccountsReport]: "Weekly New Accounts Report",
    [RegularReports.RedFlagReport]: "Red Flag Report",
    [RegularReports.AllOutstandingReport1]: "All Outstanding Exception Report (Monetary & Misc)",
    [RegularReports.AllOutstandingReport2]: "All Outstanding Exception Report (Non-Monetary)",
    [RegularReports.EscalationReport]: "Escalation Report",
  };
  
  
    