export interface PaginatedGenericResponse<T> {
  success: boolean;
  message: string;
  statusCode: number;
  data: T[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  searchTerm: string;
}