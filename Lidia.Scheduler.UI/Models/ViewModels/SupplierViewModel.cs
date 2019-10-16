using Lidia.Framework.Models;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public EntityStatus Status { get; set; }
    }
}
