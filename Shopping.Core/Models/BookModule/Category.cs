﻿using System.ComponentModel.DataAnnotations;

namespace Shopping.Core.Models.BookModule
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(length: 50)]
        [MinLength(length: 3)]
        public string Name { get; set; }
        public IEnumerable<BookCategories> booksCategories { get; set; }

    }
}

