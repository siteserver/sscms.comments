using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            //var page = request.Page;

            var comment = await _commentRepository.GetAsync(request.CommentId);
            if (comment == null) return NotFound();

            await _commentRepository.DeleteAsync(request.CommentId);

            //var pageSize = _commentManager.GetPageSize(formInfo);

            //var (total, commentList) = await _commentRepository.GetDataAsync(formInfo, false, null, page, pageSize);
            //var pages = Convert.ToInt32(Math.Ceiling((double)total / pageSize));
            //if (pages == 0) pages = 1;
            //if (page > pages) page = pages;

            //var comments = new List<IDictionary<string, object>>();
            //foreach (var info in commentList)
            //{
            //    comments.Add(info.ToDictionary());
            //}

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);

            var (total, commentList) = await _commentRepository.GetCommentsAsync(request.SiteId, request.ContentId, null, request.Page, settings.PageSize);
            var items = commentList;

            return new DeleteResult
            {
                Items = items,
                Total = total
            };
        }
    }
}
