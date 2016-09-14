using CoresightAutomation.Types;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation.AFSDK
{
    public static class AFSlimObjectHelper
    {
        /// <summary>
        /// Slims an AF SDK AFElementTemplate
        /// </summary>
        /// <param name="fat">The template to be slimmed</param>
        /// <param name="instance">Optional element instance to provide data reference information if not set on the template</param>
        /// <returns></returns>
        public static AFElementTemplateSlim ToSlim(this AFElementTemplate fat, AFElement instance = null)
        {
            //Populate element template
            AFElementTemplateSlim slim = new AFElementTemplateSlim()
            {
                Id = fat.ID,
                Name = fat.Name,
                Description = fat.Description,
                InstanceTypeName = fat.InstanceType.Name,
                Path = fat.GetPath().Replace(@"\", @"\\") //escape the backslashes
            };

            //Populate attribute templates
            IEnumerable<AFAttributeTemplate> allAttributes = fat.GetAllAttributeTemplatesAndChildren();
            slim.AllAttributes = allAttributes.Select(a => a.ToSlim(instance?.Attributes[a.GetPath(a.ElementTemplate)])).ToList();

            return slim;
        }

        /// <summary>
        /// Slims an AF SDK AFAttributeTemplate
        /// </summary>
        /// <param name="fat">The template to be slimmed</param>
        /// <param name="instance">Optional element instance to provide data reference information if not set on the template</param>
        /// <returns></returns>
        public static AFAttributeTemplateSlim ToSlim(this AFAttributeTemplate fat, AFAttribute instance = null)
        {
            AFAttributeTemplateSlim slim = new AFAttributeTemplateSlim()
            {
                Id = fat.ID,
                FullName = fat.GetPath(fat.ElementTemplate),
                Name = fat.Name,
                Description = fat.Description,
                IsTopLevel = fat.Parent == null,
                TypeName = fat.Type.Name,
                TypeQualifier = fat.TypeQualifier is IAFNamedObject ? ((IAFNamedObject)fat.TypeQualifier).Name : null,
                IsHidden = fat.IsHidden, //requires AF Client 2.7 or higher
                IsStatic = fat.DataReferencePlugIn == null && instance?.DataReferencePlugIn == null,
                HasChildren = fat.AttributeTemplates != null && fat.AttributeTemplates.Count > 0,
                CategoryNames = fat.Categories.Select(c => c.Name).ToList()
            };
            return slim;
        }

        private static IEnumerable<AFAttributeTemplate> GetAllAttributeTemplatesAndChildren(this AFElementTemplate elementTemplate)
        {
            return GetAllAttributeTemplatesAndChildren(elementTemplate.GetAllAttributeTemplates());
        }

        private static IEnumerable<AFAttributeTemplate> GetAllAttributeTemplatesAndChildren(IEnumerable<AFAttributeTemplate> attributeTemplates)
        {
            return attributeTemplates.SelectMany(a =>
                new AFAttributeTemplate[] { a }.Concat(GetAllAttributeTemplatesAndChildren(a)));
        }

        private static IEnumerable<AFAttributeTemplate> GetAllAttributeTemplatesAndChildren(AFAttributeTemplate attributeTemplate)
        {
            return attributeTemplate.AttributeTemplates
              .Concat(attributeTemplate.AttributeTemplates
                  .OfType<AFAttributeTemplate>()
                  .SelectMany(a => GetAllAttributeTemplatesAndChildren(a)));
        }
    }
}
