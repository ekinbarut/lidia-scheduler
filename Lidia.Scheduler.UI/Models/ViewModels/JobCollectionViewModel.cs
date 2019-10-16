using Lidia.Framework.Models;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class EditJobCollectionViewModel : BaseViewModel
    {
        [Key]
        public int CollectionId { get; set; }

        public int? TenantId { get; set; }

        public int ApplicationId { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public EntityStatus Status { get; set; }

    }
}