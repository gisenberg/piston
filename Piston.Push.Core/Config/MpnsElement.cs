//     MpnsElement.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Configuration;

namespace Piston.Push.Core
{
    public class MpnsElement : ConfigurationElement, IAgentSettings
    {
        [ConfigurationProperty("initialRetryWaitSeconds", DefaultValue = 60)]
        public int InitialRetryWaitSeconds
        {
            get { return (int)this["initialRetryWaitSeconds"]; }
            set { this["initialRetryWaitSeconds"] = value; }
        }

        [ConfigurationProperty("retryGrowthFactor", DefaultValue = 2.0)]
        public double RetryGrowthFactor
        {
            get { return (double)this["retryGrowthFactor"]; }
            set { this["retryGrowthFactor"] = value; }
        }

        [ConfigurationProperty("maxRetryWaitSeconds", DefaultValue = 3600)]
        public int MaxRetryWaitSeconds
        {
            get { return (int)this["maxRetryWaitSeconds"]; }
            set { this["maxRetryWaitSeconds"] = value; }
        }

        [ConfigurationProperty("redeliveryWaitSeconds", DefaultValue = 3600)]
        public int RedeliveryWaitSeconds
        {
            get { return (int)this["redeliveryWaitSeconds"]; }
            set { this["redeliveryWaitSeconds"] = value; }
        }
    }
}
