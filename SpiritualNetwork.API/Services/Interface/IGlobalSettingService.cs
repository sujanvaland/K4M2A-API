using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IGlobalSettingService
    {
        public Task<string> GetValue(string KeyName);
    }
}
