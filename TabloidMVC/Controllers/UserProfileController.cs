using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class UserProfileController : Controller
    {

        private readonly UserProfileRepository _userProfileRepository;

        public UserProfileController(IConfiguration config)
        {
            _userProfileRepository = new UserProfileRepository(config);
        }

        // GET: UserProfile
        public ActionResult Index()
        {
            if(Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfiles = _userProfileRepository.GetAllActiveUserProfiles();
                userProfiles.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName));

                return View(userProfiles);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ViewDeactivated()
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfiles = _userProfileRepository.GetAllDeactivatedUserProfiles();
                userProfiles.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName));

                return View(userProfiles);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: UserProfile/Details/5
        public ActionResult Details(int id)
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfile = _userProfileRepository.GetUserById(id);
                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: UserProfile/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: UserProfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: UserProfile/Edit/5
        public ActionResult Edit(int id)
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfile = _userProfileRepository.GetUserById(id);
                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: UserProfile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile user)
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfile = _userProfileRepository.GetUserById(id);

                try
                {
                    _userProfileRepository.UpdateUserType(userProfile);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View(userProfile);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: UserProfile/ActiveStatus/5
        public ActionResult ActiveStatus(int id)
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfile = _userProfileRepository.GetUserById(id);
                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: UserProfile/ActiveStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActiveStatus(int id, UserProfile user)
        {
            if (Int32.Parse(User.FindFirstValue(ClaimTypes.Role)) == 1)
            {
                var userProfile = _userProfileRepository.GetUserById(id);

                try
                {
                    _userProfileRepository.UpdateStatus(userProfile);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View(userProfile);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
