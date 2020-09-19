using Datory;
using Datory.Annotations;
using SSCMS.Comments.Utils;

namespace SSCMS.Comments.Models
{
    [DataTable(CommentUtils.TableName)]
    public class Comment : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public CommentStatus Status { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public string Content { get; set; }
    }
}
