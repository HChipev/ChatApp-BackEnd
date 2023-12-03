using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.ModelBuilding
{
    public static class SeedConfigurator
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            UserSeed(modelBuilder);
            RoleSeed(modelBuilder);
            UserRoleSeed(modelBuilder);
        }

        private static void UserSeed(ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    UserName = "icko15.8@gmail.com",
                    NormalizedUserName = "ICKO15.8@GMAIL.COM",
                    Email = "icko15.8@gmail.com",
                    NormalizedEmail = "ICKO15.8@GMAIL.COM",
                    Name = "Hristo Chipev",
                    Picture =
                        "https://lh3.googleusercontent.com/a/AEdFTp6Loqk8Bp9AUCmqWty1RpK0OThyeMc1MtBPF02FoQ=s96-c",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAENiNPb3FcFuhzPcO8DoEvBPAgJpINHNVLQX/UlIhdcpqdZWICDasSvDpEVMu1g/W4g==",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = "f5bd309e-5ebc-40dd-b0fc-655cfea70a70",
                    ConcurrencyStamp = "4ea1a2fc-a47c-44fe-b404-a70225b2b390",
                    CreatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<User>().HasData(users);
        }

        private static void RoleSeed(ModelBuilder modelBuilder)
        {
            var roles = new List<Role>
            {
                new()
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    CreatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<Role>().HasData(roles);
        }

        private static void UserRoleSeed(ModelBuilder modelBuilder)
        {
            var roles = new List<UserRole>
            {
                new()
                {
                    UserId = 1,
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    UserId = 1,
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<UserRole>().HasData(roles);
        }
    }
}