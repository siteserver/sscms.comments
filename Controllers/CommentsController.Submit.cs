using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Models;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Comments.Controllers
{
    public partial class CommentsController
    {
        [HttpPost, Route("")]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] Comment request)
        {
            var site = await _sitetRepository.GetAsync(request.SiteId);
            var settings = await _commentManager.GetSettingsAsync(request.SiteId);
            if (settings.IsSubmitDisabled)
            {
                return this.Error("对不起，评论已被禁用");
            }

            request.UserId = _authManager.UserId;
            request.Status = settings.IsApprovedByDefault ? CommentStatus.Approved : CommentStatus.Pending;
            request.IpAddress = PageUtils.GetIpAddress(Request);

            request.Id = await _commentRepository.InsertAsync(request);
            await _commentManager.SendNotifyAsync(site, settings, request);

            List<Comment> list = null;
            var total = 0;
            if (settings.IsApprovedByDefault)
            {
                List<Comment> items;
                (total, items) = await _commentRepository.GetCommentsAsync(request.SiteId, request.ChannelId, request.ContentId, CommentStatus.Approved, null, 1, settings.PageSize);
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

            if (_authManager.IsUser)
            {
                var user = await _authManager.GetUserAsync();
                await _logRepository.AddUserLogAsync(user, PageUtils.GetIpAddress(Request), "发表评论");
            }

            return new SubmitResult
            {
                Items = list,
                Total = total
            };
        }
    }
}
