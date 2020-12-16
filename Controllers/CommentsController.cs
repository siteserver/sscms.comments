using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Comments.Controllers
{
    [Route("api/comments")]
    public partial class CommentsController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICommentManager _commentManager;
        private readonly ICommentRepository _commentRepository;

        public CommentsController(IAuthManager authManager, IUserRepository userRepository, ILogRepository logRepository, ICommentManager commentManager, ICommentRepository commentRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _commentManager = commentManager;
            _commentRepository = commentRepository;
        }

        public class GetFormResult
        {
            public bool IsCaptcha { get; set; }
        }

        public class ListRequest : CommentRequest
        {
            public int Page { get; set; }
        }

        public class GetResult
        {
            public bool IsSubmitDisabled { get; set; }
            public bool IsCaptcha { get; set; }
            public bool IsApprovedByDefault { get; set; }
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
        }

        public class SubmitResult
        {
            public List<Comment> Items { get; set; }
            public int Total { get; set; }
        }
    }
}
