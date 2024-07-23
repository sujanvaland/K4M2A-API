using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IAttachmentService
    {
        public Task<List<Entities.File>> InsertAttachment(List<IFormFile> reqfilearr);
    }
}
