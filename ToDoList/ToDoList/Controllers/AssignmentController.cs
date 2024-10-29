using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

public class AssignmentController : Controller
{
    private readonly ToDoListContext _context;

    public AssignmentController(ToDoListContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index(string name, DateTime? dateFrom, DateTime? dateTo, string description, int? priority, int? status, SortAssignmentState sortAssignmentState = SortAssignmentState.NameAsc, int page = 1)
    {
        IQueryable<Assignment> assignments = _context.Assignments;
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            assignments = assignments.Where(a => a.Name == name);
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            assignments = assignments.Where(a => a.Name.Contains(description));
        }
        if (dateFrom.HasValue)
        {
            dateFrom = DateTime.SpecifyKind(dateFrom.Value, DateTimeKind.Utc);
            assignments = assignments.Where(a => a.DateCreation >= dateFrom);
        }
        if (dateTo.HasValue)
        {
            dateTo = DateTime.SpecifyKind(dateTo.Value, DateTimeKind.Utc);
            assignments = assignments.Where(a => a.DateCreation <= dateTo);
        }
        if (priority.HasValue)
        {
            assignments = assignments.Where(a => a.Priority == priority.Value);
        }
        if (status.HasValue)
        {
            assignments = assignments.Where(a => a.Status == status.Value);
        }
        
        ViewBag.NameSort = sortAssignmentState == SortAssignmentState.NameAsc ? SortAssignmentState.NameDesc : SortAssignmentState.NameAsc;
        ViewBag.PrioritySort = sortAssignmentState == SortAssignmentState.PriorityAsc ? SortAssignmentState.PriorityDesc : SortAssignmentState.PriorityAsc;
        ViewBag.StatusSort = sortAssignmentState == SortAssignmentState.StatusAsc ? SortAssignmentState.StatusDesc : SortAssignmentState.StatusAsc;
        ViewBag.DateCreationSort = sortAssignmentState == SortAssignmentState.DateCreationAsc ? SortAssignmentState.DateCreationDesc : SortAssignmentState.DateCreationAsc;
        switch (sortAssignmentState)
        {
            case SortAssignmentState.NameAsc:
                assignments = assignments.OrderBy(a => a.Name);
                break;
            case SortAssignmentState.NameDesc:
                assignments = assignments.OrderByDescending(a => a.Name);
                break;
            case SortAssignmentState.PriorityAsc:
                assignments = assignments.OrderBy(a => a.Priority);
                break;
            case SortAssignmentState.PriorityDesc:
                assignments = assignments.OrderByDescending(a => a.Priority);
                break;
            case SortAssignmentState.StatusAsc:
                assignments = assignments.OrderBy(a => a.Status);
                break;
            case SortAssignmentState.StatusDesc:
                assignments = assignments.OrderByDescending(a => a.Status);
                break;
            case SortAssignmentState.DateCreationAsc:
                assignments = assignments.OrderBy(a => a.DateCreation);
                break;
            case SortAssignmentState.DateCreationDesc:
                assignments = assignments.OrderByDescending(a => a.DateCreation);
                break;
        }
        
        int pageSize = 3;
        int count = assignments.Count();
        var items = assignments.Skip((page - 1) * pageSize).Take(pageSize);
        
        PageViewModel pvm = new PageViewModel(assignments.Count(), page, pageSize);
        var aivm = new AssignmentIndexViewModel()
        {
            Assignments = items.ToList(),
            PageViewModel = pvm,
            FilterName = name,
            FilterDescription = description,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Priority1 = (priority == 1),
            Priority2 = (priority == 2),
            Priority3 = (priority == 3),
            Status1 = (status == 1),
            Status2 = (status == 2),
            Status3 = (status == 3)
        };
        
        return View(aivm);
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
    
    public async Task<IActionResult> Delete(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment != null)
        {
            _context.Remove(assignment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Open(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment != null && assignment.Status == 1)
        {
            assignment.DateOpening = DateTime.UtcNow;
            assignment.Status = 2;
            _context.Update(assignment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Close(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment != null && assignment.Status == 2)
        {
            assignment.DateClosing = DateTime.UtcNow;
            assignment.Status = 3;
            _context.Update(assignment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Details(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment != null)
        {
            return View(assignment);
        }

        return NotFound();
    }
}