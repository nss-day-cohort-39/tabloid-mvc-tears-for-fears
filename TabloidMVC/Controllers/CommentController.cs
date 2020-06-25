using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class CommentController : Controller
    {
        public readonly CommentRepository _commentRepo;
        public readonly PostRepository _postRepo;
        public CommentController(IConfiguration config)
        {
            _commentRepo = new CommentRepository(config);
            _postRepo = new PostRepository(config);
        }
        // GET: CommentController
        public ActionResult Index(int id)
        {
            var comments = _commentRepo.GetCommentsByPostId(id);
            Post post = _postRepo.GetPublisedPostById(id);
            var vm = new CommentIndexViewModel()
            {
                PostComments = comments,
                Post = post
            };
            return View(vm);
        }

        // GET: CommentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CommentController/Create
        public ActionResult Create(int id)
        {
            CommentIndexViewModel vm = new CommentIndexViewModel();
            vm.PostComments = _commentRepo.GetCommentsByPostId(id);
            return View(vm);
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentIndexViewModel vm, int id)
        {
            try
            {
                vm.Comment.UserProfileId = GetCurrentUserProfileId();
                vm.Comment.PostId = id;
                vm.PostComments = _commentRepo.GetCommentsByPostId(id);
                vm.Comment.CreateDateTime = DateTime.Now;
                _commentRepo.AddComment(vm.Comment);
                return RedirectToAction("Index", new { id = id });
            }
            catch(Exception ex)
            {
                return View(vm);
            }
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            int userProfileId = GetCurrentUserProfileId();
            var comments = _commentRepo.GetCommentsByPostId(id);
            Post post = _postRepo.GetPublisedPostById(id);
            //var comment = _commentRepo.GetUserCommentById(id, userProfileId);

            var vm = new CommentIndexViewModel()
            {
                PostComments = comments,
                Post = post
            };
            return View(vm);
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CommentIndexViewModel vm)
        {
            try
            {
                _commentRepo.DeleteComment(id);
                return RedirectToAction("Index", "Post");
            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }
        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
