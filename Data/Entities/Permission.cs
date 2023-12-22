using Data.Entities.Abstract;

namespace Data.Entities
{
    public class Permission : IBaseEntity
    {
        public int Id { get; init; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}