using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class EventParticipation
    {

        public int Id { get; set; }
        public int EventId { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
    }
}
