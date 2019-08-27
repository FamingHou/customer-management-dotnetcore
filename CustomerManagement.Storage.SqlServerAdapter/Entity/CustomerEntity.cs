using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Storage.SqlServerAdapter.Entity
{
    [Table("Customers")]
    public class CustomerEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual List<CommentEntity> Comments { get; set; }
    }
}
