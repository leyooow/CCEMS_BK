/* eslint-disable @typescript-eslint/no-explicit-any */
export interface ExceptionDTO {

  employeeID: string;
  employeeName: string;
  exceptionItem: ExceptionItemDTO | null;
  exceptionItemRevs: ExceptionItemRevDTO;
  selectedExCodes: number[];
  actionPlans: ActionPlans[] | null;
  actionPlan: ActionPlan;
  subExceptionItems: SubExceptionItemDTO[];
  hasPendingUpdate: boolean;
  hasFormChanges: boolean;
  request: string;
  approvalRemarks: string;
  


  }

export interface SubExceptionsListViewDTO {
  id: number;
  subReferenceNo: string;
  exItemRefNo: string;
  dateCreated: string;
  approvalStatus: number;
  deviationStatus: number;
  exCode: number;
  exCodeDescription: string;
  deviationCategory: string;
  riskClassification: string;
  request: string | null;
}


// export interface ExceptionCodeDTO {
//   id: number;
//   subReferenceNo: string;
//   exCode: number;
//   exItemRefNo: string;
//   deviationStatus: number;
//   dateCreated: string;
//   approvalStatus: number;
//   entryDate: string;
//   approvalRemarks: string;
//   taggingDate: string;
//   exItemRefNoNavigation: string;
// }

export interface MiscDTO {
  id: number;
  category: number;
  cardNo: string;
  bankCertNo: string;
  glslaccountNo: string;
  glslaccountName: string;
  dpafno: string;
  checkNo: string;
  amount: number;
  exceptionId: string;
  refNo: string;
  refNoNavigation: string;
}

export interface ActionPlan {
  id?: number;
  category?: number;
  cifnumber?: string;
  customerName?: string;
  customerAccountNo?: string;
  [key: string]: any; 
}

export interface ExceptionCode {
  id: number;
  code: string;
  description: string;
  [key: string]: any;
}

export interface ExceptionDetailsDTO{
  actionPlan: any
  actionPlans: any
  approvalRemarks: any
  employeeID: any
  employeeName: any
  exceptionItemRevs: ExceptionItemRevDTO
  hasFormChanges: boolean
  hasPendingUpdate: boolean
  request: any
  selectedExCodes: any
  exceptionItem: ExceptionItemDTO
  subExceptionItems: SubExceptionsListViewDTO[]
}

export interface ExceptionCodeRevsDTO {
  actionTaken: string | null;
  approvalRemarks: string | null;
  approvalStatus: number;
  approvedBy: string | null;
  approvedDateTime: string | null;
  changes: string;
  dateCreated: string;
  deviationStatus: number;
  entryDate: string;
  exCode: number;
  exCodeDescription: string | null;
  exItem: string | null;
  exItemRefNo: string;
  id: number;
  isProcessed: boolean;
  modifiedBy: string;
  modifiedDateTime: string;
  subReferenceNo: string;
  taggingDate: string;
}

// Interface for Miscellaneous Details
export interface Misc {
  id: number;
  category?: number | null;
  cardNo?: string | null;
  bankCertNo?: string | null;
  glslaccountNo?: string | null;
  glslaccountName?: string | null;
  dpafno?: string | null;
  checkNo?: string | null;
  amount: number;
  exceptionId: string; // Guid represented as a string in TypeScript
  refNo?: string | null;
}

// Interface for Monetary Details
export interface Monetary {
  id: number;
  sequenceNo: string;
  bdstellerId: string;
  amount: number;
  transCode: string;
  transDescription: string;
  creditAccountNo: string;
  creditAccountName: string;
  debitAccountNo: string;
  debitAccountName: string;
  exceptionId: string;
  refNo: string;
  currency: number;
  refNoNavigation: string;
}

// export interface NonMonetaryDTO {
//   id: number;
//   category: number;
//   cifnumber: string;
//   customerName: string;
//   customerAccountNo: string;
//   exceptionId: string;
//   refNo: string;
//   refNoNavigation: string;
// }

export interface ActionPlans {
  id?: string;
  exceptionItemRefNo?: string;
  reportId?: number;
  createdBy?: string;
  actionPlan1?: string;
  dateCreated?: string;
  type?: string;
  exceptionItemRevsId?: string;
}

export interface SubExceptionItemDTO {
  id: number;
  subReferenceNo: string;
  exCode: number;
  exItemRefNo: string;
  deviationStatus: number;
  approvalStatus: number;
  dateCreated: string;
  exCodeDescription: string;
  deviationCategory: string;
  riskClassification: string;
  request: string;
}

export interface SelectedExCodeDTO {
  selectedExCodes: number[];
}

export interface SingleActionPlanDTO {
  id: string;
  exceptionItemRefNo: string;
  reportId: number;
  createdBy: string;
  actionPlan: string;
  dateCreated: string;
  type: string;
}

