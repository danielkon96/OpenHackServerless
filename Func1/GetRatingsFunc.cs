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
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace OpenHackCo.Function
{
    public static class GetRatingsFunc
    {
        [FunctionName("GetRatingsFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "serverless-oh",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];
            log.LogInformation("userId: " + userId);

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;

            if (string.IsNullOrEmpty(userId)) 
            {
                return new NotFoundObjectResult("No items found for the User ID.");
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "serverless-oh", collectionId: "Ratings");
            IDocumentQuery<RatingModel> query = client.CreateDocumentQuery<RatingModel>(collectionUri)
                .Where(Ratings => Ratings.userId == userId)
                .AsDocumentQuery();

            var ratingsList = new List<RatingModel>();
            while (query.HasMoreResults)
            {
                foreach (RatingModel rating in await query.ExecuteNextAsync())
                {
                    log.LogInformation("loop in here");
                    ratingsList.Add(rating);
                }
            }     

            return new OkObjectResult(JsonConvert.SerializeObject(ratingsList));
        }
    }
}
