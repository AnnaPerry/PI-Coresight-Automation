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
        public static AFElementTemplateSlim ToSlim(this AFElementTemplateDTO dto, IEnumerable<AFAttributeTemplateDTO> allAttributes)
        {
            //Populate element template
            AFElementTemplateSlim slim = new AFElementTemplateSlim()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                InstanceTypeName = dto.InstanceType,
                Path = dto.Path
            };

            //Populate attribute templates
            slim.AllAttributes = allAttributes.Select(a => a.ToSlim()).ToList();

            return slim;
        }

        public static AFAttributeTemplateSlim ToSlim(this AFAttributeTemplateDTO dto)
        {
            string pathRelativeToElement = dto.Path.Split(new char[] { '|' }, 2)[1];

            AFAttributeTemplateSlim slim = new AFAttributeTemplateSlim()
            {
                Id = dto.Id,
                FullName = pathRelativeToElement,
                Name = dto.Name,
                Description = dto.Description,
                IsTopLevel = !pathRelativeToElement.Contains('|'),
                TypeName = dto.Type,
                TypeQualifier = dto.TypeQualifier,
                IsHidden = dto.IsHidden,
                IsStatic = string.IsNullOrWhiteSpace(dto.DataReferencePlugIn),
                HasChildren = dto.HasChildren,
                CategoryNames = dto.CategoryNames
            };
            return slim;
        }
    }
}
