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
    public partial class TemplatesLayerEditController : ControllerBase
    {
        private const string Route = "comments/templatesLayerEdit";

        private readonly IAuthManager _authManager;
        private readonly ICommentManager _commentManager;

        public TemplatesLayerEditController(IAuthManager authManager, ICommentManager formManager)
        {
            _authManager = authManager;
            _commentManager = formManager;
        }

        public class GetRequest : SiteRequest
        {
            public string Name { get; set; }
        }

        public class GetResult
        {
            public TemplateInfo TemplateInfo { get; set; }
        }

        public class CloneRequest : SiteRequest
        {
            public string OriginalName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string TemplateHtml { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public string OriginalName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
