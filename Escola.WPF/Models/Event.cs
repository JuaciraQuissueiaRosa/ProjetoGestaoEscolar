namespace Escola.WPF.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }       // previously Data
        public string Location { get; set; }          // previously Local
        public string Description { get; set; }

        // Composição real
        public List<Student> Students { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();

        // Propriedades auxiliares para visualização
        public string StudentNames => Students != null && Students.Any()
            ? string.Join(", ", Students.Select(s => s.FullName))
            : "-";

        public string TeacherNames => Teachers != null && Teachers.Any()
            ? string.Join(", ", Teachers.Select(t => t.FullName))
            : "-";


    }
}
