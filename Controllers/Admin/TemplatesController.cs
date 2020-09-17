using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Models;
using SSCMS.Services;

namespace SSCMS.Comments.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesController : ControllerBase
    {
        private const string Route = "comments/templates";

        private readonly IAuthManager _authManager;
        private readonly ICommentManager _commentManager;
        private readonly ISettingsRepository _settingsRepository;

        public TemplatesController(IAuthManager authManager, ICommentManager formManager, ISettingsRepository formRepository)
        {
            _authManager = authManager;
            _commentManager = formManager;
            _settingsRepository = formRepository;
        }

        public class ListResult
        {
            public List<TemplateInfo> TemplateInfoList { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }
    }
}
