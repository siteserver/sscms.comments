using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsSettings))
                return Unauthorized();

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);

            settings.IsClosed = request.Settings.IsClosed;
            settings.PageSize = request.Settings.PageSize;
            settings.IsCaptcha = request.Settings.IsCaptcha;
            settings.IsAdministratorSmsNotify = request.Settings.IsAdministratorSmsNotify;
            settings.AdministratorSmsNotifyTplId = request.Settings.AdministratorSmsNotifyTplId;
            settings.AdministratorSmsNotifyKeys = request.Settings.AdministratorSmsNotifyKeys;
            settings.AdministratorSmsNotifyMobile = request.Settings.AdministratorSmsNotifyMobile;
            settings.IsAdministratorMailNotify = request.Settings.IsAdministratorMailNotify;
            settings.AdministratorMailNotifyAddress = request.Settings.AdministratorMailNotifyAddress;
            settings.IsUserSmsNotify = request.Settings.IsUserSmsNotify;
            settings.UserSmsNotifyTplId = request.Settings.UserSmsNotifyTplId;
            settings.UserSmsNotifyKeys = request.Settings.UserSmsNotifyKeys;
            settings.UserSmsNotifyMobileName = request.Settings.UserSmsNotifyMobileName;

            await _settingsRepository.UpdateAsync(settings);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
