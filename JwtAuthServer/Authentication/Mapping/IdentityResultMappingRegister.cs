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
            config.NewConfig<IdentityResult, IdentityRolesResponse>()
                .ConstructUsing(
                    x => x.Errors.Any()
                        ? new IdentityRolesResponse(x.Errors.Select(e => e.Adapt<ResponseError>()).ToArray())
                        : new IdentityRolesResponse());
            config.NewConfig<IdentityResult, AddUserToRolesResponse>()
                .ConstructUsing(
                    x => x.Errors.Any()
                        ? new AddUserToRolesResponse(x.Errors.Select(e => e.Adapt<ResponseError>()).ToArray())
                        : new AddUserToRolesResponse());
        }
    }
}
