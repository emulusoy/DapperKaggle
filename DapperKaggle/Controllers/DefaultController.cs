using DapperKaggle.Repositories.Abstact;
using Microsoft.AspNetCore.Mvc;

namespace DapperKaggle.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IClubsService _clubsService;

        public DefaultController(IClubsService clubsService)
        {
            _clubsService = clubsService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Clubs(string? q, int page = 1, int pageSize = 20)
        {
            const string compId = "TR1";
            if (page < 1) page = 1;
            if (pageSize < 5 || pageSize > 100) pageSize = 20;

            var values = await _clubsService.GetAllClubsAsync(compId, q, page, pageSize);
            var totalCount = await _clubsService.GetClubsCountAsync(compId, q);

            // ViewBag ile sayfalama bilgisini view'a geçir (view tarafında kullanacağız)
            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Q = q;

            return View(values); // List<ResultClubsDto>
        }
    }
}
