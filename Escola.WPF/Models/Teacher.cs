namespace Escola.WPF.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FullName { get; set; }         // previously Nome
        public string Phone { get; set; }            // previously Contacto
        public string Email { get; set; }
        public string TeachingArea { get; set; }     // previously AreaEnsino
    }
}
