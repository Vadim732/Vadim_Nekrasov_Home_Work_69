using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

[Authorize]
public class AssignmentController : Controller
{
    private readonly ToDoListContext _context;
    private readonly UserManager<User> _userManager;

    public AssignmentController(ToDoListContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index(string name, DateTime? dateFrom, DateTime? dateTo, string description, int? priority, int? status, bool? isTakenByUser, bool? isFree, SortAssignmentState sortAssignmentState = SortAssignmentState.NameAsc, int page = 1)
    {
        IQueryable<Assignment> assignments = _context.Assignments;
        var user = await _userManager.GetUserAsync(User);
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            assignments = assignments.Where(a => a.Name == name);
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            assignments = assignments.Where(a => a.Description.Contains(description));
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
        if (isTakenByUser.HasValue && isTakenByUser.Value && user != null)
        {
            assignments = assignments.Where(a => a.UserPerformerId == user.Id);
        }
        if (isFree.HasValue && isFree.Value)
        {
            assignments = assignments.Where(a => a.UserPerformerId == null);
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
            Status3 = (status == 3),
            IsTakenByUser = isTakenByUser,
            IsFree = isFree
        };
        
        return View(aivm);
    }
    
    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Create(Assignment assignment)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                assignment.UserCreatorId = user.Id;
                assignment.DateCreation = DateTime.UtcNow;
                assignment.Status = 1;
                
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return NotFound();
        }

        return View(assignment);
    }
    
    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Delete(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        var user = await _userManager.GetUserAsync(User);
        if (assignment != null && assignment.Status != 2 && user != null && assignment.UserCreatorId == user.Id)
        {
            _context.Remove(assignment);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        return Forbid();
    }

    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> TakeOnTask(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        var user = await _userManager.GetUserAsync(User);
        if (assignment != null && user != null && assignment.UserPerformerId == null)
        {
            assignment.UserPerformerId = user.Id;
            
            _context.Update(assignment);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        return Forbid();
    }

    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Open(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        var user = await _userManager.GetUserAsync(User);
        if (assignment != null && assignment.Status == 1 && user != null && assignment.UserPerformerId == user.Id)
        {
            assignment.DateOpening = DateTime.UtcNow;
            assignment.Status = 2;
            
            _context.Update(assignment);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        return Forbid();
    }
    
    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Close(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        var user = await _userManager.GetUserAsync(User);
        if (assignment != null && assignment.Status == 2 && user != null && assignment.UserPerformerId == user.Id)
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

    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Edit(int assignmentId)
    {
        Assignment assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        var user = await _userManager.GetUserAsync(User);
        if (assignment != null && user != null && assignment.UserCreatorId == user.Id)
        {
            return View(assignment);
        }
        
        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "user, admin")]
    public async Task<IActionResult> Edit(Assignment assignment)
    {
        if (ModelState.IsValid)
        {
            var existingAssignment = await _context.Assignments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == assignment.Id);
            var user = await _userManager.GetUserAsync(User);
            if (existingAssignment != null && user != null && assignment.UserCreatorId == user.Id)
            {
                assignment.DateCreation = existingAssignment.DateCreation;
                assignment.DateOpening = existingAssignment.DateOpening;
                assignment.DateClosing = existingAssignment.DateClosing;
                assignment.UserCreatorId = existingAssignment.UserCreatorId;
                assignment.UserPerformerId = existingAssignment.UserPerformerId;
                assignment.Status = existingAssignment.Status;
                
                _context.Assignments.Update(assignment);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        return View(assignment);
    }
}