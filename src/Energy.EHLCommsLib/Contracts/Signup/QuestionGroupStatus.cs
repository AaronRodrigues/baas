using System.Runtime.Serialization;

namespace Energy.EHLCommsLib.Contracts.Signup
{
    [DataContract]
    public class QuestionGroupStatus
    {
        public QuestionGroupStatus()
        {
        }

        public QuestionGroupStatus(string name, bool mustComplete, bool isComplete)
        {
            Name = name;
            MustComplete = mustComplete;
            IsComplete = isComplete;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "mustComplete")]
        public bool MustComplete { get; set; }

        [DataMember(Name = "isComplete")]
        public bool IsComplete { get; set; }
    }
}