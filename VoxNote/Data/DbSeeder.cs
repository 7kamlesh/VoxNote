using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using VoxNote.Models;

namespace VoxNote.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Role == Roles.Admin))
            return;

        var admin = new User
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            Role = Roles.Admin,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}
