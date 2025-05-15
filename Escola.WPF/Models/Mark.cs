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



        public string StudentName { get; set; }
        public string SubjectName { get; set; }


    }
}
