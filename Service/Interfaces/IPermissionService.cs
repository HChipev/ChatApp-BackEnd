using Service.Abstract;

namespace Service.Interfaces
{
    public interface IPermissionService : IService
    {
        public HashSet<string> GetPermissionsByUserId(int userId);
    }
}