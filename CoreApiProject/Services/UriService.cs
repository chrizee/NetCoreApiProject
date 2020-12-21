using CoreApiProject.Contracts.V1;
using CoreApiProject.Domain;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Services
{
    public class UriService : IUriService
    {
        private readonly string _BaseUri;

        public UriService(string baseUri)
        {
            _BaseUri = baseUri;
        }

        public Uri GetAllPostsUri(PaginationFilter paginationFilter = null)
        {
            if (paginationFilter is null) return new Uri(_BaseUri);
            var modifiedUri = QueryHelpers.AddQueryString(_BaseUri, "pageNumber", paginationFilter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationFilter.PageSize.ToString());
            return new Uri(modifiedUri);
        }

        public Uri GetPostUri(string postId)
        {
            return new Uri(_BaseUri + ApiRoutes.Posts.Get.Replace("{postId}", postId));
        }
    }
}
