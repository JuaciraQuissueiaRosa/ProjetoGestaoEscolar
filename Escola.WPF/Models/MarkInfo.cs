using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class MarkInfo
    {
        public string SubjectName { get; set; }
        public string AssessmentType { get; set; }
        public float Grade { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string TeacherName { get; set; }
    }
}
