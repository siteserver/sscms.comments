using System.Threading.Tasks;
using System.Web;
using SSCMS.Comments.Abstractions;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Comments.Core
{
    public class StlComments : IPluginParseAsync
    {
        private const string AttributeType = "type";

        private readonly IPathManager _pathManager;
        private readonly ISettingsRepository _settingsRepository;

        public StlComments(IPathManager pathManager, ISettingsRepository formRepository)
        {
            _pathManager = pathManager;
            _settingsRepository = formRepository;
        }

        public string ElementName => "stl:comments";

        public async Task<string> ParseAsync(IParseStlContext context)
        {
            var type = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = await context.ParseAsync(value);
                }
            }

            var apiUrl = _pathManager.GetApiUrl();
            if (string.IsNullOrEmpty(context.StlInnerHtml))
            {
                var elementId = $"iframe_{StringUtils.GetShortGuid(false)}";
                var libUrl = _pathManager.GetRootUrl("assets/comments/lib/iframe-resizer-3.6.3/iframeResizer.min.js");
                var pageUrl = _pathManager.GetRootUrl($"assets/comments/templates/{type}/index.html?siteId={context.SiteId}&contentId={context.ContentId}&apiUrl={HttpUtility.UrlEncode(apiUrl)}");

                return $@"
<iframe id=""{elementId}"" frameborder=""0"" scrolling=""no"" src=""{pageUrl}"" style=""width: 1px;min-width: 100%;""></iframe>
<script type=""text/javascript"" src=""{libUrl}""></script>
<script type=""text/javascript"">iFrameResize({{log: false}}, '#{elementId}')</script>
";
            }

            return $@"
<script>
var $formConfigApiUrl = '{apiUrl}';
var $formConfigSiteId = {context.SiteId};
var $formConfigContentId = {context.ContentId};
</script>
{context.StlInnerHtml}
";
        }
    }
}
