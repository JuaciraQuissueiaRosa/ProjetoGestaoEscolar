using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola.WPF.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }      // previously Nome
        public DateTime BirthDate { get; set; }   // previously DataNascimento
        public string Phone { get; set; }         // previously Contacto
        public string Address { get; set; }       // previously Morada
        public string Email { get; set; }
        public int? ClassId { get; set; }         // previously TurmaId
    }
}
