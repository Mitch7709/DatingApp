using System;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class PaginatedResults<T>
{
    public PaginationMetaData MetaData { get; set; } = default!;  
    public List<T> Items { get; set; } = [];  
};

public class PaginationMetaData
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
};

public class PaginationHelper
{
    public static async Task<PaginatedResults<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResults<T>
        {
            MetaData = new PaginationMetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            },
            Items = items
        };
    }
}