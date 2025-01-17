export interface RoleDTO {
    id: number;
    roleName?: string;
    description?: string;
    rolePermissions: RolePermissionDTO[];
    users: UserDTO[];
}

export interface GenericResponse {
    success: boolean;
    message: string;
    statusCode: number;
    data: RoleDTO[];
}

export interface RolePermissionDTO {
    id: number;
    permissionName?: string;
}

export interface UserDTO {
    id: number;
    username?: string;
}
