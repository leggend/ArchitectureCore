using System;
using System.Collections.Generic;
using System.Text;

namespace SplunkFormsManager.CrossCutting.Utilities
{
    public class CommonFilter
    {
        public bool ResultList { get; set; }
        public int? IndexPage { get; set; }
        public int? ItemsPerPage { get; set; }
        public string[] Includes { get; set; }
        public string JSonRules { get; set; }
        public string JSonOrders { get; set; }
        public LinqFilterRule[] FilterRules { get; set; }
        public LinqOrderRule[] OrderRules { get; set; }
    }
}
