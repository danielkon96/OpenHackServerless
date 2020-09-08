using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace OpenHackCo.Function
{
    public static class CreateRatingFunc
    {
        [FunctionName("CreateRatingFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "serverless-oh",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<RatingModel> toDoItemsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function CreateRatingFunc processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string id = Guid.NewGuid().ToString();
            string timestamp = DateTime.Now.ToString();
            string userId = data?.userId;
            string productId = data?.productId;
            string locationName = data?.locationName;
            int rating = data?.rating;
            string userNotes = data?.userNotes;

            string responseMessage = string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(locationName)
                ? "Please provide all required fields."
                : $"If you are seeing this message, This function has successfully created a rating with no errors.";
            
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(locationName)) 
            {
                return new NotFoundObjectResult(responseMessage);
            }

            if (rating < 0 || rating > 5) 
            {
                responseMessage = "Invalid rating, please provide a valid rating and try again.";
                return new NotFoundObjectResult(responseMessage);
            }

            // Both API calls will throw exceptions if Product or User IDs are invalid.
            try 
            {
                var userIdRequest = (HttpWebRequest)WebRequest.Create($"https://serverlessohuser.trafficmanager.net/api/GetUser?userId={userId}");
                var response = (HttpWebResponse)userIdRequest.GetResponse();
                var productIdRequest = (HttpWebRequest)WebRequest.Create($"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={productId}");
                var response2 = (HttpWebResponse)productIdRequest.GetResponse();
            }
            catch
            {
                responseMessage = "Invalid product or user IDs. Please check them and try again.";
                return new NotFoundObjectResult(responseMessage);
            }

            RatingModel newRating = new RatingModel();
            newRating.id = id;
            newRating.userId = userId;
            newRating.productId = productId;
            newRating.timestamp = timestamp;
            newRating.locationName = locationName;
            newRating.rating = rating;
            newRating.userNotes = userNotes;

            await toDoItemsOut.AddAsync(newRating);

            return new OkObjectResult(responseMessage);
        }
    }
}
