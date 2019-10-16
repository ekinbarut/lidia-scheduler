using Lidia.Framework.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    [Table("JobTrigger")]
    public class JobTrigger : Entity
    {
        [Key]
        public int TriggerId { get; set; }

        public int JobId { get; set; }

        public string JobTriggerType { get; set; }

        public string Name { get; set; }

        public string Condition { get; set; }

        public string Event { get; set; }

        public string Details { get; set; }

        public DateTime Created { get; set; }

        public EntityStatus Status { get; set; }

        #region [ Navigation properties ]


        [ForeignKey("JobTriggerType")]
        public virtual JobTriggerType TriggerType { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        #endregion        
    }
}