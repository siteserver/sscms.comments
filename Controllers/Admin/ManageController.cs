using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Models;

namespace SSCMS.Comments.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "comments/manage";
        private const string ActionsExport = "comments/manage/actions/export";
        private const string ActionsSetStatus = "comments/manage/actions/setStatus";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _sitetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICommentManager _commentManager;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ICommentRepository _commentRepository;

        public ManageController(IAuthManager authManager, IPathManager pathManager, ISiteRepository sitetRepository, IUserRepository userRepository, IContentRepository contentRepository, ICommentManager formManager, ISettingsRepository formRepository, ICommentRepository dataRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _sitetRepository = sitetRepository;
            _userRepository = userRepository;
            _contentRepository = contentRepository;
            _commentManager = formManager;
            _settingsRepository = formRepository;
            _commentRepository = dataRepository;
        }

        public class GetRequest : CommentRequest
        {
            public CommentStatus Status { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
        }

        public class GetResult
        {
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
        }

        public class DeleteRequest : CommentRequest
        {
            public CommentStatus Status { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int CommentId { get; set; }
        }

        public class DeleteResult
        {
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
        }

        public class SetStatusRequest : CommentRequest
        {
            public CommentStatus Status { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public List<int> CommentIds { get; set; }
            public CommentStatus CommentStatus { get; set; }
        }

        public class SetStatusResult
        {
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
        }

        private async Task<(int, List<Comment>)> GetCommentsAsync(Settings settings, Site site, int channelId, int contentId, CommentStatus status, string keyword, int page)
        {
            var pageSize = settings.PageSize;

            var (total, items) = await _commentRepository.GetCommentsAsync(site.Id, channelId, contentId, status, keyword, page, pageSize);

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
                if (channelId != comment.ChannelId || contentId != comment.ContentId)
                {
                    var content = await _contentRepository.GetAsync(site, comment.ChannelId, comment.ContentId);
                    if (content == null)
                    {
                        total--;
                        continue;
                    }
                    comment.Set("contentTitle", content.Title);
                    comment.Set("contentChecked", content.Checked);
                }
                list.Add(comment);
            }

            return (total, list);
        }
    }
}
