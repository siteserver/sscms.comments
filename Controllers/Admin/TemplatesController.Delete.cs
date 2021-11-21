using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class TemplatesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<ListResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsTemplates))
                return Unauthorized();

            _commentManager.DeleteTemplate(request.Name);

            return new ListResult
            {
                TemplateInfoList = _commentManager.GetTemplateInfoList()
            };
        }
    }
}
