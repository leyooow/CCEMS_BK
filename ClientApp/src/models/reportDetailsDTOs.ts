export interface ReportDetailsDTO {
  id: number;
  fileName: string;
  path: string;
  actionPlan: string;
  createdBy: string;
  dateGenerated?: string;
  dateSent: string;
  actionPlanCreated: string;
  status: number;
  statusName: string;
  branchCodeRecipient: string | null;
  sendingSchedule: string;
  reportCoverage: number;
  reportCoverageName: string;
  reportCategory: number;
  reportCategoryName: string;
  coverageDate: string;
  selectedBranches: string;
  toRecipients: string;
  ccrecipients: string;
  reportsGuid: string;
  approvalRemarks: string;
  reportContents: any[];
  toList: string[];
  ccList: string[] | null;
}

export enum ReportStatus {
  Standby = 0,
  PendingApproval = 1,
  Approved = 2,
  Sent = 3,
  Closed = 4,
  Rejected = 5,
}

export interface PaginatedList {
  pageIndex: number;
  totalPages: number;
  countData: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  data: ReportContent[];
}

interface ReportContent {
  id: string; // Guid in C# is equivalent to a string in TypeScript
  reportId: number; // int in C# maps to number in TypeScript
  exceptionNo?: string; // Nullable string in C# is just an optional string in TypeScript
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
  amount?: number; // Nullable decimal in C# maps to an optional number in TypeScript
  personResponsible?: string;
  otherPersonResponsible?: string;
  remarks?: string;
  actionPlan?: string;
  encodedBy?: string;
  rootCause?: string;
  deviationApprover?: string;
  report: Report; // Assuming the Report type is defined somewhere in your code
}


export interface RecipientDTO {
  text: string;       // Example: "20190221"
  value: string;      // Example: "YSH TRISTAN I MECHILINA <resimbulan@bankcom.com.ph>"
  isSelected: boolean; // Example: false
}

export interface RejectedDTO {
  id: number;
  reportsGuid: string;
  remarks: string;
}

export interface SendRequestDTO{
  ToList: string[];
  CCList: string[];
  id: number;
}