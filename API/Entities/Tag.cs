using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}