using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Data.EntityFramework
{
    public class MySqlConnectionStringOptions
    {
        [Required]
        public string ConnectionString { get; set; } = null!;
    }
}
