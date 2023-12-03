namespace Data.Entities.Abstract
{
    public interface IBaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}