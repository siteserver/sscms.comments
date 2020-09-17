using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Comments.Models;
using SSCMS.Models;

namespace SSCMS.Comments.Abstractions
{
    public interface ISettingsRepository
    {
        List<TableColumn> TableColumns { get; }

        Task<int> InsertAsync(Settings setting);

        Task<bool> UpdateAsync(Settings setting);

        Task DeleteAsync(int siteId);

        Task<Settings> GetAsync(int siteId);

        List<string> GetAllAttributeNames(List<TableStyle> styles);
    }
}
