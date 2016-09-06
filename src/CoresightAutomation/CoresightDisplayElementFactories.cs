using CoresightAutomation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    public static class SymbolFactory
    {
        public static Symbol NewTitleText(string value, string name = null)
        {
            return new Symbol()
            {
                Name = name,
                SymbolType = "statictext",
                Configuration = new StaticTextConfiguration()
                {
                    StaticText = value,
                    Top = 0,
                    Left = 0,
                    Height = 35,
                    Fill = "rgba(255,255,255,0)",
                    Stroke = "rgba(255,255,255,1)"
                }
            };
        }

        public static Symbol NewTable(ICollection<string> elementAttributePaths, string name = null, int width = 500)
        {
            return new Symbol()
            {
                Name = name,
                SymbolType = "table",
                Configuration = new TableConfiguration()
                {
                    Top = 0,
                    Left = 0,
                    Height = 30 + 25 * elementAttributePaths.Count,
                    Width = width,
                    Columns = new List<string>() { "Label", "Value", "Units" },
                    ColumnWidths = new List<string>() { "250", "170", "70" },
                    SortColumn = "Label"
                },
                DataSources = new List<string>(elementAttributePaths)
            };
        }

        public static Symbol NewTrend(ICollection<string> elementAttributePaths, string name = null, int height = 450, int width = 800)
        {
            return new Symbol()
            {
                Name = name,
                SymbolType = "trend",
                Configuration = new TrendConfiguration()
                {
                    Top = 0,
                    Left = 0,
                    Height = height,
                    Width = width,
                    TrendConfig = new TrendConfig()
                    {
                        valueScale = new ValueScale()
                        {
                            axis = false,
                            tickMarks = true,
                            bands = true,
                            padding = 2
                        },
                        timeScale = new TimeScale()
                        {
                            axis = true,
                            tickMarks = true
                        },
                        padding = 2,
                        nowPosition = true,
                        LegendWidth = 150 //may overflow
                    },
                    MultipleScales = true,
                    TimeScaleType = 0,
                    NowPosition = true
                },
                DataSources = new List<string>(elementAttributePaths)
            };
        }
    }
}
