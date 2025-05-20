namespace Escola.WPF.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }      
        public DateTime? BirthDate { get; set; }   
        public string Phone { get; set; }        
        public string Address { get; set; }      
        public string Email { get; set; }
        public int? ClassId { get; set; }         
    }
}
