using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Energy.ProviderAdapterTests
{
    public class ScopedTimingsCollector : IDisposable
    {
        private readonly string _description;
        private readonly Stopwatch _stopwatch;
        public Dictionary<string, TimeSpan> CollectedData { get; set; } =  new Dictionary<string, TimeSpan>();
        public List<double> TotalMs { get; set; } = new List<double>();

        public ScopedTimingsCollector(string description)
        {
            _description = description;
            _stopwatch =  new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            CollectedData.Add(_description, _stopwatch.Elapsed);
        }
    }
}