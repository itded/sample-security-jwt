using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using JwtDemoWebApp.Common.Enums;
using JwtDemoWebApp.Dto;

namespace JwtDemoWebApp.Services
{
    public class DocumentService : IDocumentService
    {
        public string[] GetAllDocumentsByRole(UserRoleEnum userRole)
        {
            var documents = GetDocuments();
            if (!documents.Any())
            {
                return Array.Empty<string>();
            }

            return documents.Where(d => d.Roles.Contains((int) userRole)).Select(d => d.Name).ToArray();
        }

        public string[] GetAllDocumentsByRoles(UserRoleEnum[] userRoles)
        {
            var documents = GetDocuments();
            if (!documents.Any())
            {
                return Array.Empty<string>();
            }

            return documents.Where(d => d.Roles.Any(r => userRoles.Contains((UserRoleEnum) r))).Select(d => d.Name)
                .ToArray();
        }

        private DocumentDto[] GetDocuments()
        {
            var documentsJson = File.ReadAllText("Assets/documents.json");
            var documents = JsonSerializer.Deserialize<DocumentDto[]>(documentsJson);
            return documents;
        }
    }
}