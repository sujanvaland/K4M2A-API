using Microsoft.AspNetCore.Http;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IAttachmentService
    {
        public Task<List<Entities.File>> InsertAttachment(List<IFormFile> reqfilearr);
    }
}
