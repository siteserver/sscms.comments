using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Models;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Comments.Core
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly Repository<Settings> _repository;

        public SettingsRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Settings>(settingsManager.Database, settingsManager.Redis);
        }

        private static string GetCacheKey(int siteId)
        {
            return $"SSCMS.Comments.Core.Repositories.SettingsRepository:{siteId}";
        }

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Settings settings)
        {
            if (settings.SiteId == 0) return 0;

            settings.Id = await _repository.InsertAsync(settings, Q.CachingRemove(GetCacheKey(settings.SiteId)));

            return settings.Id;
        }

        public async Task<bool> UpdateAsync(Settings settings)
        {
            var updated = await _repository.UpdateAsync(settings, Q
                .CachingRemove(GetCacheKey(settings.SiteId))
            );

            return updated;
        }

        public async Task DeleteAsync(int siteId)
        {
            if (siteId <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(Settings.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<Settings> GetAsync(int siteId)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(Settings.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );
        }

        public List<string> GetAllAttributeNames(List<TableStyle> styles)
        {
            var allAttributeNames = new List<string>
            {
                nameof(Comment.Id),
                nameof(Comment.Guid)
            };
            foreach (var style in styles)
            {
                allAttributeNames.Add(style.AttributeName);
            }
            allAttributeNames.Add(nameof(Comment.CreatedDate));
            allAttributeNames.Add(nameof(Comment.LastModifiedDate));

            return allAttributeNames;
        }
    }
}
