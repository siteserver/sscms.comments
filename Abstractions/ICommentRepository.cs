using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Comments.Models;
using SSCMS.Models;

namespace SSCMS.Comments.Abstractions
{
    public interface ICommentRepository
    {
        List<TableColumn> TableColumns { get; }

        Task<int> InsertAsync(Comment comment);

        Task UpdateAsync(Comment comment);

        Task<Comment> GetAsync(int commentId);

        Task DeleteBySiteIdAsync(int siteId);

        Task DeleteAsync(int commentId);

        Task<int> GetCountAsync(int siteId);

        Task<(int Total, List<Comment>)> GetCommentsAsync(int siteId, int contentId, string word, int page, int pageSize);

        Task<List<Comment>> GetCommentsAsync(int siteId, int contentId);

        string GetValue(TableStyle style, Comment comment);
    }
}
