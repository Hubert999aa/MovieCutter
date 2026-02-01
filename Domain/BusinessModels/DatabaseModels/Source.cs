using Domain.BusinessEnums;

namespace Domain.BusinessModels.DatabaseModels
{
    public class Source
    {
        public int IdSource { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public SourceType SourceType { get; set; }
        public int IdProfile { get; set; }


        public Profile Profile { get; set; }
    }
}
