using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostTagController : Controller
    {

        private readonly PostRepository _postRepository;
        private readonly TagRepository _tagRepository;
        private readonly PostTagRepository _postTagRepository;

        public PostTagController(IConfiguration config)
        {
            _postRepository = new PostRepository(config);
            _tagRepository = new TagRepository(config);
            _postTagRepository = new PostTagRepository(config);
        }

        public IActionResult ManageAddTags(int id)
        {
            int userProfileId = GetCurrentUserProfileId();
            var post = _postRepository.GetUserPostById(id, userProfileId);
            var tagOptions = _postTagRepository.GetTagsWORelationshipByPostId(post.Id);

            var vm = new PostTagViewModel()
            {
                Post = post,
                TagOptions = tagOptions
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAddTags(int postId, int tagId)
        {
            int userProfileId = GetCurrentUserProfileId();
            var post = _postRepository.GetUserPostById(postId, userProfileId);
            var tagOptions = _postTagRepository.GetTagsWORelationshipByPostId(post.Id);

            var vm = new PostTagViewModel()
            {
                Post = post,
                TagOptions = tagOptions
            };

            try
            {
                _postTagRepository.AddTagRelationship(postId, tagId);

                return RedirectToAction("Details", "Post", new { @id = postId });
            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }

        public IActionResult ManageRemoveTags(int id)
        {
            int userProfileId = GetCurrentUserProfileId();
            var post = _postRepository.GetUserPostById(id, userProfileId);
            var tagOptions = _postTagRepository.GetAllRelationshipTagsByPostId(post.Id);

            var vm = new PostTagViewModel()
            {
                Post = post,
                TagOptions = tagOptions
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRemoveTags(int postId, int tagId)
        {
            int userProfileId = GetCurrentUserProfileId();
            var post = _postRepository.GetUserPostById(postId, userProfileId);
            var tagOptions = _postTagRepository.GetAllRelationshipTagsByPostId(post.Id);

            var vm = new PostTagViewModel()
            {
                Post = post,
                TagOptions = tagOptions
            };

            try
            {
                _postTagRepository.DeleteTagRelationship(postId, tagId);

                return RedirectToAction("Details", "Post", new { @id = postId });
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
