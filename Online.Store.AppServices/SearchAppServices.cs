using System;
using System.Net.Http;

namespace Online.Store.AppServices
{
    public class SearchAppServices
    {
        private readonly Uri _serviceUri;
        private HttpClient _httpClient;
        public string errorMessage;
        private const string productIndexName = "products";
        private const string communityIndexName = "posts";
        private const string suggesterName = "wheel";

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
            string select = "$select[id,title,description,url]";
            string filter = BuildFilter();
            string orderby = BuildSort(sort);

            Uri uri = new Uri(_serviceUri, $"/indexes/{productIndexName}/docs?$count=true{search}{select}{paging}{orderby}");
            HttpResponseMessage response = AzureSearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            AzureSearchServices.EnsureSuccessfulSearchResponse(response);

            return AzureSearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
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
            HttpResponseMessage response = AzureSearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            AzureSearchServices.EnsureSuccessfulSearchResponse(response);

            return AzureSearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
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
            HttpResponseMessage response = AzureSearchServices.SendSearchRequest(_httpClient, HttpMethod.Get, uri);
            AzureSearchServices.EnsureSuccessfulSearchResponse(response);

            return AzureSearchServices.DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        }
        private string EscapeODataString(string s)
        {
            return Uri.EscapeDataString(s).Replace("\'", "\'\'");
        }
    }
}
