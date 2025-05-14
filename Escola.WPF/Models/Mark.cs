namespace Escola.WPF.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public string AssessmentType { get; set; }  
        public float Grade { get; set; }              
        public DateTime AssessmentDate { get; set; }  
        public int TeacherId { get; set; }


      
        public List<Student> Students { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();

        // Additional properties for wpf
        public string StudentNames => Students != null && Students.Any()
            ? string.Join(", ", Students.Select(s => s.FullName))
            : "-";

        public string SubjectNames => Subjects != null && Subjects.Any()
            ? string.Join(", ", Subjects.Select(t => t.Name))
            : "-";


    }
}
