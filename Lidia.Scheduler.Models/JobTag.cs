using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class JobTag : Tag
    {
        public int JobId { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        #endregion
    }
}