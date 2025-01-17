export interface PaginatedList {
  pageIndex: number;
  totalPages: number;
  countData: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  data: Report[];
}


export interface DownloadAdhocViewModel {
    dateFrom: string; // ISO string representation of DateTime
    dateTo: string;   // ISO string representation of DateTime
    reportAdhoc: ReportAdhoc;
    rt: RegularizationTAT;
    pr: Pervasiveness;
    ea: ExceptionAdhocs;
    [key: string]: any;
}
export interface RegularizationTAT {
    coveredBranch: number;
    employeeID: string;
}
export interface Pervasiveness {
    employeeID: string;
}
export interface ExceptionAdhocs {
    exceptionStatus: AdhocStatus;
}
export enum AdhocStatus {
    Regularized = 4,      // "Tagged as Regularized"
    ForCompliance = 5,    // "Tagged as For Compliance"
    Deleted = 6,
    Dispensed = 7         // "Tagged as Dispensed"
}
export enum ReportAdhoc {
    Pervasiveness = 1,
    RegularizationTAT = 2, // "Regularization TAT"
    AuditTrail = 3,        // "Audit Trail"
    ExceptionAdhocs = 4    // "Exception Adhocs"
}
// Helper for ReportAdhoc display names
export const ReportAdhocDisplayNames: Record<ReportAdhoc, string> = {
    [ReportAdhoc.Pervasiveness]: "Pervasiveness",
    [ReportAdhoc.RegularizationTAT]: "Regularization TAT",
    [ReportAdhoc.AuditTrail]: "Audit Trail",
    [ReportAdhoc.ExceptionAdhocs]: "Exception Adhocs",
};
// Helper for ReportAdhoc display names
export const AdhocStatusDisplayNames: Record<AdhocStatus, string> = {
    [AdhocStatus.Regularized]: "Tagged as Regularized",
    [AdhocStatus.ForCompliance]: "Tagged as For Compliance",
    [AdhocStatus.Deleted]: "Tagged as Deleted",
    [AdhocStatus.Dispensed]: "Tagged as Dispensed",
};


export interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
  }


export interface Report {
    id: number;
    fileName?: string;
    path?: string;
    actionPlan?: string;
    createdBy?: string;
    dateGenerated: Date;
    dateSent: Date;
    actionPlanCreated: Date;
    status: number;
    branchCodeRecipient?: string;
    sendingSchedule: Date;
    reportCoverage: number;
    reportCategory: number;
    coverageDate: Date;
    selectedBranches?: string;
    toRecipients?: string;
    ccRecipients?: string;
    reportsGuid: string; // Guid is represented as a string in TypeScript
    approvalRemarks?: string;
    reportContents: ReportContent[]; // Virtual ICollection
    toList: string[]; // [NotMapped]
    ccList: string[]; // [NotMapped]
    statusName: string; // [NotMapped]
    reportCategoryName: string; // [NotMapped]
    reportCoverageName: string; // [NotMapped]
  }
  
  export interface ReportContent {
    id: string; // Guid
    reportId: number;
    exceptionNo?: string;
    branchCode?: string;
    branchName?: string;
    area?: string;
    division?: string;
    transactionDate?: string;
    aging?: string;
    agingCategory?: string;
    process?: string;
    accountNo?: string;
    accountName?: string;
    deviation?: string;
    riskClassification?: string;
    deviationCategory?: string;
    amount?: number; // Decimal
    personResponsible?: string;
    otherPersonResponsible?: string;
    remarks?: string;
    actionPlan?: string;
    encodedBy?: string;
    rootCause?: string;
    deviationApprover?: string;
    report: Report;
  }
  