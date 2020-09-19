using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Models;
using SSCMS.Models;

namespace SSCMS.Comments.Controllers
{
    public partial class CommentsController
    {
        [HttpGet, Route("")]
        public async Task<ActionResult<GetResult>> List([FromQuery] ListRequest request)
        {
            var settings = await _commentManager.GetSettingsAsync(request.SiteId);

            List<Comment> list = null;
            var total = 0;
            var pageSize = settings.PageSize;

            if (request.Page > 0)
            {
                List<Comment> items;
                (total, items) = await _commentRepository.GetCommentsAsync(request.SiteId, request.ChannelId, request.ContentId, CommentStatus.Approved, null, request.Page, pageSize);
                list = new List<Comment>();
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
            }

            return new GetResult
            {
                IsSubmitDisabled = settings.IsSubmitDisabled,
                IsCaptcha = settings.IsCaptcha,
                IsApprovedByDefault = settings.IsApprovedByDefault,
                Items = list,
                Total = total,
                PageSize = pageSize
            };
        }
    }
}
