using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Comments.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommentStatus
	{
        [DataEnum(DisplayName = "全部")] All,
        [DataEnum(DisplayName = "待审核")] Pending,
        [DataEnum(DisplayName = "已审核")] Approved,
        [DataEnum(DisplayName = "垃圾信息")] Spam
    }
}
