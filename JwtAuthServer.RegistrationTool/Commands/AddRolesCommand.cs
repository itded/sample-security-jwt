using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JwtAuthServer.Authentication.Models;
using JwtAuthServer.Authentication.Services;
using JwtAuthServer.RegistrationTool.Models;
using Microsoft.Extensions.Logging;

namespace JwtAuthServer.RegistrationTool.Commands
{
    public class AddRolesCommand : IRegistrationCommand
    {
        private const string CmdName = "AddRolesCommand";

        private readonly IAppRoleService _roleService;
        private readonly ILogger<AddRolesCommand> _logger;
        private readonly FileInfo _fileInfo;

        public AddRolesCommand(IAppRoleService roleService, ILogger<AddRolesCommand> logger, FileInfo fileInfo)
        {
            _roleService = roleService;
            _logger = logger;
            _fileInfo = fileInfo;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var addUserModels = ParseInputFile();

            var identityRolesRequest = new IdentityRolesRequest()
            {
                IdentityRoles = addUserModels.Select(x => new IdentityRolesRequest.IdentityRole()
                {
                    RoleName = x.RoleName,
                    Description = x.Description
                }).ToArray(),
                ContinueOnError = false
            };

            var result = await _roleService.AddIdentityRolesAsync(identityRolesRequest);

            if (result == null || result.Succeeded)
            {
                _logger.LogInformation($"{CmdName} - execution completed");
            }
            else
            {
                var errors = string.Join("\\n", result.PrintErrors());
                _logger.LogError($"{CmdName} - execution failed with errors:\\n{errors}");
            }
        }

        private IList<AddRoleModel> ParseInputFile()
        {
            var addRoleModels = new List<AddRoleModel>();

            using var stream = _fileInfo.OpenRead();
            var document = XDocument.Load(stream);
            foreach (var roleElement in document.Root.Elements())
            {
                var name = roleElement.Attribute("name")?.Value;
                var description = roleElement.Attribute("description")?.Value;

                var addRoleModel = new AddRoleModel()
                {
                    RoleName = name,
                    Description = description
                };

                addRoleModels.Add(addRoleModel);
            }

            return addRoleModels;
        }
    }
}
