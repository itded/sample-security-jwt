using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using JwtDemoWebApp.Common.Enums;
using JwtDemoWebApp.Dto;

namespace JwtDemoWebApp.Services
{
    public class DocumentService : IDocumentService
    {
        public IList<string> GetAllDocumentsByRole(UserRoleEnum userRole)
        {
            var documentsJson = File.ReadAllText("Assets/documents.json");
            var documents = JsonSerializer.Deserialize<IList<DocumentDto>>(documentsJson);
            if (!documents.Any())
            {
                return new List<string>();
            }

            return documents.Where(d => d.Roles.Contains((int) userRole)).Select(d => d.Name).ToList();
        }
    }
}