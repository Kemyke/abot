using NLog;
using System;
using System.Threading;

namespace Abot.Util
{
    public class CachedMemoryMonitor : IMemoryMonitor, IDisposable
    {
        static ILogger _logger = LogManager.GetLogger("AbotLogger");
        IMemoryMonitor _memoryMonitor;
        Timer _usageRefreshTimer;
        int _cachedCurrentUsageInMb;

        public CachedMemoryMonitor(IMemoryMonitor memoryMonitor, int cacheExpirationInSeconds)
        {
            if (memoryMonitor == null)
                throw new ArgumentNullException("memoryMonitor");

            if (cacheExpirationInSeconds < 1)
                cacheExpirationInSeconds = 5;

            _memoryMonitor = memoryMonitor;

            UpdateCurrentUsageValue();

            _usageRefreshTimer = new Timer(UsageRefreshTimerElapsed, null, cacheExpirationInSeconds * 1000, cacheExpirationInSeconds * 1000);
        }

        private void UsageRefreshTimerElapsed(object state)
        {
            UpdateCurrentUsageValue();
        }

        protected virtual void UpdateCurrentUsageValue()
        {
            int oldUsage = _cachedCurrentUsageInMb;
            _cachedCurrentUsageInMb = _memoryMonitor.GetCurrentUsageInMb();
            _logger.Debug("Updated cached memory usage value from [{0}mb] to [{1}mb]", oldUsage, _cachedCurrentUsageInMb);
        }

        public virtual int GetCurrentUsageInMb()
        {
            return _cachedCurrentUsageInMb;
        }

        public void Dispose()
        {
            _usageRefreshTimer.Dispose();
        }
    }
}
