namespace Escola.WPF.Models
{
    public class GradeSheet
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comments { get; set; }           // previously Observacoes
    }
}
