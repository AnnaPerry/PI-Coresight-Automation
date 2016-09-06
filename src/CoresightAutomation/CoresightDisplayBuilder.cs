using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoresightAutomation
{
    /// <summary>
    /// Creates a Display out of vertically-stacked Symbols.
    /// 
    /// Deserves refactoring; currently a bramble of responsibilities.
    /// </summary>
    public class CoresightDisplayBuilder
    {
        public CoresightDisplayBuilder(Uri coresightBaseUri, string displayName)
        {
            _coresightDisplayClient = new CoresightDisplayClient(coresightBaseUri);
            Display.Name = displayName;
        }

        public CoresightDisplayBuilder(CoresightDisplayClient displayClient)
        {
            _coresightDisplayClient = displayClient;
        }

        public void SetTimeRange(string startTime = "*-2h", string endTime = "*")
        {
            _coresightDisplayClient.DisplayWrapper.StartTime = startTime;
            _coresightDisplayClient.DisplayWrapper.StartTime = endTime;
        }

        public void Append(Symbol symbol, int padBottom = 5)
        {
            _symbolAppendCount++;
            Symbol heightAdjustedSymbol = symbol.CloneAtLocation(top: _top, left: symbol.Configuration.Left + PadLeft);
            if (string.IsNullOrWhiteSpace(heightAdjustedSymbol.Name))
            {
                heightAdjustedSymbol.Name = string.Format("Symbol{0}", _symbolAppendCount);
            }
            _top += heightAdjustedSymbol.Configuration.Height;
            Display.Symbols.Add(heightAdjustedSymbol);
            PadBottom(padBottom);
        }

        public async Task<DisplayRevision> SaveAsync()
        {
            return await _coresightDisplayClient.SaveAsync();
        }

        public int PadLeft { get; set; } = 10;

        private void PadBottom(int height)
        {
            _top += height;
        }

        private Display Display
        {
            get
            {
                return _coresightDisplayClient.DisplayWrapper.Display;
            }
        }

        private CoresightDisplayClient _coresightDisplayClient;
        private int _top = 10;
        private int _symbolAppendCount;
    }
}
