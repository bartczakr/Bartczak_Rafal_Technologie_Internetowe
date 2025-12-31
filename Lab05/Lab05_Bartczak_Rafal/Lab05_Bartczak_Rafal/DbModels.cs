using System.ComponentModel.DataAnnotations;

namespace Lab05_Bartczak_Rafal
{
    public class Column
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Ord { get; set; } 
    }

    public class KanbanTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public int ColId { get; set; }

        public int Ord { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Column? Column { get; set; }
    }
    public record CreateTaskDto(string Title, int ColId);
    public record MoveTaskDto(int ColId); 
}
