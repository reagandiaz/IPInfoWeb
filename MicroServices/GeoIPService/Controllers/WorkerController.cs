using System;
using System.IO;
using System.Net;
using System.Text.Json;
using GeoIPService.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoIPService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        [HttpGet]
        public Result Get(string ip)
        {
            var result = new Result() { ip = ip };

            System.Net.IPAddress address;
            if (!System.Net.IPAddress.TryParse(ip, out address))
            {
                if (Uri.CheckHostName(ip) == UriHostNameType.Unknown)
                {
                    result.info = "Invalid ip/domain";
                    result.state = "Error";
                    result.ts = DateTime.Now;
                    return result;
                }
            }

            LoadResult(result);

            result.ts = DateTime.Now;
            return result;
        }

        void LoadResult(Result result)
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string responseFromServer = null;
            try
            {
                WebRequest request = WebRequest.Create(string.Format(
                "http://api.ipstack.com/{0}?access_key=7ac0e3b02aacd365123419fa5fcf8078&format=1", result.ip));
                // Get the response.
                response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                // Display the content.
                result.message = JsonSerializer.Deserialize<Message>(responseFromServer);
                result.state = "Complete";
                result.info = "Found";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The JSON value could not be converted to"))
                {
                    result.info = responseFromServer.Contains("\"type\":null") ? "Not Found" : responseFromServer;
                    result.message = new Message() { location = new Location() };
                    result.state = "Complete";
                }
                else
                {
                    result.info = ex.Message;
                    result.message = new Message() { location = new Location() };
                    result.state = "Error";
                }
            }
            finally
            {
                // Cleanup the streams and the response.
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
                if (response != null) response.Close();
            }
        }
    }
}
