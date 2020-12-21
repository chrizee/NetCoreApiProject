using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Contracts.V1.Responses
{
    public class AuthsuccessResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
