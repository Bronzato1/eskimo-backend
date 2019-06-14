using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace API.Models
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogContext _context;

        public BlogRepository(BlogContext context)
        {
            _context = context;
        }

        // P O S T S

        public void CreatePost(PostItem item)
        {
            _context.PostItems.Add(item);
            _context.SaveChanges();
        }

        public IEnumerable<PostItem> GetAllPosts()
        {
            return _context.PostItems.Include(x => x.Category).Include(x => x.Tags).ToList();
        }

        public PostItem GetPost(int id)
        {
            var result = _context.PostItems.Include(x => x.Category).Include(x => x.Tags).Where(t => t.Id == id).FirstOrDefault();
            return result;
        }

        public void UpdatePost(PostItem item)
        {
            _context.PostItems.Update(item);
            _context.SaveChanges();
        }

        public void DeletePost(int id)
        {
            var entity = _context.PostItems.First(t => t.Id == id);
            if (entity != null)
            {
                _context.PostItems.Remove(entity);
                _context.SaveChanges();
            }
        }

        // T A G S

        public void CreateTag(Tag item)
        {
            _context.Tags.Add(item);
            _context.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _context.Tags.ToList();
        }

        public Tag GetTag(int id)
        {
            var result = _context.Tags.Where(t => t.Id == id).FirstOrDefault();
            return result;
        }

        public void UpdateTag(int postId, string tagOldName, string tagNewName)
        {
            var entity = _context.Tags.Where(x => x.PostItemId == postId && x.Name == tagOldName).SingleOrDefault();
            if (entity != null)
            {
                entity.Name = tagNewName;
                _context.SaveChanges();
            }
        }

        public void DeleteTag(int postId, string tagName)
        {
            var entity = _context.Tags.Where(x => x.PostItemId == postId && x.Name == tagName).SingleOrDefault();
            if (entity != null)
            {
                _context.Tags.Remove(entity);
                _context.SaveChanges();
            }
        }

        // C A T E G O R I E S

        public void CreateCategory(Category item)
        {
            _context.Categories.Add(item);
            _context.SaveChanges();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            var result = _context.Categories.Where(t => t.Id == id).FirstOrDefault();
            return result;
        }

        public void UpdateCategory(Category item)
        {
            _context.Categories.Update(item);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var cat = _context.Categories.Where(x => x.Id == id).SingleOrDefault();
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                _context.SaveChanges();
            }
        }
    }
}
