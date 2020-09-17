using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageLayerEditController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromQuery] int siteId, [FromBody] Comment request)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId, CommentManager.PermissionsManage))
                return Unauthorized();

            await _commentRepository.UpdateAsync(request);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
