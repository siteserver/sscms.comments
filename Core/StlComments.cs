using System.Threading.Tasks;
using System.Web;
using SSCMS.Comments.Abstractions;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Comments.Core
{
    public class StlComments : IPluginParseAsync
    {
        private const string ChannelIndex = nameof(ChannelIndex);
        private const string ChannelName = nameof(ChannelName);
        private const string AttributeType = "type";
        private const string AttributeTheme = "theme";

        private const string TypeCount = "count";

        private readonly IPathManager _pathManager;
        private readonly IChannelRepository _channelRepository;
        private readonly ICommentRepository _commentRepository;

        public StlComments(IPathManager pathManager, IChannelRepository channelRepository, ICommentRepository commentRepository)
        {
            _pathManager = pathManager;
            _channelRepository = channelRepository;
            _commentRepository = commentRepository;
        }

        public string ElementName => "stl:comments";

        public async Task<string> ParseAsync(IParseStlContext context)
        {
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var type = string.Empty;
            var theme = string.Empty;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = await context.ParseAsync(value);
                }
            }

            var channelId = context.ChannelId;
            var contentId = context.ContentId;
            if (!string.IsNullOrEmpty(channelIndex) || !string.IsNullOrEmpty(channelName))
            {
                channelId = await _channelRepository.GetChannelIdAsync(context.SiteId, context.ChannelId, channelIndex, channelName);
                contentId = 0;
            }

            if (StringUtils.EqualsIgnoreCase(type, TypeCount))
            {
                var count = _commentRepository.GetCountAsync(context.SiteId, channelId, contentId);
                return count.ToString();
            }

            var apiUrl = _pathManager.GetApiUrl();
            if (string.IsNullOrEmpty(context.StlInnerHtml))
            {
                var elementId = $"iframe_{StringUtils.GetShortGuid(false)}";
                var libUrl = _pathManager.GetRootUrl("assets/comments/lib/iframe-resizer-3.6.3/iframeResizer.min.js");
                var pageUrl = _pathManager.GetRootUrl($"assets/comments/templates/{theme}/index.html?siteId={context.SiteId}&channelId={context.ChannelId}&contentId={context.ContentId}&apiUrl={HttpUtility.UrlEncode(apiUrl)}");

                return $@"
<iframe id=""{elementId}"" frameborder=""0"" scrolling=""no"" src=""{pageUrl}"" style=""width: 1px;min-width: 100%;""></iframe>
<script type=""text/javascript"" src=""{libUrl}""></script>
<script type=""text/javascript"">iFrameResize({{log: false}}, '#{elementId}')</script>
";
            }

            return $@"
<script>
var $commentsConfigApiUrl = '{apiUrl}';
var $commentsConfigSiteId = {context.SiteId};
var $commentsConfigChannelId = {context.ChannelId};
var $commentsConfigContentId = {context.ContentId};
</script>
{context.StlInnerHtml}
";
        }
    }
}
