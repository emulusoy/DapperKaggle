namespace DapperKaggle.Dtos.PlayerDtos
{
    public class ResultPlayersDto
    {
      
            public long player_id { get; set; }
            public string? first_name { get; set; }
            public string? last_name { get; set; }
            public string? name { get; set; }
            public short? last_season { get; set; }
            public long? current_club_id { get; set; }
            public string? player_code { get; set; }
            public string? country_of_birth { get; set; }
            public string? city_of_birth { get; set; }
            public string? country_of_citizenship { get; set; }
            public DateTime? date_of_birth { get; set; }
            public string? sub_position { get; set; }
            public string? position { get; set; }
            public string? foot { get; set; }
            public byte? height_in_cm { get; set; }
            public DateTime? contract_expiration_date { get; set; }
            public string? agent_name { get; set; }
            public string? image_url { get; set; }
            public string? url { get; set; }
            public string? current_club_domestic_competition_id { get; set; }
            public string? current_club_name { get; set; }
            public long? market_value_in_eur { get; set; }
            public long? highest_market_value_in_eur { get; set; }
        

    }
}
