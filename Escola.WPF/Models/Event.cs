namespace Escola.WPF.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }      
        public string Location { get; set; }         
        public string Description { get; set; }

       
        public List<Student> Students { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();

        // Additional properties for wpf
        public string StudentNames => Students != null && Students.Any()
            ? string.Join(", ", Students.Select(s => s.FullName))
            : "-";

        public string TeacherNames => Teachers != null && Teachers.Any()
            ? string.Join(", ", Teachers.Select(t => t.FullName))
            : "-";


    }
}
