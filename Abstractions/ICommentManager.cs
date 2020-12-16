using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Comments.Models;

namespace SSCMS.Comments.Abstractions
{
    public interface ICommentManager
    {
        Task<Settings> GetSettingsAsync(int siteId);

        Task DeleteAsync(int siteId);

        Task<string> GetMailTemplateHtmlAsync();

        Task<string> GetMailListHtmlAsync();

        void SendNotify(Comment comment);

        List<TemplateInfo> GetTemplateInfoList();

        TemplateInfo GetTemplateInfo(string name);

        void Clone(string nameToClone, TemplateInfo templateInfo, string templateHtml = null);

        void Edit(TemplateInfo templateInfo);

        string GetTemplateHtml(TemplateInfo templateInfo);

        void SetTemplateHtml(TemplateInfo templateInfo, string html);

        void DeleteTemplate(string name);
    }
}
