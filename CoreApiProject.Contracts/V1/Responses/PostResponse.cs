using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }       

        public virtual IEnumerable<TagResponse> Tags { get; set; }
    }
}
