using JwtAuthServer.Authentication.Models;
using Mapster;

namespace JwtAuthServer.Api.Mapping
{
    public class AuthenticationMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserLoginResponse, JwtAuth.Common.Models.UserLoginResponse>()
                .Map(dest => dest.UserName,
                    src => src.UserInfo.UserName)
                .Map(dest => dest.FirstName,
                    src => src.UserInfo.FirstName)
                .Map(dest => dest.LastName,
                    src => src.UserInfo.LastName)
                .Map(dest => dest.Email,
                    src => src.UserInfo.Email);
        }
    }
}