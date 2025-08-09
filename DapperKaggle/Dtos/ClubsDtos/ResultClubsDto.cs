namespace DapperKaggle.Dtos.ClubsDtos
{
    public class ResultClubsDto
    {
        public long club_Id { get; set; }   
        public string? club_Code { get; set; }   
        public string? name { get; set; }   
        public string? domestic_Competitio_Id { get; set; }   

        public string? total_Market_Value { get; set; }  
        public int? squad_Size { get; set; }   
        public decimal? average_Age { get; set; }   
        public int? foreigners_Number { get; set; }   
        public decimal? foreigners_Percentage { get; set; }  
        public int? national_Team_Players { get; set; }   

        public string? stadium_Name { get; set; }  
        public int? stadium_Seats { get; set; }   
        public string? net_Transfer_Record { get; set; }   

        public string? coach_Name { get; set; }   
        public int? last_Season { get; set; }   

        public string? filename { get; set; }  
        public string? url { get; set; }   
    }
}
