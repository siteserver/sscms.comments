﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Abstractions;
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
        private readonly ICommentManager _commentManager;
        private readonly ICommentRepository _commentRepository;

        public CommentsController(IAuthManager authManager, IUserRepository userRepository, ICommentManager commentManager, ICommentRepository commentRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _commentManager = commentManager;
            _commentRepository = commentRepository;
        }

        public class GetFormResult
        {
            public bool IsCaptcha { get; set; }
        }

        public class ListRequest
        {
            public int SiteId { get; set; }
            public int ContentId { get; set; }
            public int Page { get; set; }
        }

        public class GetResult
        {
            public bool IsClosed { get; set; }
            public bool IsCaptcha { get; set; }
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
