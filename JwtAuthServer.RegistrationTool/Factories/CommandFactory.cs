using System;
using System.IO;
using System.Linq;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.RegistrationTool.Commands;
using JwtAuthServer.RegistrationTool.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.RegistrationTool.Factories
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRegistrationCommand CreateRegistrationCommand(string[] args)
        {
            var installRoles = args[0].ToLower() == Constants.AddRolesCommand;
            if (installRoles)
            {
                var cmdArgs = args.Skip(1).ToArray();
                return CreateAddRolesCommand(cmdArgs);
            }
            
            var installUsers = args[0].ToLower() == Constants.AddUsersCommand;
            if (installUsers)
            {
                var cmdArgs = args.Skip(1).ToArray();
                return CreateAddUsersCommand(cmdArgs);
            }

            throw new InvalidOperationException();
        }

        private IRegistrationCommand CreateAddUsersCommand(string[] args)
        {
            var fileName = args[0];
            var fileInfo = new FileInfo(fileName);

            var appUserService = _serviceProvider.GetRequiredService<IAppUserService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<AddUsersCommand>>();
            return new AddUsersCommand(appUserService, logger, fileInfo);
        }

        private IRegistrationCommand CreateAddRolesCommand(string[] args)
        {
            var fileName = args[0];
            var fileInfo = new FileInfo(fileName);

            var appRoleService = _serviceProvider.GetRequiredService<IAppRoleService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<AddRolesCommand>>();
            return new AddRolesCommand(appRoleService, logger, fileInfo);
        }
    }
}
