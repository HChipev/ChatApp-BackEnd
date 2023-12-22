using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Permission = Common.Enums.Permission;

namespace Data.ModelBuilding
{
    public static class SeedConfigurator
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            UserSeed(modelBuilder);
            RoleSeed(modelBuilder);
            UserRoleSeed(modelBuilder);
            PermissionSeed(modelBuilder);
            RolePermissionSeed(modelBuilder);
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
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9820)
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
                    Id = (int)Common.Enums.Role.Admin,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9840)
                },
                new()
                {
                    Id = (int)Common.Enums.Role.User,
                    Name = "User",
                    NormalizedName = "USER",
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9840)
                },
                new()
                {
                    Id = (int)Common.Enums.Role.Subscriber,
                    Name = "Subscriber",
                    NormalizedName = "Subscriber",
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9840)
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
                    RoleId = (int)Common.Enums.Role.Admin,
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9860)
                },
                new()
                {
                    UserId = 1,
                    RoleId = (int)Common.Enums.Role.User,
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9860)
                }
            };

            modelBuilder.Entity<UserRole>().HasData(roles);
        }

        private static void PermissionSeed(ModelBuilder modelBuilder)
        {
            var permissions =
                Enum.GetValues<Permission>().Select(x => new Entities.Permission
                {
                    Id = (int)x,
                    Name = x.ToString(),
                    CreatedAt = new DateTime(2023, 12, 3, 13, 47, 34, 839, DateTimeKind.Utc).AddTicks(9840)
                });

            modelBuilder.Entity<Entities.Permission>().HasData(permissions);
        }

        private static void RolePermissionSeed(ModelBuilder modelBuilder)
        {
            var rolePermission = new List<RolePermission>
            {
                new()
                {
                    RoleId = (int)Common.Enums.Role.Admin,
                    PermissionId = (int)Permission.Subscriber
                },
                new()
                {
                    RoleId = (int)Common.Enums.Role.Admin,
                    PermissionId = (int)Permission.NonSubscriber
                },
                new()
                {
                    RoleId = (int)Common.Enums.Role.Admin,
                    PermissionId = (int)Permission.ManageIdentity
                },
                new()
                {
                    RoleId = (int)Common.Enums.Role.Admin,
                    PermissionId = (int)Permission.ManageDocuments
                },
                new()
                {
                    RoleId = (int)Common.Enums.Role.User,
                    PermissionId = (int)Permission.NonSubscriber
                },
                new()
                {
                    RoleId = (int)Common.Enums.Role.Subscriber,
                    PermissionId = (int)Permission.Subscriber
                }
            };

            modelBuilder.Entity<RolePermission>().HasData(rolePermission);
        }
    }
}