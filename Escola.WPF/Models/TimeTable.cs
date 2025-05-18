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


        public string TeacherName { get; set; }

        public string SubjectName { get; set; }

        public string ClassName { get; set; }
    }
}
