namespace WeatherApi.Data
{
    public class ApiKeyClientProvider
    {
        private Dictionary<string, ApiKeyClient> _clients = new Dictionary<string, ApiKeyClient>();
        public ApiKeyClientProvider()
        {
            AddClients();
        }

        public ApiKeyClient GetClient(string key)
        {
            ApiKeyClient result; ;

            if (_clients.TryGetValue(key, out result))
                return result;
            else
                return null;
        }

        private void AddClients()
        {
            var client = new ApiKeyClient()
            {
                ApiKey = "klucz-1",
                Email = "client1@example.com",
                Id = 1,
                Name = "Klient 1"
            };

            _clients[client.ApiKey] = client;

            var client2 = new ApiKeyClient()
            {
                ApiKey = "klucz-2",
                Email = "client2@example.com",
                Id = 2,
                Name = "Klient 2"
            };

            _clients[client2.ApiKey] = client2;
        }
    }
}
