using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models;

public class ToDoListContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<User> Users { get; set; }
    
    public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}