using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.Types
{
    public class AFElementTemplateSlim
    {
        /// <summary>
        /// The unique ID within the PI Asset Framework Server
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Template's name, unque within each PI Asset Framework Server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Free-form description of the Element Template
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Absolute path to the Element Template. Should be in escaped format, i.e. \\\\myAfServer\\myDatabase\\...
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Indicates the type of element this template is for: AFElement, AFEventFrame, etc.
        /// </summary>
        public string InstanceTypeName { get; set; }

        /// <summary>
        /// The attributes of this element.
        /// Because the AFAttributeTemplateSlim object has no property to hold its children, 
        /// this Attributes property is intended to hold a flattened collection of attributes of this template and all attributes inherited from base template(s).
        /// </summary>
        public List<AFAttributeTemplateSlim> AllAttributes { get; set; } = new List<AFAttributeTemplateSlim>();
    }
}
