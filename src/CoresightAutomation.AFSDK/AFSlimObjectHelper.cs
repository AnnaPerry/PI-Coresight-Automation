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
        public static AFElementTemplateSlim ToSlim(this AFElementTemplate fat)
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
            slim.AllAttributes = allAttributes.Select(a => a.ToSlim()).ToList();

            return slim;
        }

        public static AFAttributeTemplateSlim ToSlim(this AFAttributeTemplate fat)
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
                IsStatic = fat.DataReferencePlugIn == null,
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
