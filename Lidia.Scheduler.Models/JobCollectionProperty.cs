using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class JobCollectionProperty : Property
    {
        public int CollectionId { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("CollectionId")]
        public virtual JobCollection Collection { get; set; }

        #endregion
    }
}