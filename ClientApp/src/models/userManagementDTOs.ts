export interface UserDTO {
  employeeId: string;
  firstName: string;
  middleName: string;
  lastName: string;
}
export interface BranchAccessDTO {
  employeeId: string;
  branchId: number;
  usersLoginName: string;
}

export interface UserCreateDto {
  loginName: string;
  employeeId: string;
  firstName: string;
  middleName: string;
  lastName: string;
  email: string;
  roleId: number;
  branchAccesses: BranchAccessDTO[];
}
export interface UserUpdateDto {
  loginName: string;
  employeeId: string;
  firstName: string;
  status?: number;
  statusText?: number;
  remarks?: string;
  middleName: string;
  lastName: string;
  email: string;
  UserGroup?: string;
  roleId: number;
  branchAccessIds: number[];
  branchAccesses: BranchAccessDTO[];
}



