using System.Collections.Generic;

namespace API.Models
{
    public interface IBlogRepository
    {
        // P O S T S

        void CreatePost(PostItem item);                         // CREATE
        IEnumerable<PostItem> GetPosts();                       // READ
        IEnumerable<PostItem> GetPostsWithPagination(int id);   // READ
        IEnumerable<PostItem> GetPostsInFavorites();            // READ
        PostItem GetPost(int id);                               // READ
        void UpdatePost(PostItem item);                         // UPDATE
        void AddPostToFavorite(int id);                         // UPDATE
        void RemovePostFromFavorite(int id);                    // UPDATE
        void DeletePost(int id);                                // DELETE
        
        // T A G S

        void CreateTag(Tag item);                                                           // CREATE
        IEnumerable<Tag> GetAllTags();                                                      // READ
        Tag GetTag(int id);                                                                 // READ
        void UpdateTag(int postId, string tagOldName, string tagNewName, string language);  // UPDATE
        void DeleteTag(int postId, string tagName);                                         // DELETE

        // C A T E G O R I E S

        void CreateCategory(Category item);         // CREATE
        IEnumerable<Category> GetAllCategories();   // READ
        Category GetCategory(int id);               // READ
        void UpdateCategory(Category item);         // UPDATE
        void DeleteCategory(int categoryId);        // DELETE
    }
}
