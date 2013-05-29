//     IAgentSettings.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

namespace Piston.Push.Core
{
    public interface IAgentSettings
    {
        int InitialRetryWaitSeconds { get; }
        double RetryGrowthFactor { get; }
        int MaxRetryWaitSeconds { get; }
        int RedeliveryWaitSeconds { get; }
    }
}
