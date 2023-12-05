using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Todo_app.Models;

namespace Todo_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoContext context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ToDoContext ctx, ILogger<HomeController> logger)
        {
            context = ctx;
            _logger = logger;
        }

        public IActionResult Index(string id)
        {
            _logger.LogInformation("Index action called");

            var filters = new Filters(id);
            ViewBag.Filters = filters;
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status);

            if (filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }
            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }
            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            _logger.LogInformation("Get request to Add action");

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var task = new ToDo { StatusId = "open" };

            return View(task);
        }

        [HttpPost]
        public IActionResult Add(ToDo task)
        {
            _logger.LogInformation($"Post request to Add action with task ID: {task.Id}");

            if (ModelState.IsValid)
            {
                try
                {
                    context.ToDos.Add(task);
                    context.SaveChanges();
                    _logger.LogInformation($"Task added successfully with ID: {task.Id}");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding task");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state in Add action");
            }

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            return View(task);
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            _logger.LogInformation("Filter action called");

            string id = string.Join("-", filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
        {
            _logger.LogInformation($"MarkComplete action called for task ID: {selected.Id}");

            selected = context.ToDos.Find(selected.Id);
            if (selected != null)
            {
                selected.StatusId = "closed";
                context.SaveChanges();
                _logger.LogInformation($"Task marked complete with ID: {selected.Id}");
            }
            return RedirectToAction("Index", new { Id = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            _logger.LogInformation("DeleteComplete action called");

            var toDelete = context.ToDos.Where(t => t.StatusId == "closed").ToList();
            foreach (var task in toDelete)
            {
                context.ToDos.Remove(task);
                _logger.LogInformation($"Deleting task with ID: {task.Id}");
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}
