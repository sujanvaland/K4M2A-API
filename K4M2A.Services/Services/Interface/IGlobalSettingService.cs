using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IGlobalSettingService
    {
        public Task<string> GetValue(string KeyName);
    }
}
