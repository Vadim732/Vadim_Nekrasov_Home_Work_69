using ToDoList.Models;

namespace ToDoList.ViewModels;

public class AssignmentIndexViewModel
{
    public List<Assignment> Assignments { get; set; }
    public PageViewModel PageViewModel { get; set; }
}