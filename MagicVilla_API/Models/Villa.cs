namespace MagicVilla_API.Model
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
