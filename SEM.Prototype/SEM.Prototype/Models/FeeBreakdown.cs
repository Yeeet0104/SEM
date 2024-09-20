namespace SEM.Prototype.Models
{
    public class FeeBreakdown
    {
        public decimal BaseFee { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal CautionMoney { get; set; }
        public decimal InsurancePremium { get; set; }
        public decimal FacilitiesResourceFee { get; set; }
        public decimal LabWorkshopFee { get; set; }
        public decimal AwardAssessmentFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalFee { get; set; }
    }
}
