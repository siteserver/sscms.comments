using SSCMS.Dto;

namespace SSCMS.Comments.Core
{
    public class CommentRequest : SiteRequest
    {
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
    }
}
