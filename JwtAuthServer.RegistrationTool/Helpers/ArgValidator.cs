using System;
using System.Diagnostics;
using System.IO;
using JwtAuthServer.RegistrationTool.Common;

namespace JwtAuthServer.RegistrationTool.Helpers
{
    public static class ArgValidator
    {
        public static void StringReadValidation(string[] args)
        {
            var invalidArgLen = args.Length == 0;
            if (invalidArgLen)
            {
                ThrowUnknownCommandException();
            }

            var addRoles = args[0].ToLower() == Constants.AddRolesCommand;
            var addUsers = args[0].ToLower() == Constants.AddUsersCommand;
            var invalidMode = !addRoles && !addUsers;
            if (invalidMode)
            {
                ThrowUnknownCommandException();
            }

            var inputFileName = args[1];

            if (inputFileName.Length == 0)
            {
                throw new ApplicationException("The input file name is empty.");
            }

            if (!File.Exists(inputFileName))
            {
                throw new ApplicationException("The input file was not found.");
            }
        }

        private static void ThrowUnknownCommandException()
        {
            var fileName = Process.GetCurrentProcess().MainModule?.FileName ?? "JwtAuthServer.RegistrationTool.exe";
            throw new ApplicationException($"Unknown command.\nPlease use one of the next commands:\n" +
                                           $"{fileName} {Constants.AddRolesCommand} [file name]\n" +
                                           $"{fileName} {Constants.AddUsersCommand} [file name]");
        }
    }
}
