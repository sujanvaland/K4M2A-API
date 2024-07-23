using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class GlobalSettingService : IGlobalSettingService
    {
        private readonly IRepository<GlobalSetting> _globalSettingRepository;

        public GlobalSettingService(IRepository<GlobalSetting> globalSettingRepository)
        {
            _globalSettingRepository = globalSettingRepository;
        }
        
        public async Task<string> GetValue(string KeyName)
        {
            var keyValue = await _globalSettingRepository.Table.Where(x => x.KeyName.ToLower() == KeyName.ToLower()).FirstOrDefaultAsync();
            if(keyValue != null)
            {
                return keyValue.Value;
            }
            return null;
        }

        public async Task<List<string>> GetListOfKeyValue(List<string> KeyList)
        {
            var keyValue = await _globalSettingRepository.Table.Where(x => KeyList.Contains(x.KeyName)).ToListAsync();
            if (keyValue != null)
            {
                return keyValue.Select(x=>x.Value).ToList();
            }
            return null;
        }
    }
}
