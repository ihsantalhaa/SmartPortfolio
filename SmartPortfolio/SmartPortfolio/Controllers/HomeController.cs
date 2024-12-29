using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartPortfolio.Data;
using SmartPortfolio.Models;
using SmartPortfolio.ViewModels;
using System.Diagnostics;

namespace SmartPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IUser> _userManager;
        private readonly SignInManager<IUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IUser> userManager, SignInManager<IUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Index(UserSignInViewModel? model)
        {
            //if (model != null)
            //{
            //    return View(model);
            //}
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Submit(UserSignUpViewModel model)
        {
            if (ModelState.IsValid) {
                IUser? nameFindedUser = await _userManager.FindByNameAsync(model.Username!);
                IUser? emailFindedUser = await _userManager.FindByEmailAsync(model.Email!);
                if (nameFindedUser == null && emailFindedUser == null)
                {
                    IUser? user = new IUser();
                    user.UserName = model.Username!.Replace(" ", "");
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    string password = model.Password!.ToString();
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        var loginResult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, true);
                        if (loginResult.Succeeded)
                        {
                            return RedirectToAction("PortfolioIndex", "Dashboard");
                        }
                    }
                    TempData["Alert"] = "Error While Adding User !!!";
                }
                TempData["Alert"] = "User Available !!!";
            }
            return RedirectToAction("SubmitView", "Home", model);
        }

        [AllowAnonymous]
        public IActionResult SubmitView(UserSignUpViewModel? model)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserSignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                IUser? signedUser = await _userManager.FindByEmailAsync(model.Email);
                if (signedUser == null)
                {
                    TempData["Message"] = "Error while logging in, please try again later.";
                }
                else
                {
                    // First true for cookie and second is ban for wrong inputs  :)
                    var result = await _signInManager.PasswordSignInAsync(signedUser.UserName!, model.Password, true, true);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(signedUser, isPersistent: true);
                        var isAdmin = await _userManager.IsInRoleAsync(signedUser, "Admin");
                        if (isAdmin)
                        {
                            return RedirectToAction("UserIndex", "Panel");
                        }
                        return RedirectToAction("PortfolioIndex", "Dashboard");
                    }
                }
            }
            TempData["Message"] = "Wrong username or password!";
            return RedirectToAction("Index", "Home", model);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        
    }
}
