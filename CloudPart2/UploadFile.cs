﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;

namespace CloudPart2
{
    public static class UploadFile
    {
        [Function("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        { 
            string shareName = req.Query["shareName"];
            string fileName = req.Query["fileName"];

            using var stream = req.Body;
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
            var shareServiceClient = new ShareServiceClient(connectionString);
            var shareClient = shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);

            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}
