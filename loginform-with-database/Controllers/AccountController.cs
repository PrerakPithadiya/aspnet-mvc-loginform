using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using loginform_with_database.Data;
using loginform_with_database.Models;

namespace loginform_with_database.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Use AnyAsync to avoid materializing the full User entity (prevents errors if DB columns are missing)
        var credentialsValid = await _context.Users
            .AnyAsync(u => u.Username == username && u.Password == password);

        if (!credentialsValid)
        {
            TempData["Error"] = "Invalid username or password";
            return View();
        }

        // Create claims and sign in using cookie authentication
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, username)
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "login");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(principal);

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
            // Use AnyAsync to only check existence without materializing full entity
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == user.Username);

            if (usernameExists)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(user);
            }

            await _context.Users.AddAsync(user);

            // Also store registration record in RegisteredUsers table
            var reg = new RegisteredUser
            {
                Username = user.Username,
                Password = user.Password,
                RegisteredAt = DateTime.UtcNow
            };
            await _context.RegisteredUsers.AddAsync(reg);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log the original error
                _logger?.LogError(dbEx, "Error saving new user. Attempting to apply migrations and retry.");

                // Try to apply pending migrations and retry once
                try
                {
                    await _context.Database.MigrateAsync();
                    await _context.SaveChangesAsync();
                }
                catch (Exception retryEx)
                {
                    // Final failure: log and surface friendly message to the user
                    _logger?.LogError(retryEx, "Migration or retry SaveChanges failed while registering user.");
                    ModelState.AddModelError("", "Unable to register at this time due to a database schema or server issue. Please ensure the database server is running and try again later.");
                    return View(user);
                }
            }

            TempData["Success"] = "Your account has been successfully created and your data has been stored in the database!";
            return RedirectToAction(nameof(Login));
        }
        return View(user);
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    // Profile - view and update basic profile information
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Profile(string? username)
    {
        // Try to determine the username: explicit param -> authenticated user name
        var userNameToLookup = username ?? User?.Identity?.Name;
        if (string.IsNullOrEmpty(userNameToLookup))
        {
            // No username known; require login
            TempData["Error"] = "Please login to access your profile.";
            return RedirectToAction(nameof(Login));
        }

        RegisteredUser? reg = null;
        try
        {
            reg = await _context.RegisteredUsers.FirstOrDefaultAsync(r => r.Username == userNameToLookup);
        }
        catch (Exception ex)
        {
            // Likely a schema mismatch (missing columns like AvatarUrl/DisplayName). Provide a friendly message.
            _logger?.LogError(ex, "Database error while loading profile for {Username}", userNameToLookup);
            TempData["Error"] = "Unable to load profile due to a database schema mismatch (missing columns). Please apply the latest database migrations or run the provided SQL alterations.";
            return RedirectToAction(nameof(Login));
        }

        if (reg == null)
        {
            TempData["Error"] = "Profile not found.";
            return RedirectToAction(nameof(Login));
        }

        var model = new ProfileModel
        {
            Id = reg.Id,
            Username = reg.Username,
            DisplayName = reg.DisplayName,
            AvatarUrl = reg.AvatarUrl,
            RegisteredAt = reg.RegisteredAt
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Profile(ProfileModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var reg = await _context.RegisteredUsers.FindAsync(model.Id);
        if (reg == null)
        {
            ModelState.AddModelError("", "Profile not found.");
            return View(model);
        }

        reg.DisplayName = model.DisplayName;
        reg.AvatarUrl = model.AvatarUrl;

        try
        {
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your profile has been updated.";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating profile for user {Username}", reg.Username);
            ModelState.AddModelError("", "Unable to update profile. Please try again later.");
            return View(model);
        }

        return RedirectToAction(nameof(Profile), new { username = reg.Username });
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            // Perform an UPDATE without materializing the full User entity to avoid errors
            // when the database schema is out-of-sync (e.g. missing columns).
            int rows = 0;
            try
            {
                rows = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE `Users` SET `Password` = {model.NewPassword} WHERE `Username` = {model.Username}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating password for username {Username}", model.Username);
                ModelState.AddModelError("", "Unable to reset password due to a database error. Please ensure the database server is running and try again later.");
                return View(model);
            }

            if (rows == 0)
            {
                ModelState.AddModelError("", "Username not found");
                return View(model);
            }

            TempData["Success"] = "Your password has been changed successfully!";
            return RedirectToAction(nameof(Login));
        }
        return View(model);
    }
}
