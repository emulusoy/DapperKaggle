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
        public async Task<int> GetMatchesCountAsync(string compId, int? clubId, string? result, string? hosting)
        {
            const string sql = @"
                                    SELECT COUNT(1)
                                    FROM dbo.club_games g
                                    JOIN dbo.clubs c  ON c.club_id  = g.club_id
                                    WHERE c.domestic_competition_id = @compId
                                      AND (@clubId IS NULL OR g.club_id = @clubId)
                                      AND (@hosting IS NULL OR g.hosting = @hosting)
                                      AND (@result IS NULL OR
                                          CASE WHEN g.own_goals > g.opponent_goals THEN 'W'
                                               WHEN g.own_goals = g.opponent_goals THEN 'D'
                                               ELSE 'L' END = @result);";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(sql, new { compId, clubId, hosting, result });
        }

        public async Task<List<ClubGameDto>> GetMatchesAsync(string compId, int? clubId, string? result, string? hosting, int page, int pageSize)
        {
            const string sql = @"
                                SELECT
                                  g.game_id, g.club_id, c.name AS club_name,
                                  g.own_goals, g.own_position,
                                  g.opponent_id, c2.name AS opponent_name, g.opponent_goals, g.opponent_position,
                                  g.own_manager_name, g.opponent_manager_name,
                                  g.hosting, g.is_win,
                                  CASE WHEN g.own_goals > g.opponent_goals THEN 'W'
                                       WHEN g.own_goals = g.opponent_goals THEN 'D'
                                       ELSE 'L' END AS result
                                FROM dbo.club_games g
                                JOIN dbo.clubs c  ON c.club_id  = g.club_id
                                JOIN dbo.clubs c2 ON c2.club_id = g.opponent_id
                                WHERE c.domestic_competition_id = @compId
                                  AND (@clubId IS NULL OR g.club_id = @clubId)
                                  AND (@hosting IS NULL OR g.hosting = @hosting)
                                  AND (@result IS NULL OR
                                      CASE WHEN g.own_goals > g.opponent_goals THEN 'W'
                                           WHEN g.own_goals = g.opponent_goals THEN 'D'
                                           ELSE 'L' END = @result)
                                ORDER BY g.game_id DESC
                                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            using var con = _context.CreateConnection();
            var rows = await con.QueryAsync<ClubGameDto>(sql, new
            {
                compId,
                clubId,
                hosting,
                result,
                offset = (page - 1) * pageSize,
                pageSize
            }, commandType: CommandType.Text);

            return rows.ToList();
        }
    }
}
