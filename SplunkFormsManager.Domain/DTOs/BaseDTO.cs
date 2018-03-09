using System;
using System.Collections.Generic;
using System.Text;

namespace SplunkFormsManager.Domain.DTOs
{
    public class BaseDTO
    {
        public long Id { get; set; }
        public bool MustDelete { get; set; }
    }
}
