namespace RevisioneNew.Models
{
    public enum LeadStateCode
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Open = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Qualified = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Disqualified = 2
    };

    public enum LeadStatusCode {
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Canceled = 7,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lost = 4,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Qualified = 3
    }
    public class OptionSets
    {
    }
}
