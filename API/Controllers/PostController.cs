using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using API.Models;

namespace API.Controllers
{
    [EnableCors("AureliaSPA")]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly BlogContext _blogcontext;
        private IBlogRepository _blogRepository { get; set; }

        public PostController(IHostingEnvironment hostingEnvironment, BlogContext blogContext, IBlogRepository blogRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _blogcontext = blogContext;
            _blogRepository = blogRepository;
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] PostItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            _blogRepository.CreatePost(item);
            return CreatedAtRoute("GetPost", new { id = item.Id }, item);
        }

        [HttpGet]
        public IEnumerable<PostItem> GetAll()
        {
            return _blogRepository.GetAllPosts();
        }

        [HttpGet("{id}", Name = "GetPost")]
        public IActionResult GetById(int id)
        {
            var item = _blogRepository.GetPost(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, [FromBody] PostItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var post = _blogRepository.GetPost(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Title = item.Title;
            post.Content = item.Content;
            post.Creation = item.Creation;
            post.CategoryId = item.CategoryId;
            post.ReadingTime = item.ReadingTime;

            _blogRepository.UpdatePost(post);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _blogRepository.GetPost(id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Content != null)
            {
                var matches = Regex.Matches(post.Content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase); //.Groups[1].Value; 
                foreach (Match match in matches)
                {
                    string urlPath = match.Groups[1].Value;
                    string fileName = System.IO.Path.GetFileName(urlPath);
                    string fullPath = System.IO.Path.GetFullPath("wwwroot/uploads/" + fileName);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
            }

            _blogRepository.DeletePost(id);
            return new NoContentResult();
        }

        [HttpPost("DownloadZip")]
        public IActionResult DownloadZip([FromBody] List<int> ids)
        {
            string webrootPath = _hostingEnvironment.WebRootPath;
            string uploadPath = System.IO.Path.Combine(webrootPath, "uploads");
            string jsonFileName = "export.json";
            string jsonFilePath = System.IO.Path.Combine(webrootPath + @"\", jsonFileName);
            string zipFileName = string.Format("export-{0}.zip", DateTime.Now.ToString("yyyy-MM-dd"));
            string zipFilePath = System.IO.Path.Combine(webrootPath + @"\", zipFileName);

            DirectoryInfo di = new System.IO.DirectoryInfo(webrootPath);
            foreach (var file in di.EnumerateFiles("export*.*")) { file.Delete(); }

            Newtonsoft.Json.Linq.JArray json = new JArray(
                _blogRepository.GetAllPosts().Where(x => ids.Contains(x.Id)).Select(p => new JObject
                {
                    { "Title", p.Title},
                    { "Creation", p.Creation},
                    { "Content", p.Content}
                })
            );

            System.IO.File.WriteAllText(jsonFilePath, json.ToString());

            ZipFile.CreateFromDirectory(uploadPath, zipFilePath);

            using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                archive.CreateEntryFromFile(jsonFilePath, jsonFileName);
            }

            byte[] contents = System.IO.File.ReadAllBytes(zipFilePath);
            foreach (var file in di.EnumerateFiles("export*.*")) { file.Delete(); }
            return File(contents, "application/octetstream");
        }

        [HttpPost("UploadZip")]
        public async Task<IActionResult> UploadZip(IFormFile file)
        {
            long size = file.Length;

            string webrootPath = _hostingEnvironment.WebRootPath;
            string appRoot = Environment.CurrentDirectory;
            string uploadPath = System.IO.Path.Combine(webrootPath, "uploads");
            string jsonFilePath = System.IO.Path.Combine(uploadPath + @"\", @"export.json");
            string importFilePath = System.IO.Path.Combine(appRoot + @"\", @"App_Data\import.zip");

            if (file.Length == 0) throw new Exception("Le fichier est vide");
            if (file.FileName.EndsWith(".zip") == false) throw new Exception("Le type du fichier n'est pas valide");

            using (var stream = new System.IO.FileStream(importFilePath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (ZipArchive archive = ZipFile.OpenRead(importFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fileName = Path.Combine(uploadPath, entry.Name);
                    if (!System.IO.File.Exists(fileName))
                    {

                        entry.ExtractToFile(fileName);
                    }
                }
            }

            string json = System.IO.File.ReadAllText(jsonFilePath);

            var elements = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json).ToObject<List<JObject>>();

            foreach (var elm in elements)
            {
                Console.WriteLine("t = " + elm["Title"]);
                Console.WriteLine("l = " + elm["Content"]);
                Console.WriteLine("c = " + elm["Creation"]);

                PostItem postItem = new PostItem
                {
                    Title = (string)elm["Title"],
                    Content = (string)elm["Content"],
                    Creation = (DateTime)elm["Creation"]
                };

                _blogRepository.CreatePost(postItem);
            }

            if (System.IO.File.Exists(jsonFilePath))
                System.IO.File.Delete(jsonFilePath);

            if (System.IO.File.Exists(importFilePath))
                System.IO.File.Delete(importFilePath);

            return Ok(new { count = elements.Count });
        }

        [HttpGet("ClearAllPosts")]
        public IActionResult ClearAllPosts()
        {
            var rowsAffectedCount = _blogcontext.Database.ExecuteSqlCommand("delete from PostItems");
            return Ok(new { count = rowsAffectedCount });
        }
    }
}
