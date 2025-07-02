using System.Diagnostics;
using DBPractice.Data;
using DBPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBPractice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> AddAuthorWithPublisher(string AuthorName = "Sahil", string Title = "Hello World")
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var author = new Author { Name = AuthorName };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                    var book = new Book { Title = Title, AuthorId = author.Id};
                    await _context.Books.AddAsync(book);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok("Author and Book added successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Transaction failed.");
                    return StatusCode(500, "Transaction failed.");
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
