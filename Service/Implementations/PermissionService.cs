using Data.Entities;
using Data.Repository;
using Service.Interfaces;

namespace Service.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;

        public PermissionService(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public HashSet<string> GetPermissionsByUserId(int userId)
        {
            var userRoles = _userRepository.Find(userId, x => x.Roles)?
                .Roles.Select(x => x.Id).ToList();

            if (userRoles is null)
            {
                return new HashSet<string>();
            }

            return _roleRepository
                .FindAllByCondition(x => userRoles.Contains(x.Id), x => x.Permissions)
                .Select(r => r.Permissions.Select(p => p.Name))
                .SelectMany(x => x).ToHashSet();
        }
    }
}