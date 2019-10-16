using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Scheduler.Models
{
    public class JobCollectionTag : Tag
    {
        public int CollectionId { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("CollectionId")]
        public virtual JobCollection Collection { get; set; }

        #endregion
    }
}