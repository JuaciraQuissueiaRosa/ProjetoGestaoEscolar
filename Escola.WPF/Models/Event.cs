using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class Event
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }       // previously Data
        public string Location { get; set; }          // previously Local
        public string Description { get; set; }
    }
}
