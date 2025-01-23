namespace AccessRefreshToken.Model.UserModel
{
    public class ListUser
    {
        public List<User> ListUserPaging()
        {
            var _users = new List<User>()
            {
                new User{Id = Guid.NewGuid().ToString(), UserName = "Admin", Password = "Password" },
                new User{Id = Guid.NewGuid().ToString(), UserName = "username", Password = "password"}
            };
            return _users;
        }
    }
}
