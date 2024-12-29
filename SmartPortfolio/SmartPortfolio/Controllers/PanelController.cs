using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartPortfolio.Data;
using SmartPortfolio.Models;
using SmartPortfolio.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SmartPortfolio.Controllers
{
    public class PanelController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IRole> _roleManager;
        private readonly UserManager<IUser> _userManager;
        public PanelController(RoleManager<IRole> roleManager, UserManager<IUser> userManager, ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            //Tuple<string> usernames;
            //List<List<Asset>> assets = [];
            //List<Asset> modelList = await _db.Assets.ToListAsync();
            //foreach(var item in modelList)
            //{
            //    var id = from p in _db.Portfolios
            //                         where p.PortfolioId == item.PortfolioId
            //                         select p.UserId;
            //    usernames.Add(); = await _db.Users.FindAsync(id);
            //}

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RoleAddUser(RoleUserViewModel? model)
        {
            if (model?.RoleId != null && model?.UserId != null)
            {
                IUser? user = await _userManager.FindByIdAsync(model.UserId.ToString());
                IRole? role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                if (role != null && user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name!);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleListUsersView", "Panel", new { id = role.Id });
                    }
                    else
                    {
                        return BadRequest("Error While Adding User To Role!");
                    }
                }
            }
            return NotFound("User Already Exist In Role");
        }

        public async Task<IActionResult> RoleAddUserView(int? id)
        {
            if (id != null)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString()!);
                if (role != null)
                {
                    ViewBag.Role = "Add users to " + role.Name;
                    ViewData["RoleId"] = role.Id;
                    var users = _userManager.Users.ToList();
                    List<RoleUserViewModel>? modelList = users.Select(user => new RoleUserViewModel
                    {
                        RoleId = role.Id,
                        UserId = user.Id,
                        UserName = user.UserName,
                    }).ToList();
                    return View(modelList);
                }
            }
            return BadRequest("User Not Added To Role!");
        }


        [HttpPost]
        public async Task<IActionResult> RoleDeleteUser(RoleUserViewModel? model)
        {
            if (model?.UserId != null && model?.RoleId != null)
            {
                IUser? user = await _userManager.FindByIdAsync(model.UserId.ToString()!);
                IRole? role = await _roleManager.FindByIdAsync(model.RoleId.ToString()!);
                if (role != null && user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleListUsersView", "Panel", new { id = role.Id });
                    }
                    else
                    {
                        return BadRequest("Error While Delete User From Role!");
                    }
                }
            }
            return NotFound("User Not Found In This Role!");
        }

        public async Task<IActionResult> RoleListUsersView(int? id)
        {
            if (id != null)
            {
                IRole? role = await _roleManager.FindByIdAsync(id.ToString()!);
                if (role != null)
                {
                    ViewBag.Role = role.Name + "'s users";
                    ViewData["RoleId"] = role.Id;
                    IList<IUser>? usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                    List<RoleUserViewModel>? modelList = usersInRole.Select(user => new RoleUserViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        UserId = user.Id,
                        UserName = user.UserName,
                    }).ToList();
                    return View(modelList);
                }
            }
            return BadRequest("Role Users Not Found!");
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                IRole? otherRole = await _roleManager.FindByNameAsync(model.RoleName!);
                if (otherRole != null)
                {
                    return BadRequest("Role Name Must Be Unique!");
                }

                IRole role = new IRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleIndex", "Panel");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return BadRequest("Role Not Added!");
        }

        public IActionResult AddRoleView()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                IRole? findedRole = await _roleManager.FindByIdAsync(model.Id.ToString());
                IRole? nameFindedRole = await _roleManager.FindByNameAsync(model.RoleName!);
                if (findedRole != null && nameFindedRole == null)
                {
                    findedRole.Name = model.RoleName;
                    var result = await _roleManager.UpdateAsync(findedRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleIndex");
                    }
                    return BadRequest("Role Name Must Be Unique!");
                }
            }
            return BadRequest("Role Not Found!");
        }

        public async Task<IActionResult> EditRoleView(int? id)
        {
            if (id != null)
            {
                IRole? findedRole = await _roleManager.FindByIdAsync(id.ToString()!);
                if (findedRole != null)
                {
                    RoleDetailsViewModel rolemodel = new RoleDetailsViewModel
                    {
                        Id = findedRole.Id,
                        RoleName = findedRole.Name!
                    };
                    return View(rolemodel);
                }
            }
            return NotFound("Role not found!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(int? id)
        {
            if (id != null)
            {
                IRole? findedRole = await _roleManager.FindByIdAsync(id.ToString()!);
                if (findedRole != null)
                {
                    var result = await _roleManager.DeleteAsync(findedRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RoleIndex");
                    }
                }
            }
            return NotFound("Role Not Found!");
        }

        public IActionResult RoleIndex()
        {
            var roles = _roleManager.Roles.ToList();
            List<RoleDetailsViewModel> modelList = new List<RoleDetailsViewModel>();
            foreach (var i in roles)
            {
                RoleDetailsViewModel item = new RoleDetailsViewModel
                {
                    Id = i.Id,
                    RoleName = i.Name!,
                };
                modelList.Add(item);
            }
            return View(modelList);
        }

        public IActionResult UserIndex()
        {
            var roles = _db.Users.ToList();
            List<UserDetailsViewModel> modelList = new List<UserDetailsViewModel>();
            foreach (var i in roles)
            {
                UserDetailsViewModel item = new UserDetailsViewModel
                {
                    Id = i.Id,
                    Username = i.UserName!,
                    Email = i.Email,
                    PhoneNumber = i.PhoneNumber,
                };
                modelList.Add(item);
            }
            return View(modelList);
        }

        public async Task<IActionResult> UserListRoleView(int? id)
        {
            if (id != null)
            {
                IUser? findedUser = await _userManager.FindByIdAsync(id.ToString()!);
                if (findedUser == null)
                {
                    return NotFound("User Not Found!");
                }

                ViewBag.User = findedUser.UserName!.ToString() + "'s roles";
                ViewData["UserId"] = findedUser.Id;
                IList<string> userRoleNameList = await _userManager.GetRolesAsync(findedUser);
                List<RoleUserViewModel> modelList = new List<RoleUserViewModel>();
                foreach (string roleName in userRoleNameList)
                {
                    IRole? findedRole = await _roleManager.FindByNameAsync(roleName);
                    if (findedRole == null)
                    {
                        return BadRequest("Error While Getting Roles!");
                    }
                    RoleUserViewModel model = new RoleUserViewModel
                    {
                        RoleId = findedRole.Id,
                        RoleName = findedRole.Name,
                        UserId = findedUser.Id
                    };
                    modelList.Add(model);
                }
                return View(modelList);
            }
            return BadRequest("User Roles Not Found!");
        }

        [HttpPost]
        public async Task<IActionResult> UserDeleteRole(RoleUserViewModel? model)
        {
            if (model?.UserId != null && model?.RoleId != null)
            {
                IUser? user = await _userManager.FindByIdAsync(model.UserId.ToString());
                IRole? role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                if (role != null && user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserListRoleView", "Panel", new { id = role.Id });
                    }
                    else
                    {
                        return BadRequest("Error While Deleting Role From User!");
                    }
                }
            }
            return NotFound("Role Not Found In This User!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id != null)
            {
                IUser? findedUser = await _userManager.FindByIdAsync(id.ToString()!);
                if (findedUser != null)
                {

                    var result = await _userManager.DeleteAsync(findedUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserIndex");
                    }
                    else
                    {
                        return BadRequest("Error While Deleting User!");
                    }
                }
            }
            return NotFound("User Not Found!");
        }

    }
}
