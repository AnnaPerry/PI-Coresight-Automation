using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.PIWebAPI.DTO
{
    /// <summary>
    /// A simplified data transfer object holding Coresight-relevant properties of an Attribute Template.
    /// Deserializable from the AttributeTemplate object returned by the PI WebAPI.
    /// </summary>
    public class AFAttributeTemplateDTO
    {
        public string WebId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string TypeQualifier { get; set; }
        public bool IsHidden { get; set; }
        public string DataReferencePlugIn { get; set; }
        public bool HasChildren { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}
