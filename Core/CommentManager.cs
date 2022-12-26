using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Models;
using SSCMS.Comments.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Comments.Core
{
    public class CommentManager : ICommentManager
    {
        public const string PluginId = "sscms.comments";
        public const string PermissionsManage = "comments_manage";
        public const string PermissionsSettings = "comments_settings";
        public const string PermissionsTemplates = "comments_templates";

        private readonly ICacheManager _cacheManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISmsManager _smsManager;
        private readonly IMailManager _mailManager;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICommentRepository _commentRepository;

        public CommentManager(ICacheManager cacheManager, IPathManager pathManager, IPluginManager pluginManager, ISmsManager smsManager, IMailManager mailManager, ITableStyleRepository tableStyleRepository, IContentRepository contentRepository, ISettingsRepository settingsRepository, ICommentRepository commentRepository)
        {
            _cacheManager = cacheManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _smsManager = smsManager;
            _mailManager = mailManager;
            _tableStyleRepository = tableStyleRepository;
            _contentRepository = contentRepository;
            _settingsRepository = settingsRepository;
            _commentRepository = commentRepository;
        }

        public async Task<Settings> GetSettingsAsync(int siteId)
        {
            var settings = await _settingsRepository.GetAsync(siteId);
            if (settings == null)
            {
                settings = new Settings
                {
                    SiteId = siteId,
                    IsCaptcha = true,
                    IsApprovedByDefault = false,
                    PageSize = 30,
                };
                settings.Id = await _settingsRepository.InsertAsync(settings);
            }

            if (settings.PageSize == 0)
            {
                settings.PageSize = 30;
            }

            return settings;
        }

        public async Task DeleteAsync(int siteId)
        {
            if (siteId <= 0) return;

            var relatedIdentities = new List<int> {siteId};

            await _tableStyleRepository.DeleteAllAsync(CommentUtils.TableName, relatedIdentities);
            await _commentRepository.DeleteBySiteIdAsync(siteId);
            await _settingsRepository.DeleteAsync(siteId);
        }

        private string GetMailTemplatesDirectoryPath()
        {
            var plugin = _pluginManager.GetPlugin(PluginId);
            return PathUtils.Combine(plugin.WebRootPath, "assets/comments/mail");
        }

        public async Task<string> GetMailTemplateHtmlAsync()
        {
            var directoryPath = GetMailTemplatesDirectoryPath();
            var htmlPath = PathUtils.Combine(directoryPath, "template.html");
            if (_cacheManager.Exists(htmlPath)) return _cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            _cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }

        public async Task<string> GetMailListHtmlAsync()
        {
            var directoryPath = GetMailTemplatesDirectoryPath();
            var htmlPath = PathUtils.Combine(directoryPath, "list.html");
            if (_cacheManager.Exists(htmlPath)) return _cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            _cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }

        public async Task SendNotifyAsync(Site site, Settings settings, Comment comment)
        {
            if (settings.IsAdministratorSmsNotify &&
                !string.IsNullOrEmpty(settings.AdministratorSmsNotifyTplId) &&
                !string.IsNullOrEmpty(settings.AdministratorSmsNotifyMobile))
            {
                var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
                if (isSmsEnabled)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(settings.AdministratorSmsNotifyKeys))
                    {
                        var keys = settings.AdministratorSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (StringUtils.EqualsIgnoreCase(key, nameof(Comment.Id)))
                            {
                                parameters.Add(key, comment.Id.ToString());
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, nameof(Comment.CreatedDate)))
                            {
                                if (comment.CreatedDate.HasValue)
                                {
                                    parameters.Add(key, comment.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = comment.Get<string>(key);
                                parameters.Add(key, value);
                            }
                        }
                    }

                    await _smsManager.SendSmsAsync(settings.AdministratorSmsNotifyMobile,
                        settings.AdministratorSmsNotifyTplId, parameters);
                }
            }

            if (settings.IsAdministratorMailNotify &&
                !string.IsNullOrEmpty(settings.AdministratorMailNotifyAddress))
            {
                var isMailEnabled = await _mailManager.IsMailEnabledAsync();
                if (isMailEnabled)
                {
                    var templateHtml = await GetMailTemplateHtmlAsync();
                    var listHtml = await GetMailListHtmlAsync();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", comment.Guid)
                    };
                    if (comment.CreatedDate.HasValue)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>("提交时间",
                            comment.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm")));
                    }

                    keyValueList.Add(new KeyValuePair<string, string>("评论内容",
                        comment.Content));

                    var list = new StringBuilder();
                    foreach (var kv in keyValueList)
                    {
                        list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                    }

                    var contentInfo = await _contentRepository.GetAsync(site, comment.ChannelId, comment.ContentId);
                    var title = contentInfo?.Title ?? string.Empty;

                    var htmlBody = templateHtml
                        .Replace("{{title}}", title)
                        .Replace("{{list}}", list.ToString());

                    await _mailManager.SendMailAsync(settings.AdministratorMailNotifyAddress, "[SSCMS] 通知邮件",
                        htmlBody);
                }
            }

            if (settings.IsUserSmsNotify &&
                !string.IsNullOrEmpty(settings.UserSmsNotifyTplId) &&
                !string.IsNullOrEmpty(settings.UserSmsNotifyMobileName))
            {
                var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
                if (isSmsEnabled)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(settings.UserSmsNotifyKeys))
                    {
                        var keys = settings.UserSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (StringUtils.EqualsIgnoreCase(key, nameof(Comment.Id)))
                            {
                                parameters.Add(key, comment.Id.ToString());
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, nameof(Comment.CreatedDate)))
                            {
                                if (comment.CreatedDate.HasValue)
                                {
                                    parameters.Add(key, comment.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = comment.Get<string>(key);
                                parameters.Add(key, value);
                            }
                        }
                    }

                    var mobile = comment.Get<string>(settings.UserSmsNotifyMobileName);
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        await _smsManager.SendSmsAsync(mobile, settings.UserSmsNotifyTplId, parameters);
                    }
                }
            }
        }

        private string GetTemplatesDirectoryPath()
        {
            var plugin = _pluginManager.GetPlugin(PluginId);
            return PathUtils.Combine(plugin.WebRootPath, "assets/comments/templates");
        }

        public List<TemplateInfo> GetTemplateInfoList()
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var directoryNames = DirectoryUtils.GetDirectoryNames(directoryPath);

            return directoryNames.Select(directoryName => GetTemplateInfo(directoryPath, directoryName)).Where(templateInfo => templateInfo != null).ToList();
        }

        public TemplateInfo GetTemplateInfo(string name)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            return GetTemplateInfo(directoryPath, name);
        }

        private TemplateInfo GetTemplateInfo(string templatesDirectoryPath, string name)
        {
            TemplateInfo templateInfo = null;

            var configPath = PathUtils.Combine(templatesDirectoryPath, name, "config.json");
            if (FileUtils.IsFileExists(configPath))
            {
                templateInfo = TranslateUtils.JsonDeserialize<TemplateInfo>(FileUtils.ReadText(configPath));
                templateInfo.Name = name;
            }

            return templateInfo;
        }

        public void Clone(string nameToClone, TemplateInfo templateInfo, string templateHtml = null)
        {
            var plugin = _pluginManager.GetPlugin(PluginId);
            var directoryPath = PathUtils.Combine(plugin.WebRootPath, "assets/comments/templates");

            DirectoryUtils.Copy(PathUtils.Combine(directoryPath, nameToClone), PathUtils.Combine(directoryPath, templateInfo.Name), true);

            var configJson = TranslateUtils.JsonSerialize(templateInfo);
            var configPath = PathUtils.Combine(directoryPath, templateInfo.Name, "config.json");
            FileUtils.WriteText(configPath, configJson);

            if (templateHtml != null)
            {
                SetTemplateHtml(templateInfo, templateHtml);
            }
        }

        public void Edit(TemplateInfo templateInfo)
        {
            var plugin = _pluginManager.GetPlugin(PluginId);
            var directoryPath = PathUtils.Combine(plugin.ContentRootPath, "assets/comments/templates");

            var configJson = TranslateUtils.JsonSerialize(templateInfo);
            var configPath = PathUtils.Combine(directoryPath, templateInfo.Name, "config.json");
            FileUtils.WriteText(configPath, configJson);
        }

        public string GetTemplateHtml(TemplateInfo templateInfo)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PathUtils.Combine(directoryPath, templateInfo.Name, templateInfo.Main);
            return _pathManager.GetContentByFilePath(htmlPath);
        }

        public void SetTemplateHtml(TemplateInfo templateInfo, string html)
        {
            var directoryPath = GetTemplatesDirectoryPath();
            var htmlPath = PathUtils.Combine(directoryPath, templateInfo.Name, templateInfo.Main);

            FileUtils.WriteText(htmlPath, html);
        }

        public void DeleteTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = GetTemplatesDirectoryPath();
            var templatePath = PathUtils.Combine(directoryPath, name);
            DirectoryUtils.DeleteDirectoryIfExists(templatePath);
        }
    }
}
