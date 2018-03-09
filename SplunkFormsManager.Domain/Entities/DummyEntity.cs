using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplunkFormsManager.Domain.Entities
{
    [Table("Dummies")]
    public class DummyEntity : BaseEntity
    {
        [Required, StringLength(10)]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
