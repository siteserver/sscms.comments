using Datory;
using Datory.Annotations;

namespace SSCMS.Comments.Models
{
    [DataTable("sscms_comments_settings")]
    public class Settings : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        public bool IsClosed { get; set; }

        public bool IsCaptcha { get; set; }

        public int PageSize { get; set; } = 30;

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify { get; set; }

        public string AdministratorSmsNotifyTplId { get; set; }

        public string AdministratorSmsNotifyKeys { get; set; }

        public string AdministratorSmsNotifyMobile { get; set; }

        //向管理员发送邮件通知
        public bool IsAdministratorMailNotify { get; set; }

        public string AdministratorMailNotifyAddress { get; set; }

        //向用户发送短信通知
        public bool IsUserSmsNotify { get; set; }

        public string UserSmsNotifyTplId { get; set; }

        public string UserSmsNotifyKeys { get; set; }

        public string UserSmsNotifyMobileName { get; set; }
    }
}
