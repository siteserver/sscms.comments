using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;
using SSCMS.Dto;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class TemplatesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsTemplates))
                return Unauthorized();

            var templateInfoList = _commentManager.GetTemplateInfoList();

            return new ListResult
            {
                TemplateInfoList = templateInfoList
            };
        }
    }
}
