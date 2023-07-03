namespace WeatherApi.Data
{
    public class UserProvider
    {
        private Dictionary<string, UserData> _users = new Dictionary<string, UserData>();

        public UserProvider()
        {
            AddUsers();
        }

        public UserData GetUser(string username, string password)
        {
            UserData result;
            if (!_users.TryGetValue(username, out result))
                return null;
            else
                return result;
        }
        private void AddUsers()
        {
            var u1 = new UserData
            {
                Id = 1,
                Email = "user@example.com",
                Password = "password",
                UserName = "login"
            };

            _users[u1.UserName] = u1;

            var u2 = new UserData
            {
                Id = 2,
                Email = "other@example.com",
                Password = "be-good",
                UserName = "other"
            };

            _users[u2.UserName] = u2;
        }
    }
}
