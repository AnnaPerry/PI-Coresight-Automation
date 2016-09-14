using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoresightAutomation;
using CoresightAutomation.PIWebAPI;
using CoresightAutomation.PIWebAPI.DTO;
using CoresightAutomation.Types;

namespace CoresightAutomation.Demo.PIWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Create an example Coresight display");

            Console.WriteLine("Paste in your PI WebAPI URL:");
            Uri webApiBaseUri = new Uri(Console.ReadLine().TrimEnd('/') + "/");

            Console.WriteLine("Paste in your Coresight URL:");
            string coresightUri = Console.ReadLine();

            Console.WriteLine("Paste in the path to an AF Element whose template will form the default display");
            string elementPath = Console.ReadLine();

            Console.WriteLine("Optionally, provide a semicolon-delimited list of attribute categories to be included. By default, all are included.");
            string attributeCategoryList = Console.ReadLine();
            List<string> includedCategories = attributeCategoryList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            PIWebAPIClient piWebApiClient = new PIWebAPIClient(webApiBaseUri);
            AFElementDTO elementDTO = piWebApiClient.GetElementDTOByPathAsync(elementPath).GetAwaiter().GetResult();
            AFElementTemplateSlim elementTemplate = piWebApiClient.GetElementTemplateSlimAsync(elementDTO).GetAwaiter().GetResult();

            CoresightDefaultDisplayFactory displayFactory = new CoresightDefaultDisplayFactory(coresightUri);
            DisplayRevision defaultDisplay = displayFactory.CreateDefaultDisplayAsync(elementTemplate, elementPath, includedCategories).GetAwaiter().GetResult();

            Uri displayUri = defaultDisplay.GetUri(displayFactory.CoresightBaseUri);
            Console.WriteLine("New display created at {0}", displayUri);

            Console.ReadKey();
            Console.ReadKey();
        }
    }
}
