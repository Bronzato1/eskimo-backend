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
    public class AuthorController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly BlogContext _blogcontext;
        private IBlogRepository _blogRepository { get; set; }

        public AuthorController(IHostingEnvironment hostingEnvironment, BlogContext blogContext, IBlogRepository blogRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _blogcontext = blogContext;
            _blogRepository = blogRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] Author item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            _blogRepository.CreateAuthor(item);
            return CreatedAtRoute("GetAuthor", new { id = item.Id }, item);
        }

        [HttpGet]
        public IEnumerable<Author> GetAll()
        {
            return _blogRepository.GetAllAuthors();
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetById(int id)
        {
            var item = _blogRepository.GetAuthor(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpGet("GetAuthorByName/{name}")]
        public IActionResult GetByName(string name)
        {
            var item = _blogRepository.GetAuthorByName(name);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAuthor(int id, [FromBody] Author item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var author = _blogRepository.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            author.Name = item.Name;
            author.Url = item.Url;

            _blogRepository.UpdateAuthor(author);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            _blogRepository.DeleteAuthor(id);
            return new NoContentResult();
        }
    }
}
