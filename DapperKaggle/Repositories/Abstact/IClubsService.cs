using DapperKaggle.Dtos.ClubsDtos;

namespace DapperKaggle.Repositories.Abstact
{
    public interface IClubsService
    {
        Task<List<ResultClubsDto>> GetAllClubsAsync(string compId, string? q, int page, int pageSize);
        
        // Toplam kayıt sayısı (pager için)
        Task<int> GetClubsCountAsync(string compId, string? q);
        Task DeleteClubs(int customerId);
        Task<GetClubsByIdDto> GetClubsById(int id);

        Task<List<ClubGameDto>> GetMatchesAsync(string compId, int? clubId, string? result, string? hosting, int page, int pageSize);
        Task<int> GetMatchesCountAsync(string compId, int? clubId, string? result, string? hosting);

    }
}
