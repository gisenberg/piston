//     RabbitConsumer.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Piston.Push.Core
{
    public class RabbitConsumer : RabbitContext, INotificationConsumer
    {
        const string Exchange = "";

        EventingBasicConsumer _consumer;
        ConcurrentQueue<NotificationEventArgs> _queue;
        ManualResetEventSlim _wait;
        bool _isDisposed = false;

        public RabbitConsumer(Platform platform, string appId)
        {
            this.Platform = platform;
            this.AppId = appId;
            _wait = new ManualResetEventSlim();

            this.Initialize();
        }

        public Platform Platform { get; private set; }
        public string AppId { get; private set; }
        public string QueueName { get { return string.Format("{0}.{1}", this.AppId, this.Platform.ToString().ToLower()); } }

        public NotificationEventArgs Dequeue()
        {
            NotificationEventArgs note = null;
            while (!_isDisposed && !_queue.TryDequeue(out note))
            {
                _wait.Wait();
                _wait.Reset();
            }
            return note;
        }

        public void Accept(NotificationEventArgs note)
        {
            try
            {
                this.Model.BasicAck((ulong)note.Tag, false);
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
            {
                // Do nothing.
            }
        }

        public void Reject(NotificationEventArgs note, bool requeue)
        {
            try
            {
                this.Model.BasicNack((ulong)note.Tag, false, requeue);
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
            {
                // Do nothing.
            }
        }

        private void Initialize()
        {
            _queue = new ConcurrentQueue<NotificationEventArgs>();
            this.Model.QueueDeclare(this.QueueName, true, false, false, null);
            _consumer = new EventingBasicConsumer();
            _consumer.Received += _consumer_Received;
            _consumer.Shutdown += _consumer_Shutdown;
            this.Model.BasicConsume(this.QueueName, false, _consumer);
        }

        public override void Dispose()
        {
            _isDisposed = true;
            base.Dispose();
            _wait.Set();
        }

        private void _consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
            if (!_isDisposed)
            {
                Trace.TraceWarning("[{0}] disconnected from queue, reconnecting...", this.QueueName);
                this.Initialize();
            }
        }

        private void _consumer_Received(IBasicConsumer sender, BasicDeliverEventArgs args)
        {
            var payload = JObject.Parse(Encoding.UTF8.GetString(args.Body));
            string token = null;
            IBasicProperties props = args.BasicProperties;

            uint expiry = 0;

            if (payload == null
                || !props.IsHeadersPresent()
                || string.IsNullOrWhiteSpace((token = props.GetStringHeader("device-token")))
                || !uint.TryParse(props.GetStringHeader("expiry"), out expiry))
            {
                // the message is too incomplete to handle
                this.Model.BasicNack(args.DeliveryTag, false, false);
                return;
            }
            _queue.Enqueue(new NotificationEventArgs(this, token, expiry, payload, args.DeliveryTag));
            _wait.Set();
        }
    }
}
