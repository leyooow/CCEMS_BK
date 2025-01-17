// Enum for DeviationStatusDTO
export enum DeviationStatusDTO {
    Outstanding = 2,
    Regularized = 4,
    ForCompliance = 5,
    Dispensed = 7,
}

// Interface for ExceptionCode
export interface ExceptionCode {
    id: number;
    subReferenceNo?: string;
    exCode: number;
    exItemRefNo?: string;
    exCodeDescription: string
    deviationStatus: number;
    dateCreated: string; // Date is represented as string in JSON format
    approvalStatus?: number;
    entryDate?: string; // Date is represented as string in JSON format
    approvalRemarks?: string;
    taggingDate?: string; // Date is represented as string in JSON format
    taggingDateDisplay?: string;
}

export interface SubExceptionsDetailsDTO {
    exceptionCode: ExceptionCode;
    newStatus: DeviationStatusDTO;
    remarks: string;
    riskClassification: string;
    deviationCategory: string;
    taggingDate: string; // Date is represented as string in JSON format
    taggingDateDisplay?: string;
    branchReplyDetails: string[];
    forDeletion: number
}


export interface DeviationStatusUpdate {
    subRefNo: string;
    newStatus: DeviationStatusDTO;
    taggingDate: string; // Optional DateTime field
    exItemRefNo: string;
}


// Display names for the enum
export const DeviationStatusDisplayNames: { [key in DeviationStatusDTO]: string } = {
[DeviationStatusDTO.Outstanding]: "Outstanding",
[DeviationStatusDTO.Regularized]: "Tag as Regularized",
[DeviationStatusDTO.ForCompliance]: "Tag as Compliance",
[DeviationStatusDTO.Dispensed]: "Tag as Dispensed",
};