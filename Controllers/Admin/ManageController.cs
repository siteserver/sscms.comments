using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Comments.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "comments/manage";
        private const string ActionsExport = "comments/manage/actions/export";
        private const string ActionsColumns = "comments/manage/actions/columns";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IUserRepository _userRepository;
        private readonly ICommentManager _commentManager;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ICommentRepository _commentRepository;

        public ManageController(IAuthManager authManager, IPathManager pathManager, IUserRepository userRepository, ICommentManager formManager, ISettingsRepository formRepository, ICommentRepository dataRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
            _commentManager = formManager;
            _settingsRepository = formRepository;
            _commentRepository = dataRepository;
        }

        public class GetRequest : CommentRequest
        {
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
            public int Page { get; set; }
            public int CommentId { get; set; }
        }

        public class DeleteResult
        {
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
        }

        public class ColumnsRequest : CommentRequest
        {
            public List<string> AttributeNames { get; set; }
        }
    }
}
