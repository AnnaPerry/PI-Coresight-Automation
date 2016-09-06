using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    /// <summary>
    /// Passed to PI Coresight when creating a display
    /// </summary>
    public class DisplayWrapper
    {
        public DisplayWrapper(string requestId = null)
        {
            Display = new Display(requestId);
        }
        public string StartTime { get; set; } = "*-2h";
        public string EndTime { get; set; } = "*";
        public object EventFramePath { get; set; } = null;
        public Display Display { get; set; }
        public List<object> Attachments { get; set; } = new List<object>();
        public string TZ { get; set; } = "UTC";
    }

    /// <summary>
    /// Returned from PI Coresight after saving a display
    /// </summary>
    public class DisplayRevision
    {
        public int DisplayId { get; set; }
        public string RequestId { get; set; }
        public int Revision { get; set; }
        public string LinkName { get; set; }
        public Uri GetUri(Uri coresightBaseUri)
        {
            UriBuilder uriBuilder = new UriBuilder(coresightBaseUri);
            uriBuilder.Fragment = string.Format("/Displays/{0}", DisplayId);
            return uriBuilder.Uri;
        }
    }

    /// <summary>
    /// Returned from PI Coresight when getting an empty "NewDisplay". 
    /// Includes a RequestID which should be passed later when saving the populated display.
    /// </summary>
    public class DisplayInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public bool ReadOnly { get; set; }
        public int Revision { get; set; }
        public List<Symbol> Symbols { get; set; }
        public string RequestId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EventFramePath { get; set; }
    }

    public class Display
    {
        public Display(string requestId = null)
        {
            RequestId = requestId;
        }
        public int Id { get; set; } = -1;
        public string Name { get; set; }
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public int Revision { get; set; } = 0;
        public string EventFramePath { get; set; } = null;
        public List<Symbol> Symbols { get; set; } = new List<Symbol>();
    }

    public struct ValueScale
    {
        public bool axis { get; set; }
        public bool tickMarks { get; set; }
        public bool bands { get; set; }
        public int padding { get; set; }
    }

    public struct TimeScale
    {
        public bool axis { get; set; }
        public bool tickMarks { get; set; }
    }

    public struct TrendConfig
    {
        public ValueScale valueScale { get; set; }
        public TimeScale timeScale { get; set; }
        public int padding { get; set; }
        public bool nowPosition { get; set; }
        public int LegendWidth { get; set; }
    }

    public struct ValueScaleSetting
    {
        public int MinType { get; set; }
        public int MaxType { get; set; }
    }

    public class TrendConfiguration : Configuration
    {
        public string DataShape { get { return "Trend"; } }
        public int Width { get; set; } = 400;
        public TrendConfig TrendConfig { get; set; }
        public bool MultipleScales { get; set; } = true;
        public ValueScaleSetting ValueScaleSetting { get; set; }
        public int TimeScaleType { get; set; }
        public bool NowPosition { get; set; } = true;
        public List<int> HiddenTraceIndexes { get; set; } = new List<int>();

        public override Configuration CloneAtLocation(int? top = default(int?), int? left = default(int?))
        {
            TrendConfiguration clone = (TrendConfiguration)this.MemberwiseClone();
            clone.HiddenTraceIndexes = new List<int>(this.HiddenTraceIndexes);
            return SetLocation(clone, top, left);
        }
    }

    public class TableConfiguration : Configuration
    {
        public string DataShape { get { return "Table"; } }
        public int Width { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public List<string> ColumnWidths { get; set; } = new List<string>();
        public string SortColumn { get; set; }

        public override Configuration CloneAtLocation(int? top = default(int?), int? left = default(int?))
        {
            TableConfiguration clone = (TableConfiguration)this.MemberwiseClone();
            clone.Columns = new List<string>(this.Columns);
            clone.ColumnWidths = new List<string>(this.ColumnWidths);
            return SetLocation(clone, top, left);
        }
    }

    public class StaticTextConfiguration : Configuration
    {
        public string StaticText { get; set; } = string.Empty;
        public string LinkURL { get; set; }
        public bool NewTab { get; set; }
        public string Fill { get; set; } = "rgba(255,255,255,0)";
        public string Stroke { get; set; } = "rgba(255,255,255,1)";
        public int Rotation { get; set; }
        public override Configuration CloneAtLocation(int? top = null, int? left = null)
        {
            StaticTextConfiguration clone = (StaticTextConfiguration)this.MemberwiseClone();
            return SetLocation(clone, top, left);
        }
    }

    public abstract class Configuration
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Height { get; set; }
        public abstract Configuration CloneAtLocation(int? top = null, int? left = null);
        public Configuration SetLocation(Configuration configuration, int? top = null, int? left = null)
        {
            configuration.Top = top ?? configuration.Top;
            configuration.Left = left ?? configuration.Left;
            return configuration;
        }
    }

    public class Symbol
    {
        public string Name { get; set; }
        public string SymbolType { get; set; }
        public virtual Configuration Configuration { get; set; }
        public virtual List<string> DataSources { get; set; } = new List<string>();
        public Symbol CloneAtLocation(int? top = null, int? left = null)
        {
            return new Symbol()
            {
                Name = this.Name,
                SymbolType = this.SymbolType,
                Configuration = this.Configuration != null ? this.Configuration.CloneAtLocation(top, left) : null,
                DataSources = new List<string>(this.DataSources)
            };
        }
    }
}
