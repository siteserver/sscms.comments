using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageController
    {
        [HttpPost, Route(ActionsSetStatus)]
        public async Task<ActionResult<SetStatusResult>> SetStatus([FromBody] SetStatusRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            await _commentRepository.SetStatusAsync(request.CommentIds, request.CommentStatus);

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);
            var site = await _sitetRepository.GetAsync(request.SiteId);
            var (total, items) = await GetCommentsAsync(settings, site, request.ChannelId, request.ContentId, request.Status, request.Keyword, request.Page);

            return new SetStatusResult
            {
                Items = items,
                Total = total
            };
        }
    }
}
