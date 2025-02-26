﻿using CRUD_Operation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUD_Operation.ViewModels
{
    public class MovieFormViewModle
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Title { get; set; }
        [Required]
        public int Year { get; set; }
        [Range(1, 10)]
        public double Rate { get; set; }
        [Required, StringLength(2500)]
        public string StoryLine { get; set; }
        [Display(Name = "Select Poster")]
        public byte[] Poster { get; set; }
        [Display(Name = "Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}
