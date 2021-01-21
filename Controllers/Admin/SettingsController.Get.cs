using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;
using SSCMS.Utils;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] CommentRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsSettings))
                return Unauthorized();

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);

            var administratorSmsNotifyKeys = ListUtils.GetStringList(settings.AdministratorSmsNotifyKeys);
            var userSmsNotifyKeys = ListUtils.GetStringList(settings.UserSmsNotifyKeys);

            var isSmsEnabled = await _smsManager.IsEnabledAsync();
            var isMailEnabled = await _mailManager.IsEnabledAsync();

            return new GetResult
            {
                Settings = settings,
                AdministratorSmsNotifyKeys = administratorSmsNotifyKeys,
                UserSmsNotifyKeys = userSmsNotifyKeys,
                IsSmsEnabled = isSmsEnabled,
                IsMailEnabled = isMailEnabled
            };
        }
    }
}
