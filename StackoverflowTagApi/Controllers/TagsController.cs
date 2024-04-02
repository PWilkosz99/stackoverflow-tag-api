using Microsoft.AspNetCore.Mvc;

namespace StackoverflowTagApi.Controllers
{
    public class TagsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
