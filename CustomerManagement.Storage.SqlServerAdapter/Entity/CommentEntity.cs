using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CustomerManagement.Storage.SqlServerAdapter.Entity
{
    [Table("Comments")]
    public class CommentEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public virtual CustomerEntity Customer { get; set; }
    }
}
