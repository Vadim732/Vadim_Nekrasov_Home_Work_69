using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Controllers;

public class AssignmentController : Controller
{
    private readonly ToDoListContext _context;

    public AssignmentController(ToDoListContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        List<Assignment> assignments = await _context.Assignments.ToListAsync();
        
        return View(assignments);
    }
    
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Assignment assignment)
    {
        if (ModelState.IsValid)
        {
            assignment.DateCreation = DateTime.UtcNow;
            assignment.Status = 1;
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(assignment);
    }
}