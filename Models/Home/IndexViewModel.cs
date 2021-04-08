using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleGrouping.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<PropertyModel> Rows { get; set; }
            = Array.Empty<PropertyModel>();

        public IEnumerable<string> YearMonths =>
            // get all the YearMonths in the result set
            Rows.SelectMany(x => x.YearMonths.Select(y => y.YearMonth))
                .Distinct()
                .OrderByDescending(x => x).ToList();
    }

    public class PropertyModel
    {
        public string PropertyId { get; set; }

        public IEnumerable<YearMonthModel> YearMonths { get; set; }
            = Array.Empty<YearMonthModel>();
    }

    public class YearMonthModel
    {
        public string YearMonth { get; set; }
        // in case there are multiple, I don't think so
        public IEnumerable<OverrideModel> Amounts { get; set; }
    }
    
    public class OverrideModel 
    {
        public int OverrideId { get; set; }
        // really should be decimal
        // for better precision
        public float? Amount { get; set; }
    }
}