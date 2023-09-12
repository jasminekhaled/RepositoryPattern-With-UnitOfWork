using System.ComponentModel.DataAnnotations;

namespace Shopping.Core.Dtos.BookDtos.ResponseDtos
{
    public class CategoryDto
    {
        [MaxLength(length: 50)]
        [MinLength(length: 3)]
        public string Name { get; set; }
        
    }
}
