using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class TemplateHtmlController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsTemplates))
                return Unauthorized();

            var templateInfo = _commentManager.GetTemplateInfo(request.Name);

            _commentManager.SetTemplateHtml(templateInfo, request.TemplateHtml);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
