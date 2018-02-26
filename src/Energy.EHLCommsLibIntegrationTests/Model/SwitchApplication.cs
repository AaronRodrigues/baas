using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    public class SwitchApplication
    {
        public SwitchApplication()
        {
            CreatedTime = DateTime.Now;
        }

        public int Id { get; set; }
        public string CompareWhat { get; set; }
        public Guid SwitchId { get; set; }
        public string EhlReference { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string HeldApplication {get; set;}
        public DateTime? FutureSwitchDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime ConfirmDate { get; set; }
        public string EmailAddress { get; set; }
        public string SupplyAddress { get; set; }
        public string CorrespondenceAddress { get; set; }
        public string PhoneDayTime { get; set; }
        public bool ContactAllowedByEmail { get; set; }
        public bool ContactAllowedByPost { get; set; }
        public bool ContactAllowedByTelephone { get; set; }
        public bool ContactAllowedBySms { get; set; }
        public string NextSteps { get; set; }
        public string FutureSupplier { get; set; }
        public string FutureTariff { get; set; }
        public string TariffDetails { get; set; }
        public DateTime? TariffEndDate { get; set; }
    }
}