using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenHackCo.Function
{
    public static class GetRatingFunc
    {
        [FunctionName("GetRatingFunc")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "serverless-oh",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Query.ratingId}",
                PartitionKey = "cc20a6fb-a91f-4192-874d-132493685376")] RatingModel ratings,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            String responseMessage;
            if (ratings == null)
            {
                responseMessage = "No items found for that rating ID";
                return new NotFoundObjectResult(responseMessage);
            }
            else
            {
                responseMessage = JsonConvert.SerializeObject(ratings);
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
