using System.ComponentModel;

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

    public enum OpportunityStateCode
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Open = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Won = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lost = 2
    };

    public enum QuoteStateCode
    {
        Draft = 0,         // The quote is in draft state.
        Active = 1,        // The quote is active.
        Won = 2,           // The quote has been won.
        Closed = 3,        // The quote has been closed.
    }

    public enum QuoteStatusCode
    {
        InProgress_Draft = 1,         
        InProgress_Active = 2,         
        Open = 3,               
        Won = 4,
        Lost = 5,
        Canceled = 6,
        Revised = 7
    }

    public enum LeadStatusCode {
        [System.Runtime.Serialization.EnumMemberAttribute()]
        [Description("Canceled status")]
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
