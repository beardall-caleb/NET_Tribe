using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Data;
using TodoList.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Api.Controllers;

public class HomeController : Controller
{
    private readonly TodoContext _context;

    public HomeController(TodoContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _context.Tasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();

        return View(tasks);
    }

    [HttpGet]
    public IActionResult AddTask()
    {
        var task = new TaskItem
        {
            DueDate = DateOnly.FromDateTime(DateTime.Today),
            Priority = 5
        };

        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTask(TaskItem task)
    {
        if (!ModelState.IsValid)
        {
            return View(task);
        }

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Task \"{task.Title}\" was added.";

        return RedirectToAction(nameof(Index));
    }
}
