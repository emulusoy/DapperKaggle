using DapperKaggle.Dtos.DashboardDtos;

namespace DapperKaggle.Repositories.Abstact
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(string compId, long? clubId, int topPlayers = 10, int topClubs = 10, int clubTop = 5);


    }
}
