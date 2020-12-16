using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class TemplateHtmlController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsTemplates))
                return Unauthorized();

            var templateInfo = _commentManager.GetTemplateInfo(request.Name);
            var html = _commentManager.GetTemplateHtml(templateInfo);

            var isSystem = templateInfo.Publisher == "sscms";
            if (isSystem)
            {
                templateInfo = new TemplateInfo();
            }

            return new GetResult
            {
                TemplateInfo = templateInfo,
                TemplateHtml = html,
                IsSystem = isSystem
            };
        }
    }
}
