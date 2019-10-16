using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class JobProperty : Property
    {
        public int JobId { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        #endregion
    }
}