using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            var comment = await _commentRepository.GetAsync(request.CommentId);
            if (comment == null) return NotFound();

            await _commentRepository.DeleteAsync(request.CommentId);

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);
            var site = await _sitetRepository.GetAsync(request.SiteId);
            var (total, items) = await GetCommentsAsync(settings, site, request.ChannelId, request.ContentId, request.Status, request.Keyword, request.Page);

            return new DeleteResult
            {
                Items = items,
                Total = total
            };
        }
    }
}
