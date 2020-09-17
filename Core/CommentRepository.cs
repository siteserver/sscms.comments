using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Comments.Abstractions;
using SSCMS.Comments.Models;
using SSCMS.Comments.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Comments.Core
{
    public class CommentRepository : ICommentRepository
    {
        private readonly Repository<Comment> _repository;
        private const string ExtendValues = "ExtendValues";

        public CommentRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Comment>(settingsManager.Database, settingsManager.Redis);
        }

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Comment comment)
        {
            return await _repository.InsertAsync(comment);
        }

        public async Task UpdateAsync(Comment comment)
        {
            await _repository.UpdateAsync(comment);
        }

        public async Task<Comment> GetAsync(int commentId)
        {
            return await _repository.GetAsync(commentId);
        }

        public async Task DeleteBySiteIdAsync(int siteId)
        {
            if (siteId <= 0) return;

            await _repository.DeleteAsync(Q.Where(nameof(Comment.SiteId), siteId));
        }

        public async Task DeleteAsync(int commentId)
        {
            await _repository.DeleteAsync(commentId);
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await _repository.CountAsync(Q.Where(nameof(Comment.SiteId), siteId));
        }

        public async Task<(int Total, List<Comment>)> GetCommentsAsync(int siteId, int contentId, string word, int page, int pageSize)
        {
            if (page == 0) page = 1;

            var q = Q
                .Where(nameof(Comment.SiteId), siteId)
                .OrderByDesc(nameof(Comment.Id))
                .ForPage(page, pageSize);

            if (contentId > 0)
            {
                q.Where(nameof(Comment.ContentId), contentId);
            }

            if (!string.IsNullOrEmpty(word))
            {
                q.Where(query => query
                    .WhereLike(nameof(Comment.Content), $"%{word}%")
                    .OrWhereLike(ExtendValues, $"%{word}%")
                );
            }

            var count = await _repository.CountAsync(q);
            var list = await _repository.GetAllAsync(q);

            return (count, list);
        }

        public async Task<List<Comment>> GetCommentsAsync(int siteId, int contentId)
        {
            var q = Q
                .Where(nameof(Comment.SiteId), siteId)
                .Where(nameof(Comment.ContentId), contentId)
                .OrderByDesc(nameof(Comment.Id));

            return await _repository.GetAllAsync(q);
        }

        public string GetValue(TableStyle style, Comment comment)
        {
            var value = string.Empty;
            if (comment.ContainsKey(style.AttributeName))
            {
                var fieldValue = comment.Get<string>(style.AttributeName);

                if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
                {
                    var list = TranslateUtils.JsonDeserialize<List<string>>(fieldValue);
                    if (list != null)
                    {
                        value = string.Join(",", list);
                    }
                }
                else if (style.InputType == InputType.Date)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = CommentUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd");
                        }
                    }
                }
                else if (style.InputType == InputType.DateTime)
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        var date = CommentUtils.ToDateTime(fieldValue, DateTime.MinValue);
                        if (date != DateTime.MinValue)
                        {
                            value = date.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                else
                {
                    value = fieldValue;
                }
            }

            return value;
        }
    }
}
