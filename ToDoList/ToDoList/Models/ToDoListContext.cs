using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models;

public class ToDoListContext : DbContext
{
    public DbSet<Assignment> Assignments { get; set; }
    
    public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options) {}
}