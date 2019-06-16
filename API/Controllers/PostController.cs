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

            post.Image = item.Image;
            post.Title = item.Title;
            post.Content = item.Content;
            post.Creation = item.Creation;
            post.CategoryId = item.CategoryId;
            post.ReadingTime = item.ReadingTime;

            _blogRepository.UpdatePost(post);
            return CreatedAtRoute("GetPost", new { id = item.Id }, item);
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

        [HttpPost("ExportZip")]
        public IActionResult ExportZip([FromBody] List<int> ids)
        {
            string webrootPath = _hostingEnvironment.WebRootPath;
            string uploadPath = System.IO.Path.Combine(webrootPath, "uploads");
            string exportPath = System.IO.Path.Combine(webrootPath, "export");
            string jsonFileName = "export.json";
            string jsonFilePath = System.IO.Path.Combine(exportPath + @"\", jsonFileName);
            string zipFileName = string.Format("export-{0}.zip", DateTime.Now.ToString("yyyy-MM-dd"));
            string zipFilePath = System.IO.Path.Combine(webrootPath + @"\", zipFileName);
            DirectoryInfo di;

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            di = new System.IO.DirectoryInfo(webrootPath);
            foreach (var file in di.EnumerateFiles("export*.*")) { file.Delete(); }

            di = new System.IO.DirectoryInfo(exportPath);
            foreach (var file in di.EnumerateFiles("*.*")) { file.Delete(); }

            // Newtonsoft.Json.Linq.JArray json = new JArray(
            //     _blogRepository.GetAllPosts().Where(x => ids.Contains(x.Id)).Select(p => new JObject
            //     {
            //         { "Title", p.Title },
            //         { "Creation", p.Creation },
            //         { "CategoryId", p.CategoryId },
            //         { "Category.FrenchName", p.Category.FrenchName },
            //         { "Category.EnglishName", p.Category.EnglishName },
            //         { "Image", p.Image },
            //         { "TagNames", string.Join(',', p.Tags) },
            //         { "Content", p.Content }
            //     })
            // );


            var data = _blogRepository.GetAllPosts().Where(x => ids.Contains(x.Id)).ToList();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented,
            new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            // var userData = new PostItem { Id = 1, Title = "My title" };
            // var userDataString = Newtonsoft.Json.JsonConvert.SerializeObject(userData);

            System.IO.File.WriteAllText(jsonFilePath, json.ToString());

            _blogRepository.GetAllPosts().Where(x => ids.Contains(x.Id)).ToList().ForEach(x =>
            {
                MatchCollection matches = Regex.Matches(x.Content, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
                matches.ToList().ForEach(match =>
                {
                    string fileName = Path.GetFileName(match.Groups[1].Value);
                    string sourceFileName = Path.Combine(uploadPath, fileName);
                    string destFileName = Path.Combine(exportPath, fileName);
                    System.IO.File.Copy(sourceFileName, destFileName);
                });
            });

            ZipFile.CreateFromDirectory(exportPath, zipFilePath);

            byte[] contents = System.IO.File.ReadAllBytes(zipFilePath);
            foreach (var file in di.EnumerateFiles("export*.*")) { file.Delete(); }
            return File(contents, "application/octetstream");
        }

        [HttpPost("ImportZip")]
        public async Task<IActionResult> ImportZip(IFormFile file)
        {
            long size = file.Length;

            string webrootPath = _hostingEnvironment.WebRootPath;
            string appRoot = Environment.CurrentDirectory;
            string uploadPath = System.IO.Path.Combine(webrootPath, "uploads");
            string jsonFilePath = System.IO.Path.Combine(uploadPath + @"\", @"export.json");
            string importFilePath = System.IO.Path.Combine(appRoot + @"\", @"App_Data\import.zip");
            List<string> errors = new List<string>();
            int countSucceed = 0;
            int countError = 0;

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

            var elements = (List<PostItem>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<PostItem>));

            foreach (var postItem in elements)
            {
                var postAlreadyExists = _blogcontext.PostItems.Any(x => x.Id == postItem.Id);

                if (postAlreadyExists)
                {
                    countError++;
                    errors.Add(string.Format("Le billet {0} existe déjà.", postItem.Id));
                    continue;
                }
                else
                {
                    countSucceed++;
                    var categoryAlreadyExists = _blogcontext.Categories.Any(x => x.Id == postItem.CategoryId);
                    if (categoryAlreadyExists) postItem.Category = null;
                    _blogRepository.CreatePost(postItem);
                }
            }

            if (System.IO.File.Exists(jsonFilePath))
                System.IO.File.Delete(jsonFilePath);

            if (System.IO.File.Exists(importFilePath))
                System.IO.File.Delete(importFilePath);

            return Ok(new { countSucceed = countSucceed, countError = countError, errors = errors });
        }

        [HttpGet("ClearAllPosts")]
        public IActionResult ClearAllPosts()
        {
            var rowsAffectedCount = _blogcontext.Database.ExecuteSqlCommand("delete from PostItems");
            return Ok(new { count = rowsAffectedCount });
        }
    }
}
