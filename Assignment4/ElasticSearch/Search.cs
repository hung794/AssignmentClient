using Assignment4.Models;
using Elasticsearch.Net;
using Nest;

namespace Assignment4.ElasticSearch
{

    public class Search
    {
        private static ElasticClient searchClient;
        private static string indexName = "articles";
        private static string user = "elastic";
        private static string password = "WCIOTg3vUSbRh6RFOgjmPup5";
        private static string cloudId = "ArticleDb:YXNpYS1zb3V0aGVhc3QxLmdjcC5lbGFzdGljLWNsb3VkLmNvbSRlNWNiMzViOTRhZGY0NTY3OTUyNmZlODIyODVlOWYwZCQ4ZDZjZmFiNzA5NzY0ZmU4OWY3NjgzNGMzZTUwNThjZg==";
        public static ElasticClient EsClient()
        {
            if(searchClient == null)
            {
                var settings = new ConnectionSettings(cloudId,
                    new BasicAuthenticationCredentials(user, password)).DefaultIndex("example-index").DefaultMappingFor<Article>(i => i.IndexName(indexName));
                searchClient = new ElasticClient(settings);
            };
            return searchClient;
        }
    }
}