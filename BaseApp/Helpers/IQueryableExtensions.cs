using BaseApp.DTO;
using Microsoft.EntityFrameworkCore;

namespace BaseApp.Helpers
{
    public static class IQueryableExtensions
    {
        public static async Task<ResponsePaginated<T>> ToPaginatedResponseAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
        {
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new ResponsePaginated<T>(items, pageNumber, pageSize, totalRecords);
        }
    }
}
