using System.Collections.Generic;

namespace API.Models
{
    public interface IBlogRepository
    {
        void Add(PostItem item);
        IEnumerable<PostItem> GetAll();
        PostItem Find(int id);
        void Remove(int id);
        void Update(PostItem item);
        void AddTag(int postId, string tagName);
        void RemoveTag(int postId, string tagName);
        void ChangeTag(int postId, string tagOldName, string tagNewName);
    }
}