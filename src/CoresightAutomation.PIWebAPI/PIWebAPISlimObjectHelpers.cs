using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoresightAutomation.PIWebAPI.DTO;
using CoresightAutomation.Types;

namespace CoresightAutomation.PIWebAPI
{
    public static class PIWebAPISlimObjectHelpers
    {
        /// <summary>
        /// Slims an AF SDK AFElementTemplate
        /// </summary>
        /// <param name="elementTemplate">The template to be slimmed</param>
        /// <param name="attributeTemplates">The template's attributes</param>
        /// <param name="attributeInstances">Optional attribute instances to provide data reference information if not set on the template</param>
        /// <returns></returns>
        public static AFElementTemplateSlim ToSlim(this AFElementTemplateDTO elementTemplate, IEnumerable<AFAttributeTemplateDTO> attributeTemplates, IEnumerable<AFAttributeDTO> attributeInstances = null)
        {
            //Populate element template
            AFElementTemplateSlim slim = new AFElementTemplateSlim()
            {
                Id = elementTemplate.Id,
                Name = elementTemplate.Name,
                Description = elementTemplate.Description,
                InstanceTypeName = elementTemplate.InstanceType,
                Path = elementTemplate.Path
            };

            //Populate attribute templates
            var attributeInstancesByFullName = attributeInstances.ToDictionary(a => a.GetPathRelativeToElement());
            var attributeTemplatesByFullName = attributeTemplates.ToDictionary(a => a.GetPathRelativeToElement());

            slim.AllAttributes = attributeTemplatesByFullName.Select(kvp => kvp.Value.ToSlim(attributeInstancesByFullName[kvp.Key])).ToList();

            return slim;
        }

        /// <summary>
        /// Slims an AF SDK AFAttributeTemplate
        /// </summary>
        /// <param name="attributeTemplate">The template to be slimmed</param>
        /// <param name="attributeInstance">Optional attribute instance to provide data reference information if not set on the template</param>
        /// <returns></returns>
        public static AFAttributeTemplateSlim ToSlim(this AFAttributeTemplateDTO attributeTemplate, AFAttributeDTO attributeInstance = null)
        {
            string pathRelativeToElement = attributeTemplate.GetPathRelativeToElement();
            
            AFAttributeTemplateSlim slim = new AFAttributeTemplateSlim()
            {
                Id = attributeTemplate.Id,
                FullName = pathRelativeToElement,
                Name = attributeTemplate.Name,
                Description = attributeTemplate.Description,
                IsTopLevel = !pathRelativeToElement.Contains('|'),
                TypeName = attributeTemplate.Type,
                TypeQualifier = attributeTemplate.TypeQualifier,
                IsHidden = attributeTemplate.IsHidden,
                IsStatic = string.IsNullOrWhiteSpace(attributeTemplate.DataReferencePlugIn) && string.IsNullOrWhiteSpace(attributeInstance.DataReferencePlugIn),
                HasChildren = attributeTemplate.HasChildren,
                CategoryNames = attributeTemplate.CategoryNames
            };
            return slim;
        }
    }
}
