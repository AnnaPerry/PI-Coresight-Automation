using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.PIWebAPI.DTO
{
    /// <summary>
    /// A simplified data transfer object holding Coresight-relevant properties of an Element Template.
    /// Deserializable from the ElementTemplate object returned by the PI WebAPI.
    /// </summary>
    public class AFElementTemplateDTO
    {
        public string WebId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string InstanceType { get; set; }
        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();
        [JsonIgnore]
        public string BaseTemplateUri
        {
            get
            {
                return HasBaseTemplate ? Links[_baseTemplateLinkKey] : null;
            }
        }
        public bool HasBaseTemplate
        {
            get
            {
                return Links != null && Links.ContainsKey(_baseTemplateLinkKey);
            }
        }

        private const string _baseTemplateLinkKey = "BaseTemplate";
    }
}
