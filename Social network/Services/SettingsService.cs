using Models.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SettingsService
    {
        private readonly IRepository<Setting> _SettingRepository;

        public SettingsService(IRepository<Setting> repository)
        {
            _SettingRepository = repository;
        }
        public virtual IQueryable<Setting> GetAllSettings()
        {
            return _SettingRepository.GetAll();
        }
        public virtual IQueryable<Setting> GetAllSettingsQuerible(Expression<Func<Setting, bool>> expression)
        {
            return _SettingRepository.GetAllQuerible(expression);
        }
        public virtual Setting GetSettingsById(int SettingId)
        {
            return _SettingRepository.GetById(SettingId);
        }
        public virtual void UpdateSetting(Setting setting)
        {
            _SettingRepository.Update(setting);
        }
        public virtual void DeleteSetting(int id)
        {
            _SettingRepository.Remove(id);
        }
        public virtual void CreateSetting(Setting setting)
        {
            _SettingRepository.Create(setting);
        }
    }
}
