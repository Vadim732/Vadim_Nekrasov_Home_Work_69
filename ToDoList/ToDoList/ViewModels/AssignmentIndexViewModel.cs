using ToDoList.Models;

namespace ToDoList.ViewModels;

public class AssignmentIndexViewModel
{
    public List<Assignment> Assignments { get; set; }
    public PageViewModel PageViewModel { get; set; }
    public string FilterName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; } 
    public string FilterDescription { get; set; }
    
    public bool Priority1 { get; set; }
    public bool Priority2 { get; set; }
    public bool Priority3 { get; set; }

    public bool Status1 { get; set; }
    public bool Status2 { get; set; }
    public bool Status3 { get; set; }

}