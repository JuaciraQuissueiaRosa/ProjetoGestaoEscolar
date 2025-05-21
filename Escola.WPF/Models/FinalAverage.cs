using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class FinalAverage
    {
        public int StudentId { get; set; }

        //to show client side
        public string StudentName { get; set; }
        public int SubjectId { get; set; }

        //to show client side
        public string SubjectName { get; set; }
        public float Average { get; set; }
    }
}
