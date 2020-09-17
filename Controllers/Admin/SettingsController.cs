using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Services;

namespace SSCMS.Comments.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsController : ControllerBase
    {
        private const string Route = "comments/settings";

        private readonly IAuthManager _authManager;
        private readonly ICommentManager _commentManager;
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(IAuthManager authManager, ICommentManager formManager, ISettingsRepository formRepository)
        {
            _authManager = authManager;
            _commentManager = formManager;
            _settingsRepository = formRepository;
        }

        public class GetResult
        {
            public Settings Settings { get; set; }
            public List<string> AdministratorSmsNotifyKeys { get; set; }
            public List<string> UserSmsNotifyKeys { get; set; }
        }

        public class SubmitRequest : CommentRequest
        {
            public Settings Settings { get; set; }
        }
    }
}
