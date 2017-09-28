using StudioX.Authorization.Users;

namespace StudioX.Zero.SampleApp.Users
{
    public class User : StudioXUser<User>
    {
        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}