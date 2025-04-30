using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public string AssessmentType { get; set; }    // e.g. Test, Assignment
        public float Grade { get; set; }              // previously Nota
        public DateTime AssessmentDate { get; set; }  // previously DataAvaliacao
        public int TeacherId { get; set; }
    }
}
