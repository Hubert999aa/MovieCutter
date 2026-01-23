namespace Domain.BusinessModels.DatabaseModels
{
    public class Profile
    {
        public int IdProfile { get; set; }
        public string Name { get; set; } = string.Empty;


        public IEnumerable<Source> Sources { get; set; }
    }
}
