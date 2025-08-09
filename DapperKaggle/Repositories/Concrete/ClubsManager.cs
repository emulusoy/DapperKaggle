using System.Data;
using Dapper;
using DapperKaggle.Context;
using DapperKaggle.Dtos.ClubsDtos;
using DapperKaggle.Repositories.Abstact;

namespace DapperKaggle.Repositories.Concrete
{
    public class ClubsManager : IClubsService
    {
        private readonly DapperKaggleContext _context;

        public ClubsManager(DapperKaggleContext context)
        {
            _context = context;
        }

        public async Task DeleteClubs(int clubsId)
        {
            string sql = "DELETE FROM clubs WHERE club_id  = @clubsId";
            var parameters = new DynamicParameters();
            parameters.Add("clubsId", clubsId);
            var conn=  _context.CreateConnection();
            await conn.ExecuteAsync(sql, parameters);
        }

        // Geriye uyumluluk: istersen TR1 ilk sayfa 20 kayıt dönsün
        public Task<List<ResultClubsDto>> GetAllClubsAsync()
            => GetAllClubsAsync("TR1", null, 1, 20);

        public async Task<List<ResultClubsDto>> GetAllClubsAsync(string compId, string? q, int page, int pageSize)
        {
            const string sql = @"
                                    SELECT club_id, club_code, name, domestic_competition_id, total_market_value, squad_size,
                                           average_age, foreigners_number, foreigners_percentage, national_team_players,
                                           stadium_name, stadium_seats, net_transfer_record, coach_name, last_season, filename, url
                                    FROM dbo.clubs
                                    WHERE domestic_competition_id = @compId
                                      AND (@q IS NULL OR name LIKE '%' + @q + '%')
                                    ORDER BY total_market_value DESC, name
                                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            using var connection = _context.CreateConnection();
            var list = await connection.QueryAsync<ResultClubsDto>(
                sql,
                new
                {
                    compId,
                    q = string.IsNullOrWhiteSpace(q) ? null : q.Trim(),
                    offset = (page - 1) * pageSize,
                    pageSize
                },
                commandType: CommandType.Text
            );
            return list.ToList();
        }

        public async Task<int> GetClubsCountAsync(string compId, string? q)
        {
            const string sql = @"
                                SELECT COUNT(1)
                                FROM dbo.clubs
                                WHERE domestic_competition_id = @compId
                                  AND (@q IS NULL OR name LIKE '%' + @q + '%');";

            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { compId, q = string.IsNullOrWhiteSpace(q) ? null : q.Trim() },
                commandType: CommandType.Text
            );
            return count;
        }

        public Task<GetClubsByIdDto> GetClubsById(int id)
        {
            string sql = "SELECT * FROM clubs Where club_id =@id"; 
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            var connn=_context.CreateConnection();
            var value = connn.QueryFirstAsync<GetClubsByIdDto>(sql, parameters);
            return value;
        }
    }
}
