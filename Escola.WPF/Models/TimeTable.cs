namespace Escola.WPF.Models
{
    public class TimeTable
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // additional properties
        public List<Teacher> Teachers { get; set; } = new();

        public List<Subject> Subjects { get; set; } = new();


        public string StudentNames => Teachers != null && Teachers.Any()
            ? string.Join(", ", Teachers.Select(s => s.FullName))
            : "-";

        public string SubjectNames => Subjects != null && Subjects.Any()
            ? string.Join(", ", Subjects.Select(t => t.Name))
            : "-";
    }
}
