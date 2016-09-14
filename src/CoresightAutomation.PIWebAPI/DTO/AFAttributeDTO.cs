using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.PIWebAPI.DTO
{
    public class AFAttributeDTO
    {
        public string WebId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string TypeQualifier { get; set; }
        public string DefaultUnitsName { get; set; }
        public string DataReferencePlugIn { get; set; }
        public string ConfigString { get; set; }
        public bool IsConfigurationItem { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsHidden { get; set; }
        public bool IsManualDataEntry { get; set; }
        public bool HasChildren { get; set; }
        public List<string> CategoryNames { get; set; }
        public bool Step { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public string GetPathRelativeToElement()
        {
            return Path.Split(new char[] { '|' }, 2)[1];
        }
    }
}
