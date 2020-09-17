using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Comments.Core;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class ManageLayerEditController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsManage))
                return Unauthorized();

            //var value = new Dictionary<string, object>();
            //if (request.CommentId > 0)
            //{
            //    var comment = await _commentRepository.GetCommentAsync(request.CommentId);
            //    value = _commentRepository.GetDict(styles, comment);
            //}
            var comment = await _commentRepository.GetAsync(request.CommentId);

            //foreach (var style in styles)
            //{
            //    object value;
            //    if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
            //    {
            //        value = comment != null
            //            ? TranslateUtils.JsonDeserialize<List<string>>(comment.Get<string>(style.AttributeName))
            //            : new List<string>();
            //    }
            //    //else if (style.FieldType == InputType.Image.Value)
            //    //{
            //    //    value = comment != null
            //    //        ? new List<string> {comment.Get<string>(style.Title)}
            //    //        : new List<string>();
            //    //}
            //    else if (style.InputType == InputType.Date || style.InputType == InputType.DateTime)
            //    {
            //        value = comment?.Get<DateTime>(style.AttributeName);
            //    }
            //    else
            //    {
            //        value = comment?.Get<string>(style.AttributeName);
            //    }

            //    if (value == null)
            //    {
            //        value = string.Empty;
            //    }
            //}

            return new GetResult
            {
                Comment = comment
            };
        }
    }
}