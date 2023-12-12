using Microsoft.AspNetCore.Identity;
using Pronia.Helpers;

namespace Pronia.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm login , string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await _userManager.FindByNameAsync(login.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(login.UsernameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError("", "Username Or Email or Password Error");
                    return View();
                }
            }
            var result = _signInManager.CheckPasswordSignInAsync(user,login.Password,true).Result;
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Zehmet Olmasa Biraz Sonra Yeniden Cehd Edin");
                return View();
            }
            if (result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username Or Email or Password Error");
                return View();
            }
            await _signInManager.SignInAsync(user, login.RememberMe);
            if(ReturnUrl != null && !ReturnUrl.Contains("Login"))
            {
                return RedirectToAction(ReturnUrl);
            }
            return RedirectToAction(nameof(Index) , "Home");
        }
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm register)
        {
            if(ModelState.IsValid)
            {
                return View();
            }

            AppUser user = new AppUser()
            {
                Name=register.Name,
                Email=register.Email,
                Surname=register.Surname,
                UserName=register.Username
            };
            var result = await _userManager.CreateAsync(user , register.Password);

            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(Index),"Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index),"Home");
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (var role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = role.ToString(),
                    });
                }
            }

            return RedirectToAction(nameof(Index), "home");
        }
    }
}
