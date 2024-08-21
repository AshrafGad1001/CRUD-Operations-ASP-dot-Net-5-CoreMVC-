using System.ComponentModel.DataAnnotations;

namespace CRUD_Operation.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(250)]
        public string Title { get; set; }
        [Required]
        public int Year { get; set; }
        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoryLine { get; set; }
        [Required]
        public byte[] Poster { get; set; }

        public byte GenreId { get; set; }
        public Genre Genre { get; set; }

    }
}
