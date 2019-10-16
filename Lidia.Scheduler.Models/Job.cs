using CronExpressionDescriptor;
using Lidia.Framework.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class Job : Entity
    {
        [Key]
        public int JobId { get; set; }        

        public int CollectionId { get; set; }

        public JobType Type { get; set; }

        public string Name { get; set; }

        public string AppKey { get; set; }

        public string Process { get; set; }

        public string Description { get; set; }

        public string CronExpression { get; set; }

        public string JobUrl { get; set; }

        [NotMapped]
        public string CronExpressionDescription
        {
            get
            {
                if (string.IsNullOrEmpty(CronExpression))
                {
                    return string.Empty;
                }
                return ExpressionDescriptor.GetDescription(CronExpression);
            }
        }

        public bool ProcessResult { get; set; }

        public bool SendSummary { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("CollectionId")]
        public virtual JobCollection Collection { get; set; }

        public virtual List<JobLogEntry> Log { get; set; }

        public virtual List<JobProperty> Properties { get; set; } = new List<JobProperty>();

        public virtual List<JobTag> Tags { get; set; } = new List<JobTag>();
        
        

        #endregion
    }
}