using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Toluwaloope.FunctionApp.Api;

public class GetCallerInformation
{
    private readonly ILogger<GetCallerInformation> _logger;

    public GetCallerInformation(ILogger<GetCallerInformation> logger)
    {
        _logger = logger;
    }

    [Function("GetCallerInformation")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = "";

            var geoLocation = new GeoLocation();

            try
            {   

                var headers = String.Empty;
                foreach (var key in req.Headers)
                headers += key.ToString() + "=" + req.Headers[key].ToString() + Environment.NewLine.ToString();
                _logger.LogInformation($"Request Headers: {headers}");

                string ip = req.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0];
                 _logger.LogInformation($"Client IP: {ip} from X-Forwarded-For");


                if (string.IsNullOrEmpty(ip) || ip == "127.0.0.1" || ip == "::1" || ip == "localhost")
                {
                    ip = req.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                }

                _logger.LogInformation($"Client IP: {ip} from HttpContext");

                if (string.IsNullOrEmpty(ip))
                {
                    responseMessage= "Yawa don gas o, system is unable to detect your IP";
                }


                var location = await geoLocation.GetGeoLocationAsync(ip);

                if (location == null)
                {
                    responseMessage = String.Format("Looks like you're not on the surface of the earth. {0} could not detect your location", nameof(GetCallerInformation));
                }
                else
                {   
                    responseMessage = String.Format("Eau De Play!! {0} detected your ip to be {1} and location to be {2}, {3} {4} at Lat: {5}, Long: {6}", nameof(GetCallerInformation), location.Query, location.City, location.RegionName, location.Country, location.Lat, location.Lon);

                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

        
        return new OkObjectResult(responseMessage);

    }

}
