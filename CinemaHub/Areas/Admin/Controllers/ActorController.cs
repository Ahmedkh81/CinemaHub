using CinemaHub.Data;
using CinemaHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(int page = 1)
        {
            IQueryable<Actor> actors = _context.Actors;

            var totalActorsInPage = 6;
            var totalPages = Math.Ceiling((double)actors.Count() / totalActorsInPage);

            if (totalPages < page)
                return View("NotFound");
            actors = actors
                .Skip((page - 1) * (int)totalActorsInPage)
                .Take((int)totalActorsInPage);
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(actors.ToList());
        }

        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile ProfilePictureUrl)
        {
            ModelState.Remove("ProfilePictureUrl");
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            if (ProfilePictureUrl is not null && ProfilePictureUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePictureUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);

                using(var stream = System.IO.File.Create(filePath))
                {
                    await ProfilePictureUrl.CopyToAsync(stream);
                }

                actor.ProfilePictureUrl = fileName;
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();
            TempData["success-notification"] = "Actor Added Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile ProfilePictureUrl)
        {
            ModelState.Remove("ProfilePictureUrl");
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            var actorImg = _context.Actors.AsNoTracking().FirstOrDefault(e => e.Id == actor.Id);
            if (actorImg is not null)
            {
                if(ProfilePictureUrl is not null && ProfilePictureUrl.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePictureUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);

                    using(var stream = System.IO.File.Create(filePath))
                    {
                        await ProfilePictureUrl.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(actor.ProfilePictureUrl))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", actor.ProfilePictureUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    actor.ProfilePictureUrl = fileName;
                }
                else
                {
                    actor.ProfilePictureUrl = actorImg.ProfilePictureUrl;
                }
                _context.Actors.Update(actor);
                _context.SaveChanges();

                TempData["success-notification"] = "Actor Updated Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
                return NotFound();

            if (!string.IsNullOrEmpty(actor.ProfilePictureUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", actor.ProfilePictureUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _context.Actors.Remove(actor);
            _context.SaveChanges();

            TempData["success-notification"] = "Actor Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
