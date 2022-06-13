using Microsoft.AspNetCore.Http;

namespace Shared.Service;

public interface IImportService
{
    Task Import(IFormFile formFile);
}