export interface ExceptionCodeDTO {
  id: number;
  subReferenceNo: string;
  exCode: number;
  exItemRefNo: string;
  deviationStatus: number;
  dateCreated: string; // ISO string format for dates
  approvalStatus: number;
  entryDate: string; // ISO string format for dates
  approvalRemarks: string;
  taggingDate: string; // ISO string format for dates
  exItemRefNoNavigation: string;
  transCode?: string | null;
  transDescription?: string | null;
  creditAccountNo?: string | null;
  creditAccountName?: string | null;
  debitAccountNo?: string | null;
  debitAccountName?: string | null;
  exceptionId: string; // Guid represented as a string
  refNo?: string | null;
  currency?: number | null;
}

// Interface for Non-Monetary Details
export interface NonMonetary {
  id: number;
  category?: number | null;
  cifnumber: string;
  customerName: string;
  customerAccountNo?: string | null;
  exceptionId: string; // Guid represented as a string
  refNo?: string | null;
}

// Main Exception Item DTO
export interface ExceptionItemDTO {
  actionPlans: ActionPlan[] | null;
  age: number;
  agingCategory?: any;
  approvalRemarks?: string | null;
  area: string;
  branchCode: string;
  branchName: string;
  createdBy: string;
  dateCreated: string; // ISO date string
  deviationApprovedBy: string | null;
  deviationApprover: string | null;
  deviationCategoryId: number;
  division: string;
  employeeId: string;
  entryDate: string; // ISO date string
  exceptionCodes: any[];
  id: string;
  miscs: Misc[]; 
  monetaries: Monetary[];
  nonMonetaries: NonMonetary[];
  otherPersonResponsible: string;
  otherRemarks: string | null;
  personResponsible: string;
  redFlag: boolean | false;
  refNo: string;
  remarks: string | null;
  riskClassificationId: number;
  rootCause: number;
  severity: number | null;
  status: number;
  transactionDate: string; // ISO date string
  type?: number | null;
}

export interface ExceptionItemRevDTO {
  changes?: string | null;
  modifiedBy?: string | null;
  modifiedDateTime?: Date | null;
  approvedBy?: string | null;
  approvedDateTime?: Date | null;
  actionTaken?: string | null;
  isProcessed: boolean;
  id: string | null; // Guid
  refNo?: string | null;
  employeeId?: string | null;
  branchCode?: string | null;
  branchName?: string | null;
  personResponsible?: string | null;
  otherPersonResponsible?: string | null;
  severity?: number | null;
  deviationApprovedBy?: string | null;
  remarks?: string | null;
  redFlag?: boolean | null;
  transactionDate?: string | null;
  dateCreated?: Date | null;
  createdBy?: string | null;
  status?: number | null;
  type?: number | null;
  monetaryRevsId?: number | null;
  nonMonetaryRevsId?: number | null;
  miscRevsId?: number | null;
  age?: number | null;
  agingCategory?: number | null;
  deviationApprover?: string | null;
  deviationCategoryId?: number | null;
  riskClassificationId?: number | null;
  rootCause?: number | null;
  division?: string | null;
  area?: string | null;
  entryDate?: Date | null;
  approvalRemarks?: string | null;
  otherRemarks?: string | null;
  exceptionCodeRevs?: ExceptionCodeRev[] | null; 
  miscRevs?: MiscRev | null;
  monetaryRevs?: MonetaryRev | null; 
  nonMonetaryRevs?: NonMonetaryRev | null;
  actionPlans?: ActionPlan[] | null; 
  isCredit?: boolean | null;
}

export interface MiscRev {
  id?: number;
  exceptionId?: string; // Guid
  type?: number | null;
  cardNo?: string | null;
  bankCertNo?: string | null;
  glslaccountNo?: string | null;
  glslaccountName?: string | null;
  dpafno?: string | null;
  checkNo?: string | null;
  amount?: number;
  refNo?: string | null;
}

export interface MonetaryRev {
  id: number;
  sequenceNo: string;
  bdstellerId: string;
  amount: number;
  transCode?: string | null;
  transDescription?: string | null;
  creditAccountNo?: string | null;
  creditAccountName?: string | null;
  debitAccountNo?: string | null;
  debitAccountName?: string | null;
  exceptionId: string; // Guid
  refNo?: string | null;
  currency?: number | null;
}

export interface NonMonetaryRev {
  id?: number;
  type?: number | null;
  cifnumber?: string;
  customerName?: string;
  customerAccountNo?: string | null;
  exceptionId?: string; // Guid
  refNo?: string | null;
}

export interface ExceptionCodeRev {
  id: number;
  subReferenceNo?: string | null;
  exCode: number;
  exItemId?: string | null; // Guid
  exItemRefNo?: string | null;
  deviationStatus: number;
  actionTaken?: string | null;
  approvedBy?: string | null;
  approvedDateTime?: Date | null;
  changes?: string | null;
  isProcessed: boolean;
  modifiedBy?: string | null;
  modifiedDateTime?: Date | null;
  remarks: string;
  dateCreated?: Date | null;
  approvalStatus?: number | null;
  entryDate?: Date | null;
  approvalRemarks?: string | null;
  taggingDate: Date;
}


export interface DeviationDTO {
  id: number; 
  classification?: string | null; 
  category?: string | null; 
  deviation1?: string | null; 
  riskClassification?: string | null; 
}

