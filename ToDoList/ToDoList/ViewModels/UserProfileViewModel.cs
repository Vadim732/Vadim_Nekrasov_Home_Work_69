using ToDoList.Models;

namespace ToDoList.ViewModels;

public class UserProfileViewModel
{
    public User User { get; set; }
    public List<Assignment> CreatedAssignments { get; set; }
}