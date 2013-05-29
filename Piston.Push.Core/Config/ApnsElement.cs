//     ApnsElement.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Configuration;

namespace Piston.Push.Core
{
    public class ApnsElement : ConfigurationElement, IAgentSettings
    {
        [ConfigurationProperty("gatewayHost", DefaultValue = "gateway.sandbox.push.apple.com:2195")]
        public string GatewayHost
        {
            get { return (string)this["gatewayHost"]; }
            set { this["gatewayHost"] = value; }
        }

        [ConfigurationProperty("connectRetrySeconds", DefaultValue = 60)]
        public int ConnectRetryDelay
        {
            get { return (int)this["connectRetrySeconds"]; }
            set { this["connectRetrySeconds"] = value; }
        }

        [ConfigurationProperty("feedbackHost", DefaultValue = "feedback.sandbox.push.apple.com:2196")]
        public string FeedbackHost
        {
            get { return (string)this["feedbackHost"]; }
            set { this["feedbackHost"] = value; }
        }

        [ConfigurationProperty("feedbackIntervalSeconds", DefaultValue = 360)]
        public int FeedbackIntervalSeconds
        {
            get { return (int)this["feedbackIntervalSeconds"]; }
            set { this["feedbackIntervalSeconds"] = value; }
        }

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
