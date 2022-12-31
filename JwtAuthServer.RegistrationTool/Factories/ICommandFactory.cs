using JwtAuthServer.RegistrationTool.Commands;

namespace JwtAuthServer.RegistrationTool.Factories
{
    public interface ICommandFactory
    {
        IRegistrationCommand CreateRegistrationCommand(string[] args);
    }
}
