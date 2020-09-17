using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Models;

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

            var pageSize = settings.PageSize;

            var (total, items) = await _commentRepository.GetCommentsAsync(request.SiteId, request.ContentId, null, request.Page, pageSize);

            var list = new List<Comment>();
            foreach (var item in items)
            {
                var comment = item.Clone<Comment>();
                var user = new User();
                if (comment.UserId > 0)
                {
                    user = await _userRepository.GetByUserIdAsync(comment.UserId);
                }
                comment.Set("user", user);
                list.Add(comment);
            }

            return new GetResult
            {
                Items = list,
                Total = total,
                PageSize = pageSize
            };
        }
    }
}
