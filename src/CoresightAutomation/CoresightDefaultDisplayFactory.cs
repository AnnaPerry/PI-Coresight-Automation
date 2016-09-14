using CoresightAutomation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    public class CoresightDefaultDisplayFactory
    {
        /// <summary>
        /// Constructs a display factory for the specified Coresight server.
        /// </summary>
        /// <param name="coresightUri">The Uri to your Coresight server. If the specified Uri contains a query or fragment, those will be stripped.</param>
        public CoresightDefaultDisplayFactory(string coresightUri)
        {
            //Uri.GetLeftPart unavailable in .NET Core; time for a hack
            string coresightUriSanitized = new Uri(coresightUri).GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped).TrimEnd('/') + "/";

            CoresightBaseUri = new Uri(coresightUriSanitized);
        }

        /// <summary>
        /// Constructs a display factory for the specified Coresight server.
        /// </summary>
        /// <param name="coresightBaseUri">Must be a well-formed Uri to a valid Coresight root. Typically, this will be http(s)://yourserver/coresight/</param>
        public CoresightDefaultDisplayFactory(Uri coresightBaseUri)
        {
            if (!coresightBaseUri.AbsolutePath.EndsWith("/"))
            {
                throw new ArgumentException("Uri must end with a /. To specify an unsanitized uri, use the constructor overload which accepts a string.");
            }
            else if (!string.IsNullOrEmpty(coresightBaseUri.Fragment))
            {
                throw new ArgumentException("Uri must not contain a fragment. To specify an unsanitized Uri, use the constructor overload which accepts a string.");
            }

            CoresightBaseUri = coresightBaseUri;
        }

        public Uri CoresightBaseUri { get; private set; }

        /// <summary>
        /// Creates a default display for a given Element Template and Element instance.
        /// </summary>
        /// <param name="elementTemplate">The element template on which the display will be based</param>
        /// <param name="initialContextElementPath">The element which will be the display's initial context</param>
        /// <param name="includedCategories">Optional list of categories to include. By default, all categories are included, as is the set of attributes not in a category. If specifying categories, the name @None will explicitly include uncategorized attributes.</param>
        /// <returns>A reference to the created display</returns>
        public Task<DisplayRevision> CreateDefaultDisplayAsync(AFElementTemplateSlim elementTemplate, string initialContextElementPath, ICollection<string> includedCategories = null)
        {
            //If the path is not escaped, do so here. TODO does this need to be URL encoded?
            if (!initialContextElementPath.Contains(@"\\\\"))
            {
                initialContextElementPath = initialContextElementPath.Replace(@"\", @"\\");
            }

            string name = string.Format("{0} default display", elementTemplate.Name); //TODO check collisions
            CoresightDisplayBuilder displayBuilder = new CoresightDisplayBuilder(CoresightBaseUri, name);

            IEnumerable<AFAttributeTemplateSlim> includedAttributes = elementTemplate.AllAttributes.Where(a => !a.IsHidden && ValueTypeHelper.TypeIsSupported(a.TypeName));
            Dictionary<string, IEnumerable<AFAttributeTemplateSlim>> attributesByCategory = AFAttributeTemplateSlim.GroupByCategory(includedAttributes, includedCategories);

            //If an explicit collection of included categories is specified, preserve its order
            bool explicitCategoriesSpecified = includedCategories != null && includedCategories.Count > 0;
            var attributesByCategoryOrdered = !explicitCategoriesSpecified ? 
                attributesByCategory.AsEnumerable() : 
                includedCategories.Select(c => new KeyValuePair<string, IEnumerable<AFAttributeTemplateSlim>>(c, attributesByCategory[c])); 

            foreach (var category in attributesByCategoryOrdered)
            {
                string titleText = AFAttributeTemplateSlim.NullCategory.Equals(category.Key, StringComparison.OrdinalIgnoreCase) ? 
                    "Uncategorized" : 
                    category.Key;
                titleText = attributesByCategory.Count < 2 ? null : titleText;

                AddAttributeGroup(displayBuilder, category.Value, initialContextElementPath, titleText);
            }

            return displayBuilder.SaveAsync();
        }

        private void AddAttributeGroup(CoresightDisplayBuilder displayBuilder, IEnumerable<AFAttributeTemplateSlim> attributeTemplates, string initialContextElementPath, string titleText = null)
        {
            IEnumerable<AFAttributeTemplateSlim> trendAttributeTemplates = attributeTemplates.Where(a => !a.IsStatic && ValueTypeHelper.TypeIsNumeric(a.TypeName));
            IEnumerable<AFAttributeTemplateSlim> tableAttributeTemplates = attributeTemplates.Except(trendAttributeTemplates);

            List<string> trendAttributePaths = trendAttributeTemplates.Select(a => GetAttributeInstancePath(initialContextElementPath, a, true)).ToList();
            List<string> tableAttributePaths = tableAttributeTemplates.Select(a => GetAttributeInstancePath(initialContextElementPath, a, true)).ToList();

            if (!string.IsNullOrWhiteSpace(titleText))
            {
                Symbol title = SymbolFactory.NewTitleText(titleText);
                displayBuilder.Append(title);
            }

            if (trendAttributePaths.Any())
            {
                IEnumerable<List<string>> trendAttributePathPages = Paginate(trendAttributePaths, 4, 3);
                foreach (ICollection<string> trendAttributePathPage in trendAttributePathPages)
                {
                    Symbol trend = SymbolFactory.NewTrend(trendAttributePathPage, height: 200);
                    displayBuilder.Append(trend);
                }
            }

            if (tableAttributePaths.Any())
            {
                Symbol table = SymbolFactory.NewTable(tableAttributePaths);
                displayBuilder.Append(table);
            }
        }

        private static IEnumerable<List<string>> Paginate(ICollection<string> items, int pageSize = 4, int? alternatePageSize = null)
        {
            Func<int> getIdealPageSize = () =>
            {
                if (!alternatePageSize.HasValue) return pageSize;

                int remainder = items.Count % pageSize;
                if (remainder == 0) return pageSize;

                int remainderAlt = items.Count % alternatePageSize.Value;
                if (remainderAlt == 0) return alternatePageSize.Value;

                return remainder > remainderAlt ? pageSize : alternatePageSize.Value;
            };

            int idealPageSize = getIdealPageSize();
            int pageCount = (int)Math.Ceiling(1.0f * items.Count / idealPageSize);

            for (int pageNumber = 0; pageNumber < pageCount; pageNumber++)
            {
                yield return items.Skip(pageNumber * idealPageSize).Take(idealPageSize).ToList();
            }
        }

        private static string GetAttributeInstancePath(string elementPath, AFAttributeTemplateSlim attributeTemplate, bool includeProtocol = true)
        {
            string protocol = includeProtocol ? "af:" : string.Empty;
            return string.Concat(protocol, elementPath, "|", attributeTemplate.FullName);
        }
    }
}
