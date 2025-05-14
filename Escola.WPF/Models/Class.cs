namespace Escola.WPF.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }             
        public string AcademicYear { get; set; }      
        public string Course { get; set; }
        public string Shift { get; set; }


        // Additional properties for wpf

        public List<Student> Students { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();

        public string StudentNames => Students != null && Students.Any()
            ? string.Join(", ", Students.Select(s => s.FullName))
            : "-";

        public string TeacherNames => Teachers != null && Teachers.Any()
            ? string.Join(", ", Teachers.Select(t => t.FullName))
            : "-";

        public string SubjectNames => Subjects != null && Subjects.Any()
            ? string.Join(", ", Subjects.Select(s => s.Name))
            : "-";
    }
}
