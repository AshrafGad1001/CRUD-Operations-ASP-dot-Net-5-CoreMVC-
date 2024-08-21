using CRUD_Operation.Models;
using CRUD_Operation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Operation.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<string> _Allowed = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedPosterSize = 1048576;
        private readonly IToastNotification _ToastNotification;
        public MoviesController(ApplicationDbContext context, IToastNotification ToastNotification)
        {
            this._context = context;
            this._ToastNotification = ToastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var Movies = await _context.Movies.OrderByDescending(v => v.Rate).ToListAsync();

            return View(Movies);
        }
        public async Task<IActionResult> Create()
        {
            var ViewModel = new MovieFormViewModle
            {
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync()
            };


            return View("MovieForm", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModle model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var Files = Request.Form.Files;

            if (!Files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Select Movie Poster !!");
                return View("MovieForm", model);
            }
            var poster = Files.FirstOrDefault();


            if (!_Allowed.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only (jpg) and (png) are _Allowed !!");
                return View("MovieForm", model);
            }

            if (poster.Length > _MaxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Can not Upload Img ,Beacuse Large Than 1 mega Byte");
                return View("MovieForm", model);
            }

            using var datastreem = new MemoryStream();
            await poster.CopyToAsync(datastreem);

            var Movies = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                StoryLine = model.StoryLine,
                Poster = datastreem.ToArray()
            };
            _context.Movies.Add(Movies);
            _context.SaveChanges();

            _ToastNotification.AddSuccessToastMessage("Movie Created Successfully.");

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(Id);

            if (movie == null)
                return NotFound();

            var viewModel = new MovieFormViewModle
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Year = movie.Year,
                StoryLine = movie.StoryLine,
                Poster = movie.Poster,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("MovieForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModle model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                return View("MovieForm", model);
            }

            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
            {
                return NotFound();
            }


            var files = Request.Form.Files;

            if (files.Any())
            {
                var Poster = files.FirstOrDefault();

                using var datastream = new MemoryStream();

                await Poster.CopyToAsync(datastream);

                model.Poster = datastream.ToArray();
                if (!_Allowed.Contains(Path.GetExtension(Poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only (jpg) and (png) are _Allowed !!");
                    return View("MovieForm", model);
                }

                if (Poster.Length > _MaxAllowedPosterSize)
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Can not Upload Img ,Beacuse Large Than 1 mega Byte");
                    return View("MovieForm", model);
                }


                movie.Poster = model.Poster;
            }


            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.StoryLine = model.StoryLine;


            _context.SaveChanges();
            _ToastNotification.AddSuccessToastMessage("Movie Updated Successfully.");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? Id)
        {
            var movie = await _context.Movies.Include(G => G.Genre).SingleOrDefaultAsync(G => G.Id == Id);

            if (Id == null)
                return BadRequest();

            if (movie == null)
                return NotFound();

            return View(movie);
        }
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(Id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }

    }

}




