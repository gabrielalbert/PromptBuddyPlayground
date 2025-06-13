using System.ComponentModel.DataAnnotations;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class DynamicPrompt
    {
        [Key]
        public int DynamicPromptId { get; set; }
        [Required]
        public string UniqueName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string SystemPromptTemplate { get; set; }
        [Required]
        public string SystemPromptKeysCommaSeperated { get; set; }
        [Required]
        public string UserPromptTemplate { get; set; }
        [Required]
        public string UserPromptKeysCommaSeperated { get; set; }
        [Required]
        public string ModelName { get; set; }
        public int? Temperature { get; set; }
        public int? Seed { get; set; }
    }
}