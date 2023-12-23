using Data.Entities.Abstract;

namespace Data.Entities
{
    public class RolePermission : IBaseEntity
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}