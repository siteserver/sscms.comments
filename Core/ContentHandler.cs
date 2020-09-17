using SSCMS.Comments.Abstractions;
using SSCMS.Plugins;

namespace SSCMS.Comments.Core
{
    public class ContentHandler : PluginContentHandler
    {
        private readonly ICommentManager _commentManager;

        public ContentHandler(ICommentManager formManager)
        {
            _commentManager = formManager;
        }

        //public override async Task OnTranslatedAsync(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId, int targetContentId)
        //{
        //    await _commentManager.TranslateAsync(siteId, channelId, contentId, targetSiteId, targetChannelId,
        //        targetContentId);
        //}
    }
}
