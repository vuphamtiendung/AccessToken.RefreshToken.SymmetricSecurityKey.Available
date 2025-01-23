using AccessRefreshToken.Model.UserModel;

namespace AccessRefreshToken.Domain.Common
{
    public class ValidateUser : IValidateUser
    {
        private readonly ListUser _listUser;
        public ValidateUser(ListUser listUser)
        {
            _listUser = listUser;
        }

        public string UserId(string userId)
        {
            var id = _listUser.ListUserPaging().FirstOrDefault(s => s.Id == userId);
            return id.ToString();
        }

        User IValidateUser.ValidateUser(string userName, string password)
        {
            var user = _listUser.ListUserPaging()                         
                                     .FirstOrDefault(s => s.UserName == userName && s.Password == password);
            return user;
        }
    }
}
