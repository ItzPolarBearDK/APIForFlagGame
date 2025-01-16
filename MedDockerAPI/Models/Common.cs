using System.ComponentModel.DataAnnotations.Schema;

namespace MedDockerAPI.Models
{
    public class Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}