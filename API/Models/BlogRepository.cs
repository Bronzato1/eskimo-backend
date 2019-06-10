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

        public IEnumerable<PostItem> GetAll()
        {
            return _context.PostItems.Include(x => x.Tags).ToList();
        }

        public void Add(PostItem item)
        {
            _context.PostItems.Add(item);
            _context.SaveChanges();
        }

        public PostItem Find(int id)
        {
            var result = _context.PostItems.Include(x => x.Tags).Where(t => t.Id == id).FirstOrDefault();
            return result;
        }

        public void Remove(int id)
        {
            var entity = _context.PostItems.First(t => t.Id == id);
            if (entity != null)
            {
                _context.PostItems.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Update(PostItem item)
        {
            _context.PostItems.Update(item);
            _context.SaveChanges();
        }
    
        public void AddTag(int postId, string tagName) 
        {
            Tag tag = new Tag { PostItemId = postId, Name = tagName };
            _context.Tags.Add(tag);
            _context.SaveChanges();
        }

        public void RemoveTag(int postId, string tagName) 
        {
            var entity = _context.Tags.Where(x => x.PostItemId == postId && x.Name == tagName).SingleOrDefault();
            if (entity != null)
            {
                _context.Tags.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void ChangeTag(int postId, string tagOldName, string tagNewName) 
        {
            var entity = _context.Tags.Where(x => x.PostItemId == postId && x.Name == tagOldName).SingleOrDefault();
            if (entity != null)
            {
                entity.Name = tagNewName;
                _context.SaveChanges();
            }
        }
    }
}