export enum RootCause {
  EmployeeLapses = 1,
  BusinessDecision = 2,
}

export const RootCauseDisplay: Record<RootCause, string> = {
  [RootCause.EmployeeLapses]: "Employee Lapse",
  [RootCause.BusinessDecision]: "Business Decision",
};

export function getRootCauseDisplayName(rootCause: RootCause): string {
  return RootCauseDisplay[rootCause];
}

export enum AgingCategory {
  LessEqual7Days = 0,
  LessEqual15Days = 1,
  LessEqual30Days = 2,
  LessEqual45Days = 3,
  LessEqual180Days = 4,
  LessEqual1Year = 5,
  LessEqual2Year = 6,
  LessEqual3Year = 7,
  LessEqual4Year = 8,
  LessEqual5Year = 9
}

export const AgingCategoryDisplay: Record<AgingCategory, string> = {
  [AgingCategory.LessEqual7Days]: "≤ 7D banking days",
  [AgingCategory.LessEqual15Days]: "≤ 15D banking days",
  [AgingCategory.LessEqual30Days]: "≤ 30D banking days",
  [AgingCategory.LessEqual45Days]: "≤ 45D banking days",
  [AgingCategory.LessEqual180Days]: "≤ 180D banking days",
  [AgingCategory.LessEqual1Year]: "≤ 1Y (251 banking days)",
  [AgingCategory.LessEqual2Year]: "≤ 2Y (2 x 251 banking days)",
  [AgingCategory.LessEqual3Year]: "≤ 3Y (3 x 251 banking days)",
  [AgingCategory.LessEqual4Year]: "≤ 4Y (4 x 251 banking days)",
  [AgingCategory.LessEqual5Year]: "≤ 5Y (5 x 251 banking days)"
};

export function getAgingCategoryDisplayName(category: AgingCategory): string {
  return AgingCategoryDisplay[category];
}

export enum TransactionType {
  Monetary = 1,
  NonMonetary = 2,
  Miscellaneous = 3
}

export const TransactionTypeDisplay: Record<TransactionType, string> = {
  [TransactionType.Monetary]: "Monetary",
  [TransactionType.NonMonetary]: "Non Monetary",
  [TransactionType.Miscellaneous]: "Miscellaneous",
};

export function getTransactionTypeDisplayName(category: TransactionType): string {
  return TransactionTypeDisplay[category];
}

export enum NonMonetaryTypes {
  CIFCreation = 1,
  CIFMaintenance = 2,
  AccountOpening = 3,
  ReactivatedDormant = 4
}

export const NonMonetaryTypesDisplay: Record<NonMonetaryTypes, string> = {
  [NonMonetaryTypes.CIFCreation]: "CIF Creation",
  [NonMonetaryTypes.CIFMaintenance]: "CIF Maintenance",
  [NonMonetaryTypes.AccountOpening]: "Account Opening",
  [NonMonetaryTypes.ReactivatedDormant]: "Reactivated Dormant"
};


export enum ExceptionCategory {
  EMV = 1,
  BankCertification = 2,
  GeneralLedger = 3,
  DepositPickupAuthorizationForm = 4,
  DueFromLocalBanks = 5,
  BDSReports = 6,
  OtherClearingDeficiencies = 7,
  Checkbook = 8,
  NewAccountTagging = 9,
  SurpriseCount = 10,
  OfficialReceipt = 11
}

export const ExceptionCategoryDisplay: Record<ExceptionCategory, string> = {
  [ExceptionCategory.EMV]: "EMV",
  [ExceptionCategory.BankCertification]: "Bank Certification",
  [ExceptionCategory.GeneralLedger]: "General Ledger",
  [ExceptionCategory.DepositPickupAuthorizationForm]: "Deposit Pick-up Authorization Form",
  [ExceptionCategory.DueFromLocalBanks]: "Due From Local Banks",
  [ExceptionCategory.BDSReports]: "BDS Reports",
  [ExceptionCategory.OtherClearingDeficiencies]: "Other Clearing Deficiencies",
  [ExceptionCategory.Checkbook]: "Checkbook",
  [ExceptionCategory.NewAccountTagging]: "New Account Tagging",
  [ExceptionCategory.SurpriseCount]: "Surprise Count",
  [ExceptionCategory.OfficialReceipt]: "Official Receipt"
};

export enum Currency {
  PHP = 1,
  USD = 2,
  EUR = 3,
  YEN = 4
}

export const CurrencyDisplay: Record<Currency, string> = {
  [Currency.PHP]: "PHP",
  [Currency.USD]: "USD",
  [Currency.EUR]: "EUR",
  [Currency.YEN]: "YEN"
};

export function getCurrencyDisplayName(currency: Currency): string {
  return CurrencyDisplay[currency];
}




export function getExceptionCategoryDisplayName(category: ExceptionCategory): string {
  return ExceptionCategoryDisplay[category];
}

export function getNonMonetaryTypesDisplayName(type: NonMonetaryTypes): string {
  return NonMonetaryTypesDisplay[type];

}
