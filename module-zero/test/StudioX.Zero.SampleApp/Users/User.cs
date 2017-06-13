using StudioX.Authorization.Users;

namespace StudioX.Zero.SampleApp.Users
{
    public class User : StudioXUser<User>
    {
        public override string ToString()
        {
            return string.Format("[User {0}] {1}", Id, UserName);
        }
    }
}