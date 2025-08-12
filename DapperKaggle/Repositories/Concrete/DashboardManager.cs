using System.Text.RegularExpressions;
using Dapper;
using DapperKaggle.Context;
using DapperKaggle.Dtos.DashboardDtos;
using DapperKaggle.Repositories.Abstact;
using Microsoft.AspNetCore.SignalR;

namespace DapperKaggle.Repositories.Concrete
{
    public class DashboardManager : IDashboardService
    {
        private readonly DapperKaggleContext _ctx;
        public DashboardManager(DapperKaggleContext ctx)
        {
            _ctx = ctx;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        private sealed class PlayerRow
        {
            public long player_id { get; set; }
            public string? name { get; set; }
            public string? position { get; set; }
            public string? sub_position { get; set; }  
            public string? foot { get; set; }
            public long? market_value_in_eur { get; set; }
            public long? current_club_id { get; set; }
            public string? current_club_name { get; set; }
            public string? image_url { get; set; }
            public DateTime? date_of_birth { get; set; }
        }

        private sealed class ClubRow
        {
            public long club_id { get; set; }
            public string? name { get; set; }
            public object? total_market_value { get; set; }
            public decimal? foreigners_percentage { get; set; }
            public int? national_team_players { get; set; } 
            public int? squad_size { get; set; }           
            public decimal? average_age { get; set; }          
        }

        public async Task<DashboardDto> GetDashboardAsync(string compId, long? clubId, int topPlayers = 10, int topClubs = 10, int clubTop = 5)
        {
            const string playersSql = @"
SELECT player_id, name, position, sub_position, foot, market_value_in_eur,
       current_club_id, current_club_name, image_url, date_of_birth
FROM dbo.players
WHERE current_club_domestic_competition_id = @compId;";

            const string clubsSql = @"
SELECT club_id, name, total_market_value, foreigners_percentage,
       national_team_players, squad_size, average_age
FROM dbo.clubs
WHERE domestic_competition_id = @compId;";


            using var con = _ctx.CreateConnection();

            var players = (await con.QueryAsync<PlayerRow>(playersSql, new { compId })).ToList();
            var clubs = (await con.QueryAsync<ClubRow>(clubsSql, new { compId })).ToList();
            var dto = new DashboardDto
            {
                total_players = players.Count,
                total_clubs = clubs.Count
            };
            string MapPos(PlayerRow p)
            {
                var pos = (p.position ?? p.sub_position ?? "").Trim();
               
                if (string.IsNullOrEmpty(pos)) return "Bilinmiyor";
                pos = pos.ToLowerInvariant();
                if (pos.Contains("goal")) return "Goalkeeper";
                if (pos.Contains("def")) return "Defender";
                if (pos.Contains("mid")) return "Midfield";
                if (pos.Contains("att") || pos.Contains("forw") || pos.Contains("wing")) return "Attack";
                return char.ToUpper(pos[0]) + pos.Substring(1);
            }

            dto.position_counts = players
                .GroupBy(p => MapPos(p))
                .Select(g => new label_count { label = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToList();

            dto.club_nat_players_top = clubs
                .Where(c => (c.national_team_players ?? 0) > 0)
                .OrderByDescending(c => c.national_team_players ?? 0)
                .ThenBy(c => c.name)
                .Take(10)
                .Select(c => new club_nat
                {
                    club_id = c.club_id,
                    name = c.name ?? "",
                    national_team_players = c.national_team_players ?? 0
                })
                .ToList();


            dto.total_market_value = clubs.Sum(c => ParseMoneyToLong(c.total_market_value));

            var ages = players
                .Where(p => p.date_of_birth.HasValue)
                .Select(p => Age(p.date_of_birth!.Value))
                .Where(a => a.HasValue)
                .Select(a => (double)a!.Value)
                .ToList();
            dto.avg_age = ages.Count > 0 ? Math.Round((decimal)ages.Average(), 1) : 0m;

            var foreignList = clubs.Where(c => c.foreigners_percentage.HasValue).Select(c => (double)c.foreigners_percentage!.Value).ToList();
            dto.foreigners_avg = foreignList.Count > 0 ? (decimal)Math.Round(foreignList.Average(), 2) : 0m;

            dto.top_players = players
                .OrderByDescending(p => p.market_value_in_eur ?? 0)
                .Take(topPlayers)
                .Select(p => new mini_player
                {
                    player_id = p.player_id,
                    name = p.name,
                    position = p.position,
                    market_value_in_eur = p.market_value_in_eur,
                    current_club_id = p.current_club_id,
                    current_club_name = p.current_club_name,
                    image_url = p.image_url
                }).ToList();

            dto.top_clubs = clubs
                .OrderByDescending(c => ParseMoneyToLong(c.total_market_value))
                .Take(topClubs)
                .Select(c => new top_club
                {
                    club_id = c.club_id,
                    name = c.name,
                    total_market_value = ParseMoneyToLong(c.total_market_value)
                }).ToList();


            dto.age_buckets = BucketizeAges(ages);


            dto.clubs = clubs.Select(c => new club_mini { club_id = c.club_id, name = c.name }).OrderBy(c => c.name).ToList();

            dto.club_top_players = clubId.HasValue
                ? players.Where(p => p.current_club_id == clubId)
                         .OrderByDescending(p => p.market_value_in_eur ?? 0)
                         .Take(clubTop)
                         .Select(p => new mini_player
                         {
                             player_id = p.player_id,
                             name = p.name,
                             market_value_in_eur = p.market_value_in_eur,
                             image_url = p.image_url,
                             position = p.position,
                             current_club_id = p.current_club_id,
                             current_club_name = p.current_club_name
                         }).ToList()
                : new List<mini_player>();

            return dto;
        }

        private static long ParseMoneyToLong(object? raw)
        {
            if (raw == null) return 0L;

            if (raw is long l) return l;
            if (raw is int i) return i;
            if (raw is decimal d) return (long)d;
            if (raw is double f) return (long)f;

            var s = raw.ToString()?.Trim();
            if (string.IsNullOrEmpty(s)) return 0L;

            s = s.ToLowerInvariant();

            var mult = 1_000_000L;
            if (s.EndsWith("k")) { mult = 1_000L; s = s[..^1]; }
            else if (s.EndsWith("m")) { mult = 1_000_000L; s = s[..^1]; }
            else if (s.EndsWith("b")) { mult = 1_000_000_000L; s = s[..^1]; }

            s = Regex.Replace(s, @"[^\d\.\-]", ""); // para işaretlerini sil
            if (string.IsNullOrWhiteSpace(s)) return 0L;

            if (decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                 System.Globalization.CultureInfo.InvariantCulture, out var val))
            {

                if (!raw.ToString()!.ToLower().EndsWith("k") &&
                    !raw.ToString()!.ToLower().EndsWith("m") &&
                    !raw.ToString()!.ToLower().EndsWith("b"))
                    return (long)Math.Round(val);

                return (long)Math.Round(val * mult);
            }
            return 0L;
        }

        private static int? Age(DateTime dob)
        {
            var today = DateTime.Today;
            var a = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-a)) a--;
            return a >= 0 ? a : null;
        }

        private static List<age_bucket> BucketizeAges(List<double> ages)
        {
            var res = new Dictionary<string, int>
            {
                ["≤20"] = 0,
                ["21–23"] = 0,
                ["24–26"] = 0,
                ["27–29"] = 0,
                ["30–32"] = 0,
                ["33+"] = 0
            };
            foreach (var ag in ages)
            {
                if (ag <= 20) res["≤20"]++;
                else if (ag <= 23) res["21–23"]++;
                else if (ag <= 26) res["24–26"]++;
                else if (ag <= 29) res["27–29"]++;
                else if (ag <= 32) res["30–32"]++;
                else res["33+"]++;
            }
            return res.Select(kv => new age_bucket { bucket = kv.Key, count = kv.Value }).ToList();
        }
    }
}
