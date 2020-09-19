using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;
using SSCMS.Comments.Utils;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageController
    {
        [HttpPost, Route(ActionsExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] CommentRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            var comments = await _commentRepository.GetCommentsAsync(request.SiteId, request.ChannelId, request.ContentId);

            var head = new List<string> {"编号", "评论", "添加时间"};

            var rows = new List<List<string>>();

            foreach (var comment in comments)
            {
                var row = new List<string>
                {
                    comment.Guid,
                    comment.Content
                };

                if (comment.CreatedDate.HasValue)
                {
                    row.Add(comment.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                }

                rows.Add(row);
            }

            const string fileName = "评论.csv";

            CsvUtils.Export(_pathManager.GetTemporaryFilesPath(fileName), head, rows);
            var downloadUrl = _pathManager.GetTemporaryFilesUrl(fileName);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
