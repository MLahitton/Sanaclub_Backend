using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Auth;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Seeders;

public sealed class DevelopmentAdminSeeder
{
    private readonly SanaclubDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public DevelopmentAdminSeeder(
        SanaclubDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _configuration["DevAdmin:Email"];
        var fullName = _configuration["DevAdmin:FullName"];
        var password = _configuration["DevAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("DevAdmin:Email configuration is required.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException("DevAdmin:FullName configuration is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("DevAdmin:Password configuration is required.");
        }

        if (password.Length < 8)
        {
            throw new InvalidOperationException("DevAdmin:Password must be at least 8 characters.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var trimmedEmail = email.Trim();
        var trimmedFullName = fullName.Trim();

        var adminRole = await _context.Roles.SingleOrDefaultAsync(
            x => x.Code == "ADMIN",
            cancellationToken);

        if (adminRole is null)
        {
            throw new InvalidOperationException("ADMIN role was not found. Run database seed first.");
        }

        var user = await _context.Users.SingleOrDefaultAsync(
            x => x.NormalizedEmail == normalizedEmail,
            cancellationToken);
        var userWasCreated = false;

        if (user is null)
        {
            var passwordHash = _passwordHasher.Hash(password);
            user = new User(trimmedEmail, passwordHash, trimmedFullName);
            _context.Users.Add(user);
            userWasCreated = true;
        }

        if (!userWasCreated && !user.IsActive)
        {
            throw new InvalidOperationException("Development admin user exists but is inactive.");
        }

        if (!await _context.UserRoles.AnyAsync(
            x => x.UserId == user.Id &&
                 x.RoleId == adminRole.Id &&
                 x.IsActive,
            cancellationToken))
        {
            _context.UserRoles.Add(new UserRole(user.Id, adminRole.Id));
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
