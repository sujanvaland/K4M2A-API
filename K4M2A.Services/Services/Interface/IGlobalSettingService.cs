using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.Services.Interface
{
    public interface IGlobalSettingService
    {
        public Task<string> GetValue(string KeyName);
    }
}
