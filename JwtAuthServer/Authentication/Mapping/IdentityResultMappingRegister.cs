using System.Linq;
using JwtAuthServer.Authentication.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthServer.Authentication.Mapping
{
    public class IdentityResultMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdentityError, ResponseError>();
            config.NewConfig<IdentityResult, UserRegisterResponse>()
                .ConstructUsing(
                    x => x.Errors.Any()
                        ? new UserRegisterResponse(x.Errors.Select(e => e.Adapt<ResponseError>()).ToArray())
                        : new UserRegisterResponse());
        }
    }
}
