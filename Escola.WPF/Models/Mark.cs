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


        // Composição real
        public List<Student> Students { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();

        // Propriedades auxiliares para visualização
        public string StudentNames => Students != null && Students.Any()
            ? string.Join(", ", Students.Select(s => s.FullName))
            : "-";

        public string SubjectNames => Subjects != null && Subjects.Any()
            ? string.Join(", ", Subjects.Select(t => t.Name))
            : "-";


    }
}
