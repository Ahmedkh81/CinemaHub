using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;
using CinemaHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private IActorRepository _actorRepository;

        public ActorController(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var actors = await _actorRepository.GetAsync();

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

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public IActionResult Create()
        {
            return View(new Actor());
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
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

            await _actorRepository.CreateAsync(actor);

            TempData["success-notification"] = "Actor Added Successfully";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id );
            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile ProfilePictureUrl)
        {
            ModelState.Remove("ProfilePictureUrl");
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            var actorImg = await _actorRepository.GetOneAsync(e => e.Id == actor.Id , tracked : false);
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

                await _actorRepository.UpdateAsync(actor);

                TempData["success-notification"] = "Actor Updated Successfully";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);
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

            await _actorRepository.DeleteAsync(actor);

            TempData["success-notification"] = "Actor Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
