namespace DapperKaggle.Dtos.DashboardDtos
{
    public class DashboardDto
    { // Özet
        public int total_players { get; set; }
        public int total_clubs { get; set; }
        public long total_market_value { get; set; }
        public decimal avg_age { get; set; }  
        public decimal foreigners_avg { get; set; }  

        // Listeler
        public List<mini_player> top_players { get; set; } = new();
        public List<top_club> top_clubs { get; set; } = new();
        public List<age_bucket> age_buckets { get; set; } = new();
        public List<club_mini> clubs { get; set; } = new();
        public List<mini_player> club_top_players { get; set; } = new();
        public List<label_count> position_counts { get; set; } = new();
        public List<club_nat> club_nat_players_top { get; set; } = new();

    }

    public sealed class mini_player
    {
        public long player_id { get; set; }
        public string? name { get; set; }
        public string? position { get; set; }
        public long? market_value_in_eur { get; set; }
        public long? current_club_id { get; set; }
        public string? current_club_name { get; set; }
        public string? image_url { get; set; }
    }

    public sealed class top_club
    {
        public long club_id { get; set; }
        public string? name { get; set; }
        public long? total_market_value { get; set; }
    }

    public sealed class age_bucket
    {
        public string bucket { get; set; } = "";
        public int count { get; set; }
    }

    public sealed class club_mini
    {
        public long club_id { get; set; }
        public string? name { get; set; }
    }
    public sealed class label_count
    {
        public string label { get; set; } = "";
        public int count { get; set; }
    }

    public sealed class club_nat
    {
        public long club_id { get; set; }
        public string name { get; set; } = "";
        public int national_team_players { get; set; }
    }

}
