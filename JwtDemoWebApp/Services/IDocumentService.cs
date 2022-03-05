using JwtDemoWebApp.Common.Enums;

namespace JwtDemoWebApp.Services
{
    public interface IDocumentService
    {
        public string[] GetAllDocumentsByRole(UserRoleEnum userRole);

        public string[] GetAllDocumentsByRoles(UserRoleEnum[] userRoles);
    }
}