using DapperKaggle.Repositories.Abstact;
using Microsoft.AspNetCore.Mvc;

namespace DapperKaggle.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IClubsService _clubsService;
        private readonly IPlayersService _playersService;
        private readonly IDashboardService _dashboardService;

        public DefaultController(IClubsService clubsService, IPlayersService playersService, IDashboardService dashboardService)
        {
            _clubsService = clubsService;
            _playersService = playersService;
            _dashboardService = dashboardService;
        }
        [HttpGet]

        public async Task<IActionResult> Dashboard(string compId = "TR1", long? clubId = null)
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TR1", "GB1", "ES1" };
            if (!allowed.Contains(compId)) compId = "TR1";

            var model = await _dashboardService.GetDashboardAsync(compId, clubId, 10, 10, 5);
            ViewBag.CompId = compId;
            ViewBag.ClubId = clubId;
            return View(model); 
        }
        [HttpGet]
        public async Task<IActionResult> Clubs(string compId = "TR1", string? q = null, int page = 1, int pageSize = 20)
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TR1", "GB1", "ES1" };
            if (!allowed.Contains(compId)) compId = "TR1";
            if (page < 1) page = 1;
            if (pageSize < 5 || pageSize > 100) pageSize = 20;
            var values = await _clubsService.GetAllClubsAsync(compId, q, page, pageSize);
            var totalCount = await _clubsService.GetClubsCountAsync(compId, q);
            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Q = q;
            ViewBag.CompId = compId; 

            return View(values);
        }
        public async Task<IActionResult> Players(long clubId)
        {
            var players = await _playersService.GetByClubAsync(clubId);
            var clubName = await _playersService.GetClubNameAsync(clubId);
            ViewBag.ClubId = clubId;
            ViewBag.ClubName = clubName ?? players.FirstOrDefault()?.current_club_name ?? "Club";
            return View(players); 
        }
        [HttpGet]
        public async Task<IActionResult> Matches(string compId = "TR1", int? clubId = null,
                                         string? result = null, string? hosting = null,
                                         int page = 1, int pageSize = 20)
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TR1", "GB1", "ES1" };
            if (!allowed.Contains(compId)) compId = "TR1";
            if (page < 1) page = 1;
            if (pageSize is < 5 or > 100) pageSize = 20;

            var total = await _clubsService.GetMatchesCountAsync(compId, clubId, result, hosting);
            var rows = await _clubsService.GetMatchesAsync(compId, clubId, result, hosting, page, pageSize);
            var clubs = await _clubsService.GetAllClubsAsync(compId, null, 1, 500); 

            ViewBag.CompId = compId;
            ViewBag.Clubs = clubs;     
            ViewBag.ClubId = clubId;
            ViewBag.Result = result;   
            ViewBag.Hosting = hosting;  
            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(rows); 
        }
    }
}
