using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.PIWebAPI.DTO
{
    public class AFElementDTO
    {
        public string WebId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string TemplateName { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}
