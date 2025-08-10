using DapperKaggle.Dtos.PlayerDtos;

namespace DapperKaggle.Repositories.Abstact
{
    public interface IPlayersService
    {
        Task<List<ResultPlayersDto>> GetByClubAsync(long clubId);
        Task<string?> GetClubNameAsync(long clubId);
    }
}
