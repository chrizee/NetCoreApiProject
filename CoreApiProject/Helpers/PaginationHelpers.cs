using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Domain;
using CoreApiProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Helpers
{
    public class PaginationHelpers
    {
        internal static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter paginationFilter, List<T> postResponse)
        {
            var nextPageUri = paginationFilter.PageNumber >= 1 ? uriService.GetAllPostsUri(new PaginationFilter { PageNumber = paginationFilter.PageNumber + 1, PageSize = paginationFilter.PageSize }).ToString() : null;
            var previousPageUri = paginationFilter.PageNumber - 1 >= 1 ? uriService.GetAllPostsUri(new PaginationFilter { PageNumber = paginationFilter.PageNumber - 1, PageSize = paginationFilter.PageSize }).ToString() : null;


            return new PagedResponse<T>
            {
                Data = postResponse,
                PageNumber = paginationFilter.PageNumber >= 1 ? paginationFilter.PageNumber : (int?)null,
                PageSize = paginationFilter.PageSize >= 1 ? paginationFilter.PageSize : (int?)null,
                NextPage = postResponse.Any() ? nextPageUri : null,
                PreviousPage = previousPageUri
            };
        }
    }
}
