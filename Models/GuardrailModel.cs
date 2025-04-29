using System;

namespace PromptEngineering.Models
{
    public class GuardrailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
