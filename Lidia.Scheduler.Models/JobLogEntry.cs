using Lidia.Framework.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    [Table("JobLog")]
    public class JobLogEntry : Entity
    {
        [Key]
        public int EntryId { get; set; }

        public int JobId { get; set; }

        public string Summary { get; set; }

        public string Details { get; set; }

        public string Change { get; set; }

        public JobActivityType ActivityType { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        #endregion        
    }
}