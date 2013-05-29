//     Agent.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Piston.Push.Core
{
    public abstract class Agent : IDisposable
    {
        protected static SettingsSection Settings = (SettingsSection)ConfigurationManager.GetSection("pushGateway");

        protected static ushort ParseHostPort(ref string host, ushort defaultPort)
        {
            string[] parts = host.Split(new[] { ':' }, 2);
            host = parts[0];
            ushort port = defaultPort;
            if (parts.Length > 1)
                ushort.TryParse(parts[1], out port);
            return port;
        }

        Thread _worker;
        IAgentSettings _agentSettings;
        int _retryWaitSeconds;

        public Agent(AppSecret secret, INotificationConsumer consumer, ITokenStore store, IAgentSettings agentSettings)
        {
            this.Secret = secret;
            this.Consumer = consumer;
            this.Store = store;
            _agentSettings = agentSettings;
            _retryWaitSeconds = agentSettings.InitialRetryWaitSeconds;
        }

        public void Start()
        {
            _worker = new Thread(DoLoop);
            _worker.Name = this.AgentId;
            _worker.Start();
        }

        public virtual void Dispose()
        {
            this.Consumer.Dispose();
            this.IsDisposed = true;
            _worker.Join();
        }

        protected AppSecret Secret { get; private set; }
        protected INotificationConsumer Consumer { get; private set; }
        protected ITokenStore Store { get; private set; }
        protected bool IsDisposed { get; private set; }
        protected string AgentId { get { return this.Consumer.AppId + "." + this.Consumer.Platform.ToString().ToLower(); } }

        protected abstract void OnNotify(NotificationEventArgs e);

        private void DoLoop()
        {
            while (!this.IsDisposed)
            {
                uint now = System.DateTime.Now.ToUnixEpoch();
                using (NotificationEventArgs note = this.Consumer.Dequeue())
                {
                    if (note == null)
                        return;
                    if (note.Expiry > 0 && now > note.Expiry)
                        continue;

                    try
                    {
                        Trace.TraceInformation("[{0}] Sending: {1}", this.AgentId, note.ToString());
                        this.OnNotify(note);
                        _retryWaitSeconds = _agentSettings.InitialRetryWaitSeconds;
                    }
                    catch (UndeliverableException ex)
                    {
                        Trace.TraceError("[{0}] Undeliverable ({1}): {2}", this.AgentId, ex.Message, note.ToString());
                        note.Reject(false);
                    }
                    catch (TemporaryUndeliverableException ex)
                    {
                        Trace.TraceError("[{0}] Temporarily undeliverable ({1}): {2}", this.AgentId, ex.Message, note.ToString());
                        this.Redeliver(note, _agentSettings.RedeliveryWaitSeconds);
                    }
                    catch (TemporaryServiceException ex)
                    {
                        Trace.TraceError("[{0}] Service temporarily unavailable ({1})", this.AgentId, ex.Message);
                        note.Reject(true);
                        System.Threading.Thread.Sleep(_retryWaitSeconds * 1000);
                        _retryWaitSeconds = (int)Math.Min((double)_agentSettings.MaxRetryWaitSeconds, (double)(_retryWaitSeconds * _agentSettings.RetryGrowthFactor));
                    }
                    catch (FatalServiceException ex)
                    {
                        Trace.TraceError("[{0}] Fatal service failure ({1})", this.AgentId, ex.Message);
                        this.Consumer.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("[{0}] General exception: {1}", this.AgentId, ex.ToString());
                        note.Reject(false);
                    }
                }
            }
        }

        private async void Redeliver(NotificationEventArgs note, int redeliverSeconds)
        {
            await Task.Delay(redeliverSeconds * 1000);
            try
            {
                note.Reject(true);
            }
            catch
            {
                // If connection to the queue server was broken and re-established,
                // this will be expected to fail.
            }
        }

        protected void Connect(string hostAndPort, ref TcpClient client, ref SslStream stream, int maxTries = int.MaxValue)
        {
            if (stream != null)
                stream.Close();

            string host = hostAndPort;
            ushort port = ParseHostPort(ref host, 80);

            int tries = 0;
            while (tries++ < maxTries)
            {
                try
                {
                    Trace.TraceInformation("[{0}] Connecting to {1}:{2}", this.AgentId, host, port.ToString());
                    client = new TcpClient(host, port);
                    stream = new SslStream(client.GetStream(), false);
                    stream.AuthenticateAsClient(host, this.Secret.GetX509Certificates(), SslProtocols.Default, false);
                    Trace.TraceInformation("[{0}] Connected to {1}:{2}", this.AgentId, host, port.ToString());
                    return;
                }
                catch (AuthenticationException ex)
                {
                    Trace.TraceError("[{0}] Authentication failure: {1}", this.AgentId, ex.Message);
                    throw new FatalServiceException("Unable to authenticate", ex);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("[{0}] Unable to connect: {1}", this.AgentId, ex.ToString());
                    Thread.Sleep(Settings.Apns.ConnectRetryDelay * 1000);
                }
            }
        }
    }
}
