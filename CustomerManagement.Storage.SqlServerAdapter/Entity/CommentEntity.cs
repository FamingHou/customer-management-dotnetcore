using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
