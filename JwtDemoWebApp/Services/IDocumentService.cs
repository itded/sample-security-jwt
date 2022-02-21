using System.Collections.Generic;
using JwtDemoWebApp.Common.Enums;

namespace JwtDemoWebApp.Services
{
    public interface IDocumentService
    {
        public IList<string> GetAllDocumentsByRole(UserRoleEnum userRole);
    }
}