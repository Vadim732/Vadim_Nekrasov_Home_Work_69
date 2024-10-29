namespace ToDoList.Models;

public class Assignment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public string ArtistName { get; set; }
    public string Description { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateOpening { get; set; }
    public DateTime DateClosing { get; set; }
}