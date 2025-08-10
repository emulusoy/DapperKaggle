using Dapper;
using System.Data;
using DapperKaggle.Context;
using DapperKaggle.Dtos.PlayerDtos;
using DapperKaggle.Repositories.Abstact;

namespace DapperKaggle.Repositories.Concrete
{
    public class PlayersManager : IPlayersService
    {
        private readonly DapperKaggleContext _context;

        public PlayersManager(DapperKaggleContext context)
        {
            _context = context;
        }

        public async Task<List<ResultPlayersDto>> GetByClubAsync(long clubId)
        {
            const string sql = @"
                                SELECT player_id, first_name, last_name, name, last_season, current_club_id, player_code,
                                       country_of_birth, city_of_birth, country_of_citizenship, date_of_birth, sub_position,
                                       position, foot, height_in_cm, contract_expiration_date, agent_name, image_url, url,
                                       current_club_domestic_competition_id, current_club_name, market_value_in_eur, highest_market_value_in_eur
                                FROM dbo.players
                                WHERE current_club_id = @clubId
                                ORDER BY
                                  CASE position
                                     WHEN 'Goalkeeper' THEN 1
                                     WHEN 'Defender'   THEN 2
                                     WHEN 'Midfield'   THEN 3
                                     WHEN 'Attack'     THEN 4
                                     ELSE 5
                                  END, name;";

            using var con = _context.CreateConnection();
            var rows = await con.QueryAsync<ResultPlayersDto>(sql, new { clubId }, commandType: CommandType.Text);
            return rows.ToList();
        }

        public async Task<string?> GetClubNameAsync(long clubId)
        {
            const string sql = @"SELECT TOP 1 name FROM dbo.clubs WHERE club_id = @clubId;";
            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<string?>(sql, new { clubId });
        }
        
    }
}
