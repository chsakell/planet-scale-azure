using Online.Store.ElasticScale;
using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class ProductsAppService
    {
        private readonly string _elasticScaleDb;
        private readonly string _serverName;
        private readonly string _databaseEdition;
        private readonly string _elasticDbUsername;
        private readonly string _elasticDbPassword;
        private readonly string _integratedSecurity;

        public ProductsAppService(
            string elasticScaleDb,
            string serverName,
            string databaseEdition,
            string elasticDbUsername,
            string elasticDbPassword,
            string integratedSecurity)
        {
            _elasticScaleDb = elasticScaleDb;
            _serverName = serverName;
            _databaseEdition = databaseEdition;
            _elasticDbUsername = elasticDbUsername;
            _elasticDbPassword = elasticDbPassword;
            _integratedSecurity = integratedSecurity;
        }

        public void CheckOut(CartDTO cart)
        {
            var azureElasticService = new AzureElasticScaleServices(
                _elasticScaleDb,
                _serverName,
                _databaseEdition,
                _elasticDbUsername,
                _elasticDbPassword,
                _integratedSecurity);
            azureElasticService.InsertData(cart);
        }
    }
}
