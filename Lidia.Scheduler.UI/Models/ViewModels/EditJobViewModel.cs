using CronExpressionDescriptor;
using Lidia.Framework.Models;
using Lidia.Framework.SaaS.Models;
using Lidia.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lidia.Scheduler.UI.Models.ViewModels
{
    public class EditJobViewModel : BaseViewModel
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

        public EntityStatus Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

    }
}