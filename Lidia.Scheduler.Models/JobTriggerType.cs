using Lidia.Framework.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    [Table("JobTriggerType")]
    public class JobTriggerType : Entity
    {
        [Key]
        public int Id { get; set; }

        public int JobTriggerId { get; set; }

        public string TriggerType { get; set; }

        public string ContinueWith { get; set; }

        public string Source { get; set; }

        public string Key { get; set; }

        public string To { get; set; }

        public string Details { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("JobTriggerId")]
        public virtual JobTrigger JobTrigger { get; set; }

        #endregion        
    }
}