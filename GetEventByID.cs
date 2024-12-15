using System; // data types+functionality
using System.Collections.Generic; // collections
using System.Linq; // LINQ
using System.Net.Http; // HTTP request builder
using System.Threading.Tasks; // allows for wait
using Microsoft.AspNetCore.Mvc; // provides IActionResult
using Microsoft.Azure.WebJobs; // Azure function trigger
using Microsoft.Azure.WebJobs.Extensions.Http; // HTTP trigger
using Microsoft.AspNetCore.Http; // HTTP handling
using Microsoft.Extensions.Logging; // logging
using Newtonsoft.Json; // json deserialization

public static class GetEventByID
{
    // client init
    private static readonly HttpClient _httpClient = new HttpClient();

    // define the function and set it up to retrieve the information 
    [FunctionName("GetEventById")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "events/{id}")] HttpRequest req, string id, ILogger log){
        log.LogInformation($"Attempting to search for Event: {id}");

        const string ApiUrl = "https://iir-interview-homework-ddbrefhkdkcgdpbs.eastus2-01.azurewebsites.net/api/v1.0/event-data";

        Event eventData = null;
        int attempts = 0;

        // 5 time retry limit
        while (attempts < 5){
            try{
                // connect to API and fetch data
                var response = await _httpClient.GetStringAsync(ApiUrl);

                if (string.IsNullOrWhiteSpace(response)){
                    log.LogWarning("Null or empty response received from API.");
                    continue;
                }

                // try to deserialize the JSON into a series of parseable objects
                List<Event> eventList = null;
                try{
                    eventList = JsonConvert.DeserializeObject<List<Event>>(response);
                }
                catch (JsonException jsonEx){
                    log.LogError($"JSON deserialization failed: {jsonEx.Message}");
                    continue;
                }

                // check if received list is null
                if (eventList == null || eventList.Count == 0){
                    log.LogWarning("Empty list received.");
                    continue;
                }

                // find the event ID in the request message
                if (int.TryParse(id, out var eventId)){
                    log.LogInformation($"Searching for Event ID: {eventId}");

                    // used LINQ functionality to parse for the event ID
                    eventData = eventList.FirstOrDefault(e => e.Id == eventId);

                    if (eventData != null){
                        log.LogInformation($"Event found: {eventData.Name}");
                        // once the event is found, break the loop since we don't need to search anymore
                        break;
                    }
                    else{
                        log.LogWarning($"No matching event found for ID: {eventId}");
                    }
                }
                else{
                    log.LogWarning($"Invalid ID format: {id}. Unable to parse to integer.");
                }
            }
            catch (Exception ex){
                // if error, log it to the console and which attempt it was
                log.LogError($"Attempt {attempts + 1}: Error fetching data from API - {ex.Message}");
            }

            attempts++;
            // not necessary in the instructions but added a slight delay between queries to prevent overloading the API
            await Task.Delay(1000);
        }

        // if event data is null, 500 code returned and error message logged
        if (eventData == null){
            log.LogError($"Failed to retrieve event after {attempts} attempts.");
            return new StatusCodeResult(500);
        }

        // even duration, whole number
        int days = (eventData.DateEnd - eventData.DateStart).Days;

        // result object to be returned to client
        var result = new{
            name = eventData.Name,
            days = days,
            websiteUrl = eventData.Url
        };

        // 200
        return new OkObjectResult(result);
    }

    // strongly typed event class data model
    public class Event{
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("program")]
        public string Program { get; set; }

        [JsonProperty("dateStart")]
        public DateTime DateStart { get; set; }

        [JsonProperty("dateEnd")]
        public DateTime DateEnd { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }
    }
}
