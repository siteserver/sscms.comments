using Microsoft.Extensions.DependencyInjection;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Core;
using SSCMS.Plugins;

namespace SSCMS.Comments
{
    public class Startup : IPluginConfigureServices
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentManager, CommentManager>();
        }
    }
}
