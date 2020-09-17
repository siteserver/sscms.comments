using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Comments.Core;
using SSCMS.Comments.Models;
using SSCMS.Utils;

namespace SSCMS.Comments.Controllers.Admin
{
    public partial class TemplatesLayerEditController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Edit([FromBody] EditRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, CommentManager.PermissionsTemplates))
                return Unauthorized();

            if (StringUtils.EqualsIgnoreCase(request.OriginalName, request.Name))
            {
                var templateInfoList = _commentManager.GetTemplateInfoList();
                var originalTemplateInfo = templateInfoList.First(x => StringUtils.EqualsIgnoreCase(request.OriginalName, x.Name));

                originalTemplateInfo.Name = request.Name;
                originalTemplateInfo.Description = request.Description;
                _commentManager.Edit(originalTemplateInfo);
            }
            else
            {
                var templateInfoList = _commentManager.GetTemplateInfoList();
                var originalTemplateInfo = templateInfoList.First(x => StringUtils.EqualsIgnoreCase(request.OriginalName, x.Name));

                if (templateInfoList.Any(x => StringUtils.EqualsIgnoreCase(request.Name, x.Name)))
                {
                    return this.Error($"标识为 {request.Name} 的模板已存在，请更换模板标识！");
                }

                var templateInfo = new TemplateInfo
                {
                    Name = request.Name,
                    Main = originalTemplateInfo.Main,
                    Publisher = string.Empty,
                    Description = request.Description,
                    Icon = originalTemplateInfo.Icon
                };
                templateInfoList.Add(templateInfo);

                _commentManager.Clone(request.OriginalName, templateInfo);

                _commentManager.DeleteTemplate(request.OriginalName);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
