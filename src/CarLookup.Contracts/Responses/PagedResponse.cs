using System.Collections.Generic;
using System.Linq;

namespace CarLookup.Contracts.Responses;

/// <summary>
/// Paginated response wrapper
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public PagedResponse()
    {
        Success = true;
        Message = "Successful";
        Data = Enumerable.Empty<T>();
    }
}