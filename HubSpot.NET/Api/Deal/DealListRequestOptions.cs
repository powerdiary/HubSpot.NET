using System.Collections.Generic;
using HubSpot.NET.Core;

namespace HubSpot.NET.Api.Deal
{
    public class DealListRequestOptions : ListRequestOptions
    {
        public List<string> Associations { get; set; } = new List<string>();
    }
}