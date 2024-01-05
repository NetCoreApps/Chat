using ServiceStack;
using Chat.ServiceModel;

namespace Chat.ServiceInterface;

[Authenticate]
public class UserDetailsService : Service
{
    public object Get(GetUserDetails request)
    {
        var session = GetSession();
        return session.ConvertTo<GetUserDetailsResponse>();
    }
}
