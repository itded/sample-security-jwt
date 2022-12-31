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
    public class AddUsersCommand : IRegistrationCommand
    {
        private const string CmdName = "AddUsersCommand";

        private readonly IAppUserService _userService;
        private readonly ILogger<AddUsersCommand> _logger;
        private readonly FileInfo _fileInfo;

        public AddUsersCommand(IAppUserService userService,  ILogger<AddUsersCommand> logger, FileInfo fileInfo)
        {
            _userService = userService;
            _logger = logger;
            _fileInfo = fileInfo;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var addUserModels = ParseInputFile();

            ResponseBase result = null;
            foreach (var addUserModel in addUserModels)
            {
                var userRegisterRequest = new UserRegisterRequest()
                {
                    UserName = addUserModel.UserName,
                    FirstName = addUserModel.FirstName,
                    LastName = addUserModel.LastName,
                    Email = addUserModel.Email,
                    Password = addUserModel.Password
                };

                result = await _userService.RegisterUserAsync(userRegisterRequest);

                if (!result.Succeeded)
                {
                    break;
                }

                var addUserToRolesRequest = new AddUserToRolesRequest()
                {
                    UserName = addUserModel.UserName,
                    RoleNames = addUserModel.Roles
                };

                result = await _userService.AddUserToRolesAsync(addUserToRolesRequest);
                if (!result.Succeeded)
                {
                    break;
                }
            }

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

        private IList<AddUserModel> ParseInputFile()
        {
            var addUserModels = new List<AddUserModel>();

            using var stream = _fileInfo.OpenRead();
            var document = XDocument.Load(stream);
            foreach (var userElement in document.Root.Elements())
            {
                var name = userElement.Attribute("name")?.Value;
                var firstName = userElement.Element("firstName")?.Value;
                var lastName = userElement.Element("lastName")?.Value;
                var email = userElement.Element("email")?.Value;
                var password = userElement.Element("password")?.Value;
                var roles = userElement.Element("roles")?.Elements("role").Select(x => x.Value).ToArray();

                var addUserModel = new AddUserModel()
                {
                    UserName = name,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password,
                    Roles = roles
                };

                addUserModels.Add(addUserModel);
            }

            return addUserModels;
        }
    }
}
