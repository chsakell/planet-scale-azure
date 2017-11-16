using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Online.Store.Azure.Services
{
    public class SearchAppServices
    {
        private readonly Uri _serviceUri;
        private HttpClient _httpClient;
        public string errorMessage;
        private const string productIndexName = "product-index";
        private const string communityIndexName = "posts";
        private const string suggesterName = "product-suggester";

        public SearchAppServices(string searchServiceName, string searchServiceApiKey)
        {
            try
            {
                _serviceUri = new Uri("https://" + searchServiceName + ".search.windows.net");
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("api-key", searchServiceApiKey);
            }
            catch (Exception e)
            {
                errorMessage = e.Message.ToString();
            }
        }

        public dynamic SearchProduct(string searchText, string sort)
        {
            string search = "&search=" + Uri.EscapeDataString(searchText);
            //string facets = "&facet=title&facet=description";
            string paging = "&$top=100";
            string select = "$select[id,title,model,sku,description]";
            string filter = BuildFilter();
            string orderby = BuildSort(sort);

            Uri uri = new Uri(_serviceUri, $"/indexes/{productIndexName}/docs?$count=true{search}{select}{paging}{orderby}");
            HttpResponseMessage response = SearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            SearchServices.EnsureSuccessfulSearchResponse(response);

            return SearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        }

        public dynamic SearchPost(string searchText, string sort)
        {
            string search = "&search=" + Uri.EscapeDataString(searchText);
            string facets = "&facet=title&facet=content";
            string paging = "&$top=100";
            string select = "$select[postId,title,userId,content,mediaDescription]";
            string filter = BuildFilter();
            string orderby = BuildSort(sort);

            Uri uri = new Uri(_serviceUri, $"/indexes/{communityIndexName}/docs?$count=true{search}{select}{facets}{paging}{orderby}");
            HttpResponseMessage response = SearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            SearchServices.EnsureSuccessfulSearchResponse(response);

            return SearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        }

        private string BuildSort(string sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return string.Empty;
            }

            throw new Exception("Invalid sort order");
        }

        private string BuildFilter()
        {
            // carefully escape and combine input for filters, injection attacks that are typical in SQL
            // also apply here. No "DROP TABLE" risk, but a well injected "or" can cause unwanted disclosure

            string filter = "&$filter=discontinuedDate eq null";

            return filter;
        }

        //    return AzureSearchHelper.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        //}

        public dynamic Suggest(string searchText)
        {
            // we still need a default filter to exclude discontinued products from the suggestions
            Uri uri = new Uri(_serviceUri, $"/indexes/{productIndexName}/docs/suggest?$select=title&suggesterName={suggesterName}&search={Uri.EscapeDataString(searchText)}");
            HttpResponseMessage response = SearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            SearchServices.EnsureSuccessfulSearchResponse(response);

            return SearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        }
        private string EscapeODataString(string s)
        {
            return Uri.EscapeDataString(s).Replace("\'", "\'\'");
        }
    }
    public class SearchServices
    {
        public const string ApiVersionString = "api-version=2016-09-01";

        private static readonly JsonSerializerSettings _jsonSettings;

        static SearchServices()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // for readability, change to None for compactness
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            _jsonSettings.Converters.Add(new StringEnumConverter());
        }

        public static string SerializeJson(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSettings);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }

        public static HttpResponseMessage SendSearchRequest(HttpClient client, HttpMethod method, Uri uri, string json = null)
        {
            UriBuilder builder = new UriBuilder(uri);
            string separator = string.IsNullOrWhiteSpace(builder.Query) ? string.Empty : "&";
            builder.Query = builder.Query.TrimStart('?') + separator + ApiVersionString;

            var request = new HttpRequestMessage(method, builder.Uri);

            if (json != null)
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return client.SendAsync(request).Result;
        }

        public static void EnsureSuccessfulSearchResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = response.Content == null ? null : response.Content.ReadAsStringAsync().Result;
                throw new Exception("Search request failed: " + error);
            }
        }
    }
}
