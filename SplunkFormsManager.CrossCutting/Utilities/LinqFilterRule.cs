using System;
using System.Collections.Generic;
using System.Text;

namespace SplunkFormsManager.CrossCutting.Utilities
{
    public class LinqFilterRule
    {
        public string Property { get; set; }
        public string Comparison { get; set; }
        public object Value { get; set; }
    }
}
