using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public  class FinalAverage
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SubjectId { get; set; }
    
        public float Average { get; set; }

        // Propriedade adicional para exibição
        public string SubjectName { get; set; }
    }

}
