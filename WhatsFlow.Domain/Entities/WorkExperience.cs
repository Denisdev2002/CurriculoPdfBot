using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVPdfBot.Domain.Entities
{
    public class WorkExperience
    {
        public string? Company { get; set; }
        public string? Role { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Period { get; set; }
        public List<string> Responsibilities { get; set; } = new();
    }
}
