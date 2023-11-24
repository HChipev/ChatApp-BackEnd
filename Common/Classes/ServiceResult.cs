namespace Common.Classes
{
    public class ServiceResult<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}