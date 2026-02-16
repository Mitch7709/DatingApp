export type Pagination = {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalCount: number;
}

export type PaginatedResult<T> = {
    items: T[];
    metaData: Pagination;
}