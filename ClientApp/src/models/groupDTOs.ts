export interface GroupDTO {
  id: number;
  code: string;
  name: string;
  description?: string;
  dateCreated: Date;
  dateModified?: Date | null;
  createdBy?: string;
  area?: string;
  division?: string;
}



// export interface PagedResult {
//   items: GroupDTO[];    // Array of GroupDTO (list of groups)
//   totalCount: number;   // Total number of groups
//   pageNumber: number;   // Current page number
//   pageSize: number;     // Number of groups per page
//   searchTerm: string;   // Search term used (if any)
// }

export interface GroupCreateDTO {
  code: string,
  name: string,
  area: string,
  division: string,
}

export interface GroupUpdateDTO {
  id: number;
  name: string;
  code : string;
  area : string;
  division : string;
  // Other properties for updating a group
}


export interface BranchOption {
  brCode: string;
  brName: string;

}