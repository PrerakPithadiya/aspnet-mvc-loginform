using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using loginform_with_database.Data;
using loginform_with_database.Models;

namespace loginform_with_database.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            TempData["Error"] = "Invalid username or password";
            return View();
        }

        TempData["Success"] = "You have been successfully logged in!";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(user);
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your account has been successfully created and your data has been stored in the database!";
            return RedirectToAction(nameof(Login));
        }
        return View(user);
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Username not found");
                return View(model);
            }

            // Update password
            user.Password = model.NewPassword;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your password has been changed successfully!";
            return RedirectToAction(nameof(Login));
        }
        return View(model);
    }
}
