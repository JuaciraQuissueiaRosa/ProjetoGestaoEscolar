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

        // Para mostrar os nomes
        public string StudentName { get; set; }
        public string SubjectName { get; set; }

        // ✅ Propriedade calculada para o ano letivo
        public string AcademicYear
        {
            get
            {
                int startYear = AssessmentDate.Year;
                int endYear = startYear + 1;
                return $"{startYear}/{endYear}";
            }
        }
    }
}
