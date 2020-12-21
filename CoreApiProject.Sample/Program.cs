using CoreApiProject.Sdk;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //var count = stockPairs(new List<int> { 5, 7, 3, 9, 13, 11, 6, 6, 3, 3 }, 12);
            var x = weightCapacity(new List<int> { 1, 3, 5 }, 7);
            Console.ReadLine();

            var cachedToken = string.Empty;
            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");

            var postApi = RestService.For<ICoreApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });

            var registerResponse = await identityApi.RegisterAsync(new Contracts.V1.Requests.UserRegistrationRequest
            {
                Email = "sdk@gmail.com",
                Password = "P@ssw0rd"
            });

            var loginResponse = await identityApi.LoginAsync(new Contracts.V1.Requests.UserLoginRequest
            {
                Email = "sdk@gmail.com",
                Password = "P@ssw0rd"
            });

            cachedToken = loginResponse.Content.Token;

            var allPosts = await postApi.GetAllAsync();

            var createdPost = await postApi.CreateAsync(new Contracts.V1.Requests.CreatePostRequest
            {
                Name = "Created from SDK",
                Tags = new[] { "SDK" }
            });

            var retrievedPost = await postApi.GetAsync(createdPost.Content.Id);

            var updatedPost = await postApi.UpdateAsync(createdPost.Content.Id, new Contracts.V1.Requests.UpdatePostRequest
            {
                Name = "Updated from SDK"
            });

            var deletedPost = await postApi.DeleteAsync(createdPost.Content.Id);
        }

        public static int stockPairs(List<int> stocksProfit, long target)
        {
            var sums = new List<Tuple<int, int>>();
            int a,  b;
            for(int x = 0; x <= stocksProfit.Count; x++)
            {
                for (int y = x + 1; y <= stocksProfit.Count - 1; y++)
                {
                    a = stocksProfit[x]; b = stocksProfit[y];
                    if (a + b == target)
                    {
                        if (sums.Contains(Tuple.Create(a, b)) || sums.Contains(Tuple.Create(b, a)))
                            continue;
                        sums.Add(Tuple.Create(stocksProfit[x], stocksProfit[y]));
                    }
                }                                  
            }
            return sums.Distinct().Count();
        }

        public static int weightCapacity(List<int> weights, int maxCapacity)
        {           
            int comboCount = (int)Math.Pow(2, weights.Count) - 1;
            List<List<int>> result = new List<List<int>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                // make each combo here
                result.Add(new List<int>());
                for (int j = 0; j < weights.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(weights[j]);
                }
            }            

            int max = 0;
            foreach (var item in result)
            {
                var sum2 = item.Sum();
                if (sum2 > max && sum2 < maxCapacity) max = sum2;
            }
            return max;
        }


        public static List<int> getUnallottedUsers(List<List<int>> bids, int totalShares)
        {
            var unallottedUsers = bids.Select(x => x[0]).ToList();

            bids = bids.OrderByDescending(x => x[2]).ThenBy(x => x[3]).ToList();
            while (totalShares > 0)
            {
                foreach (var item in bids)
                {
                    if (item[2] < totalShares)
                    {
                        totalShares -= item[2];
                        unallottedUsers.RemoveAll(x => x == item[0]);
                    }
                }
            }

            return unallottedUsers;
        }

        public static int weightCapacity2(List<int> weights, int maxCapacity)
        {
            int max = 0;
            var combo = Math.Pow(2, weights.Count) - 1;
            List<List<int>> result = new List<List<int>>();
            for (int i = 1; i < combo + 1; i++)
            {
                result.Add(new List<int>());
                for (int j = 0; j < weights.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(weights[j]);
                }
            }

            foreach (var item in result)
            {
                var sum2 = item.Sum();
                if (sum2 > max && sum2 < maxCapacity)
                    max = sum2;
            }
            return max;
        }


        

        
    }
}
