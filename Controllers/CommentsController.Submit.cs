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
            var settings = await _commentManager.GetSettingsAsync(request.SiteId);
            if (settings.IsClosed)
            {
                return this.Error("对不起，表单已被禁用");
            }

            request.SiteId = request.SiteId;
            request.ContentId = request.ContentId;
            request.UserId = _authManager.UserId;

            request.Id = await _commentRepository.InsertAsync(request);
            _commentManager.SendNotify(request);

            var (total, items) = await _commentRepository.GetCommentsAsync(request.SiteId, request.ContentId, null, 1, settings.PageSize);
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

            return new SubmitResult
            {
                Items = list,
                Total = total
            };
        }
    }
}
