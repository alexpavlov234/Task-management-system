using Accuweather.Core;
using Accuweather.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Security.Policy;
using System.Threading.Tasks;
using Task_management_system.Interfaces;
using Task_management_system.Models;

/// <summary>
///	Accuweather Current Conditions Api
///	This class implements the ICurrentConditions interface and is a wrapper for the Accuweather API for current conditions.
/// </summary>
public class CurrentConditions : AccuweatherApiCore, ICurrentConditions
{
    // The base URL for the Accuweather current conditions API
    private const string _baseUrl = "http://dataservice.accuweather.com/currentconditions/v1/";

    /// <summary>
    /// Constructor for CurrentConditions class
    /// </summary>
    /// <param name="apiKey">Api key accuweather.</param>
    /// <param name="language">Language.</param>
    public CurrentConditions(string apiKey, string language = "en-us") : base(apiKey, language)
    {
        // Empty constructor as the base constructor is called to initialize the apiKey and language variables.
    }

    /// <summary>
    /// Get the current conditions for a specific location
    /// </summary>
    /// <param name="locationKey">The key of the location to get the current conditions for</param>
    /// <param name="details">Whether or not to include additional details in the response</param>
    /// <returns>A string representation of the JSON response from the API</returns>
    public async Task<string> Get(int locationKey, bool details = false)
    {
        // Create an object to be passed as query parameters in the API request
        var obj = new
        {
            language = _language,
            details
        };

        // Form the URL for the API request by appending the locationKey and API key to the base URL
        var url = $"{_baseUrl}{locationKey}?apikey={_apiKey}&";

        // Make the API request, URL encode the parameters, and return the JSON response
        return await SendGetRequest(UrlEncodeHelper.UrlEncode(obj, url));
    }

    /// <summary>
    /// Gets the top cities for the specified group number.
    /// </summary>
    /// <param name="group">The group number to retrieve top cities for.</param>
    /// <returns>A string representation of the JSON response from the API.</returns>
    public async Task<string> GetTopCities(int group)
    {
        // Create an object to be passed as query parameters in the API request
        var obj = new
        {
            language = _language
        };

        // Form the URL for the API request by appending the locationKey and API key to the base URL
        var url = $"{_baseUrl}topcities/{group}?apikey={_apiKey}&";

        // Make the API request, URL encode the parameters, and return the JSON response
        return System.Text.RegularExpressions.Regex.Unescape(await SendGetRequest(UrlEncodeHelper.UrlEncode(obj, url)));
    }

    /// <summary>
    /// Gets the historical 24-hour weather conditions for the specified location.
    /// </summary>
    /// <param name="locationKey">The location key of the location to retrieve weather conditions for.</param>
    /// <param name="details">Whether to retrieve detailed information about the conditions.</param>
    /// <returns>A string representation of the JSON response from the API.</returns>
    public async Task<string> GetHistorical24Hours(int locationKey, bool details = false)
    {
        // Create an object to be passed as query parameters in the API request
        var obj = new
        {
            language = _language,
            details
        };

        // Form the URL for the API request by appending the locationKey and API key to the base URL
        var url = $"{_baseUrl}{locationKey}/historical/24?apikey={_apiKey}&";

        // Make the API request, URL encode the parameters, and return the JSON response
        return System.Text.RegularExpressions.Regex.Unescape(await SendGetRequest(UrlEncodeHelper.UrlEncode(obj, url)));
    }

    /// <summary>
    /// Gets the historical 6-hour weather conditions for the specified location.
    /// </summary>
    /// <param name="locationKey">The location key of the location to retrieve weather conditions for.</param>
    /// <param name="details">Whether to retrieve detailed information about the conditions.</param>
    /// <returns>A string representation of the JSON response from the API.</returns>
    public async Task<string> GetHistorical6Hours(int locationKey, bool details = false)
    {
        // Create an object to be passed as query parameters in the API request
        var obj = new
        {
            language = _language,
            details
        };

        // Form the URL for the API request by appending the locationKey and API key to the base URL
        var url = $"{_baseUrl}{locationKey}/historical?apikey={_apiKey}&";

        // Make the API request, URL encode the parameters, and return the JSON response
        return System.Text.RegularExpressions.Regex.Unescape(await SendGetRequest(UrlEncodeHelper.UrlEncode(obj, url)));
    }

    public Response ConvertToResponseModel(string json)
    {
        return JsonConvert.DeserializeObject<Response>(json);
    }

    public IList<CurrentCondition> ConvertToCurrentConditionModel(string json)
    {
        return JsonConvert.DeserializeObject<List<CurrentCondition>>(json);
    }

    public CurrentCondition ConvertData(string json)
    {
        Response response = ConvertToResponseModel(json);
        CurrentCondition currentCondition = ConvertToCurrentConditionModel(response.Data).FirstOrDefault();
        return currentCondition;
    }
}
