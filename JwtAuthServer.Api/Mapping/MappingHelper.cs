using Mapster;

namespace JwtAuthServer.Api.Mapping
{
    public static class MappingHelper
    {
        public static void ConfigureControllersMapping()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Apply(new AuthenticationMappingRegister());
        }
    }
}