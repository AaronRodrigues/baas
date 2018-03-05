using System;

namespace Energy.EHLCommsLibIntegrationTests.Model
{
    [Serializable]
    public class OptionItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object MetaData { get; set; }
        public bool Data { get; set; }
    }
}