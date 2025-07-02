using Microsoft.AspNetCore.Mvc;

namespace MVCPractice.Components
{
    public class SidebarViewComponent:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = new List<string> { "Post 1", "Post 2", "Post 3" }; // Sample data
            return View(posts); // Pass data to the view
        }
    }
}
