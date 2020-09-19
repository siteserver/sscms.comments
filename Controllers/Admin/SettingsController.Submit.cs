using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class SettingsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] Settings request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsSettings))
                return Unauthorized();

            var settings = await _commentManager.GetSettingsAsync(request.SiteId);

            settings.IsSubmitDisabled = request.IsSubmitDisabled;
            settings.PageSize = request.PageSize;
            settings.IsCaptcha = request.IsCaptcha;
            settings.IsApprovedByDefault = request.IsApprovedByDefault;
            settings.IsAdministratorSmsNotify = request.IsAdministratorSmsNotify;
            settings.AdministratorSmsNotifyTplId = request.AdministratorSmsNotifyTplId;
            settings.AdministratorSmsNotifyKeys = request.AdministratorSmsNotifyKeys;
            settings.AdministratorSmsNotifyMobile = request.AdministratorSmsNotifyMobile;
            settings.IsAdministratorMailNotify = request.IsAdministratorMailNotify;
            settings.AdministratorMailNotifyAddress = request.AdministratorMailNotifyAddress;
            settings.IsUserSmsNotify = request.IsUserSmsNotify;
            settings.UserSmsNotifyTplId = request.UserSmsNotifyTplId;
            settings.UserSmsNotifyKeys = request.UserSmsNotifyKeys;
            settings.UserSmsNotifyMobileName = request.UserSmsNotifyMobileName;

            await _settingsRepository.UpdateAsync(settings);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
