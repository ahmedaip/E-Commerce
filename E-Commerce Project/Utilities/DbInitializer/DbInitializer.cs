using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Project.Utilities.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole(CD.ROLE_SUPER_ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(CD.ROLE_ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(CD.ROLE_EMPLOYEE));
                    await _roleManager.CreateAsync(new IdentityRole(CD.ROLE_CUSTOMER));


                    var user = new ApplicationUser()
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = "SuperAdmin",
                        Email = "superadmin@example.com",
                        EmailConfirmed = true,
                    };

                    await _userManager.CreateAsync(user, "Password123!");

                    await _userManager.AddToRoleAsync(user, CD.ROLE_SUPER_ADMIN);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}