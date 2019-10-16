using Lidia.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace Lidia.Scheduler.Models
{
    public class Property : Entity
    {
        [Key]
        public int PropertyId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Extra { get; set; }
    }
}
