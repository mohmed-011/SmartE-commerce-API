using Microsoft.AspNetCore.Mvc;
using SmartE_commerce.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("GetAllCategory")]
        public ActionResult<IEnumerable<Category>> Get()
        {
            var records = _dbContext.Set<Category>().ToList();
            return Ok(records);
        }
    }
}
