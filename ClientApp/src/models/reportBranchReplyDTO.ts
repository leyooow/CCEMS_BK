import { Report } from './reportDTO';

export  interface BranchReplyViewModel {
  ActionPlan: string;
  ReportContentsId: string;
}
export interface RouteParams {
  [key: string]: string | undefined; // Add an index signature
}
export interface PaginatedList {
  pageIndex: number;
  totalPages: number;
  countData: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  data: ReportContent[];
}

export interface ReportContent {
  id: string; // Guid in C# maps to string in TypeScript
  reportId: number;
  exceptionNo?: string; // Nullable string in C#
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
  amount?: number; // Nullable decimal in C# maps to number or undefined
  personResponsible?: string;
  otherPersonResponsible?: string;
  remarks?: string;
  actionPlan?: string;
  encodedBy?: string;
  rootCause?: string;
  deviationApprover?: string;
  report: Report; // Assuming `Report` is another TypeScript interface
}

export interface BranchReplyTable {
  id: string;
  reportContentsId?: string;
  createdBy?: string;
  actionPlan?: string;
  dateCreated?: Date;
  exceptionNo?: string;
  isHighlight: boolean;
}

