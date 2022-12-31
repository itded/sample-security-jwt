using JwtAuthServer.Authentication.Entities;
using JwtAuthServer.Authentication.Models;
using Mapster;

namespace JwtAuthServer.Authentication.Mapping
{
    public class AppUserMappingRegister: IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AppUser, UserInfo>();
        }
    }
}
