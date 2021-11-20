using System.Threading;
using System.Threading.Tasks;

namespace JwtAuthServer.RegistrationTool.Commands
{
    public interface IRegistrationCommand
    {
        Task ExecuteAsync(CancellationToken token);
    }
}
