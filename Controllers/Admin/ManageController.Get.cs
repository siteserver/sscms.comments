using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);
            var site = await _sitetRepository.GetAsync(request.SiteId);
            var (total, items) = await GetCommentsAsync(settings, site, request.ChannelId, request.ContentId, request.Status, request.Keyword, request.Page);

            return new GetResult
            {
                Items = items,
                Total = total,
                PageSize = settings.PageSize
            };
        }
    }
}
