using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Services;

namespace SSCMS.Comments.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ManageLayerEditController : ControllerBase
    {
        private const string Route = "comments/manageLayerEdit";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICommentManager _commentManager;
        private readonly ICommentRepository _commentRepository;
        private readonly ISettingsRepository _settingsRepository;

        public ManageLayerEditController(IAuthManager authManager, IPathManager pathManager, ICommentManager formManager, ICommentRepository dataRepository, ISettingsRepository formRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _commentManager = formManager;
            _commentRepository = dataRepository;
            _settingsRepository = formRepository;
        }

        public class GetRequest : CommentRequest
        {
            public int CommentId { get; set; }
        }

        public class GetResult
        {
            public Comment Comment { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public int FieldId { get; set; }
        }

        public class UploadResult
        {
            public string ImageUrl { get; set; }
            public int FieldId { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string FileUrl { get; set; }
        }
    }
}
