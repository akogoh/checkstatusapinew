namespace checkstastusapinew.Model
{
    public class RiskRecord
    {
        public int Id { get; set; }
        public int? RiskRegisterId { get; set; }
        public string RiskCode { get; set; }
        public string RiskName { get; set; }
        public string RiskDescription { get; set; }
        public string RiskCategory { get; set; }
        public string Directorate { get; set; }
        public string RiskOwner { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string RegionId { get; set; }
        public string DistrictId { get; set; }
        public string Landmark { get; set; }
        public string PhoneNumber { get; set; }
        public int? Frequency { get; set; }
        public string FrequencyPeriod { get; set; }
        public int? Impact { get; set; }
        public int? RiskScore { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int? RiskChampionId { get; set; }
        public int? ChampionAcknowledged { get; set; }
        public DateTime? ChampionAcknowledgedDate { get; set; }
        public string ChampionFeedback { get; set; }
        public int? StaffAssignedId { get; set; }
        public string StaffAssignedName { get; set; }
        public int? StaffAcknowledged { get; set; }
        public DateTime? StaffAcknowledgedDate { get; set; }
        public string StaffFeedback { get; set; }
        public int? StaffCompleted { get; set; }
        public DateTime? StaffCompletedDate { get; set; }
        public string StaffEvidenceUrls { get; set; }
        public DateTime? StaffReturnedRiskDate { get; set; }
        public int? CorporateChampionId { get; set; }
        public string CorporateChampionName { get; set; }
        public DateTime? CorporateEscalatedDate { get; set; }
        public string CorporateEscalationNote { get; set; }
        public string CorporateChampionComment { get; set; }
        public DateTime? CorporateChampionCommentDate { get; set; }
        public string CorporateOwnerComment { get; set; }
        public DateTime? CorporateOwnerCommentDate { get; set; }
        public string OwnerFeedback { get; set; }
        public string RegionalRepFeedback { get; set; }
        public string Status { get; set; }
        public int? IsLocked { get; set; }
        public string ImageUrls { get; set; }
        public string MitigationNotes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
