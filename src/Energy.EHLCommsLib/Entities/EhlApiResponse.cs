namespace Energy.EHLCommsLib.Entities
{
    public class EhlApiResponse
    {
        public string ConcatenatedErrorString { get; set; }
        public bool ApiCallWasSuccessful { get; set; }
        public string ApiStage { get; set; }
        public string NextUrl { get; set; }
    }
}