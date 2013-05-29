//     ApnsAgent.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Piston.Push.Core
{
    public class ApnsAgent : Agent
    {
        static ApnsElement _settings = Settings.Apns;

        const int DefaultApnsPort = 2195;
        const int DefaultFeedbackPort = 2196;
        const int MaxPayloadLength = 256;

        TcpClient _client;
        SslStream _stream;
        int _curMessageId = 0;
        readonly byte[] _readBuffer;
        readonly Timer _feedbackTimer;

        public ApnsAgent(AppSecret secret, INotificationConsumer consumer, ITokenStore store)
            : base(secret, consumer, store, _settings)
        {
            _readBuffer = new byte[6];
            var feedbackInterval = Settings.Apns.FeedbackIntervalSeconds * 1000;
            _feedbackTimer = new Timer(OnQueryFeedback, null, feedbackInterval, feedbackInterval);
        }

        // Nothing should ever be read unless the connection is about to close.
        // Log the end result and abandon the connection.
        private void OnRead(IAsyncResult iar)
        {
            try
            {
                if (_stream.EndRead(iar) >= _readBuffer.Length && _readBuffer[0] == 8) // 8 = error command
                {
                    int status = _readBuffer[1];
                    Trace.TraceError("[{0}] Received error code {1}", this.AgentId, status);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("[{0}] Socket error {1}", this.AgentId, ex.Message);
            }
            finally
            {
                Trace.TraceWarning("[{0}] Disconnected", this.AgentId);
                _stream.Close();
            }
        }

        protected override void OnNotify(NotificationEventArgs e)
        {
            if (_client == null || !_client.Connected)
            {
                Connect(Settings.Apns.GatewayHost, ref _client, ref _stream);
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, OnRead, null);
            }

            var payloadJson = JsonConvert.SerializeObject(e.Payload);
            var payloadLength = Encoding.UTF8.GetByteCount(payloadJson);

            if (payloadLength > MaxPayloadLength)
                throw new UndeliverableException("Payload too large.");
            var buf = new byte[45 + payloadLength];

            try
            {
                DeviceToken.Validate(Platform.Apns, e.DeviceToken);
            }
            catch (ValidationException)
            {
                throw new UndeliverableException("Invalid device token.");
            }

            FormattingExtensions.FromHexString(e.DeviceToken);

            buf[0] = 1; // command (always 1)
            (_curMessageId++).ToBytesNetworkOrder(buf, 1); // incrementing reference number
            e.Expiry.ToBytesNetworkOrder(buf, 5); // UNIX epoch expiry

            // token length and token
            buf[9] = 0;
            buf[10] = 32; // length (always 32)
            FormattingExtensions.FromHexString(e.DeviceToken, buf, 11);

            // payload length and payload
            ((short)payloadLength).ToBytesNetworkOrder(buf, 43);
            Encoding.UTF8.GetBytes(payloadJson, 0, payloadJson.Length, buf, 45);

            _stream.Write(buf);
        }

        private void OnQueryFeedback(object state)
        {
            TcpClient client = null;
            SslStream stream = null;
            var readBuffer = new byte[38];

            Trace.TraceInformation("[{0}] Starting feedback query", this.AgentId);
            try
            {
                this.Connect(Settings.Apns.FeedbackHost, ref client, ref stream, 1);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("[{0}] Unable to query feedback service: {1}", this.AgentId, ex.Message);
                return;
            }

            if (!client.Connected)
                return;

            while (true)
            {
                int count = stream.Read(readBuffer, 0, readBuffer.Length);
                if(count < readBuffer.Length)
                    break;
                uint time = (uint)FormattingExtensions.FromBytesNetworkOrder(readBuffer, 0);
                string token = FormattingExtensions.ToHexString(readBuffer, 6, 32);
                Trace.TraceInformation("[{0}] Feedback received: Time={1}, Token={2}", this.AgentId, time, token);
                try
                {
                    this.Store.Unregister(this.Consumer.AppId, Platform.Apns, token, time);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("[{0}] Unable to unregister device: {1}", ex.Message);
                }
            }

            Trace.TraceInformation("[{0}] Feedback query ended", this.AgentId);
        }

        public override void Dispose()
        {
            _feedbackTimer.Dispose();
            base.Dispose();
        }
    }
}
