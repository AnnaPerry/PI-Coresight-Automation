using CoresightAutomation.AFSDK;
using CoresightAutomation.Types;
using OSIsoft.AF.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CoresightAutomation.Demo.AFSDK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Create an example Coresight display");

            Console.WriteLine("Paste in your Coresight URL:");
            string coresightUri = Console.ReadLine();

            Console.WriteLine("Paste in the path to an AF Element whose template will form the default display");
            string elementPath = Console.ReadLine();
            AFElement element = AFObjectHelper.Resolve<AFElement>(elementPath);
            if (element.Template == null)
            {
                throw new InvalidOperationException("Element must be based on a template");
            }

            AFElementTemplateSlim elementTemplateSlim = element.Template.ToSlim();

            CoresightDefaultDisplayFactory displayFactory = new CoresightDefaultDisplayFactory(coresightUri);
            DisplayRevision defaultDisplay = displayFactory.CreateDefaultDisplayAsync(elementTemplateSlim, elementPath).GetAwaiter().GetResult();

            Uri displayUri = defaultDisplay.GetUri(displayFactory.CoresightBaseUri);
            Console.WriteLine("New display created at {0}", displayUri);

            SetClipboardText(displayUri.ToString());
            Console.WriteLine("This URL has been copied to the clipboard");
            Console.ReadKey();
        }

        public static void SetClipboardText(string value)
        {
            Thread staThread = new Thread(_ => Clipboard.SetText(value));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}
