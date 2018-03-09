using System;
using System.Collections.Generic;
using System.Text;

namespace SplunkFormsManager.Domain.DTOs
{
    public class PageListDTO<D> where D : BaseDTO
    {
        public int ItemsPerPages { set; get; }
        public int TotalPages { set; get; }
        public int CurrentPage { get; set; }
        public ICollection<D> Items { get; set; }
    }
}
