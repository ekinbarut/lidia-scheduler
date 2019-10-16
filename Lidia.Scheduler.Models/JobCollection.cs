using Lidia.Framework.Models;
using Lidia.Framework.SaaS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class JobCollection : Entity
    {
        [Key]
        public int CollectionId { get; set; }

        public int? TenantId { get; set; }

        public int ApplicationId { get; set; }

        public string Name { get; set; }

        #region [ Navigation Properties ]

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }

        public virtual List<Job> Jobs { get; set; } = new List<Job>();

        public virtual List<JobCollectionTag> Tags { get; set; } = new List<JobCollectionTag>();

        public virtual List<JobCollectionProperty> Properties { get; set; } = new List<JobCollectionProperty>();

        #endregion
    }
}