using System.Text.Json.Serialization;

namespace Lab06_Bartczak_Rafal
{
    public class DbModels
    {
        public class Note
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        }

        public class Tag
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;

            [JsonIgnore] 
            public ICollection<Note> Notes { get; set; } = new List<Note>();
        }

        public record CreateNoteDto(string Title, string Body);

        public record AssignTagsDto(List<string> Tags);
    }
}
