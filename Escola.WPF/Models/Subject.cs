namespace Escola.WPF.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }            
        public int WeeklyHours { get; set; }       

        public List<Teacher> TeacherNamesList { get; set; } = new();
      
        //join to show teachers names of subjects in wpf client side

        public string TeacherNames => TeacherNamesList != null && TeacherNamesList.Any()
           ? string.Join(", ", TeacherNamesList.Select(s => s.FullName))
           : "-";
    }
}
