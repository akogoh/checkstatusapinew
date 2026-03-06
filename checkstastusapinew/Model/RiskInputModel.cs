namespace checkstastusapinew.Model
{
    public class RiskInputModel
    {
        public int? RiskRegisterId { get; set; }
        public string RiskName { get; set; }
        public string OtherRiskName { get; set; }
        public string RiskDescription { get; set; }
        public string RiskCategory { get; set; }
        public string Directorate { get; set; }
        public string RiskOwner { get; set; }
        public string FrequencyPeriod { get; set; }
        public int? FrequencyScore { get; set; }
        public int? Impact { get; set; }
        public int? InherentRiskScore { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string Landmark { get; set; }
        public string PhoneNumber { get; set; }
    }
}