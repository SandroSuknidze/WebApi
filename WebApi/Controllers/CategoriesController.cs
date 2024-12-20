using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Packages;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(IPKG_CATEGORIES pKG_CATEGORIES) : ControllerBase
    {
        private readonly IPKG_CATEGORIES pKG_CATEGORIES = pKG_CATEGORIES;

        [HttpGet]
        public IEnumerable<Category> Get()
        {
            var categories = pKG_CATEGORIES.GetCategories();

            return categories;
        }
    }
}
