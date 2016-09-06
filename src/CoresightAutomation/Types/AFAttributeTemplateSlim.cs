using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.Types
{
    public class AFAttributeTemplateSlim
    {
        /// <summary>
        /// The unique ID within the PI Asset Framework Server
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The full name of the attribute relative to its template.
        /// For example, if the attribute named X is a child of attribute Location, the FullName of X would be Location|X
        /// This value is unique per element template.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The relative name of the attribute.
        /// This value is unique per Attributes collection. 
        /// THis value is not guaranteed to be uniqe per Element, because Attributes collections may be nested (i.e. when Attributes have children)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Free-form description of the Attribute Template
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if this Attribute is the direct child of its Element.
        /// False if this Attribute is the child of another Attribute.
        /// </summary>
        public bool IsTopLevel { get; set; }

        /// <summary>
        /// The name of the Attribute Template's value type
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// If the value type is complex (e.g. AFEnumerationSet), 
        /// this qualifier specifies sub-type (e.g. which enumeration set)
        /// </summary>
        public string TypeQualifier { get; set; }

        /// <summary>
        /// Indicates that the value is not for public consumption, and is instead an intermediate field used for calculations or integration.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Indicates that the Attribute Template is not backed by a Data Reference.
        /// This is not a guarantee that Element instances implementing this Template will or will not use a Data Reference.
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Indicates if the attribute has children.
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// The names of the categories to which this Attribute Template as been assigned.
        /// </summary>
        public List<string> CategoryNames { get; set; } = new List<string>();

        /// <summary>
        /// Group a collection of attriute templates by the categories they are in.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Dictionary<string, IEnumerable<AFAttributeTemplateSlim>> GroupByCategory(IEnumerable<AFAttributeTemplateSlim> attributes)
        {
            IEnumerable<AFAttributeTemplateSlim> topLevelAttributes = attributes.Where(a => a.IsTopLevel);
            List<string> topLevelCategories = topLevelAttributes.SelectMany(a => a.CategoryNames).Distinct().ToList();

            //Group the attributes by category
            var attributesByCategory = topLevelCategories.ToDictionary(c => c, c => attributes.Where(a => a.CategoryNames.Contains(c)));

            //Account for attributes which are uncategorized
            attributesByCategory.Add(NullCategory, attributes.Where(a => !a.CategoryNames.Any()));

            return attributesByCategory;
        }

        public static readonly string NullCategory = string.Empty;

    }
}
