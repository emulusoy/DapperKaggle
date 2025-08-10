namespace DapperKaggle.Dtos.ClubsDtos
{
    public class ClubGameDto
    {
        public int game_id { get; set; }
        public int club_id { get; set; }
        public string? club_name { get; set; }

        public byte? own_goals { get; set; }
        public byte? own_position { get; set; }

        public int opponent_id { get; set; }
        public string? opponent_name { get; set; }
        public byte? opponent_goals { get; set; }
        public byte? opponent_position { get; set; }

        public string? own_manager_name { get; set; }
        public string? opponent_manager_name { get; set; }
        public string? hosting { get; set; }
        public bool is_win { get; set; }

        public string? result { get; set; }
    }
}
