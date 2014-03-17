using System;
using System.IO;
using NMSD.Cronus.Pipelining;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Framing.v0_9_1;

namespace NMSD.Cronus.Transports.RabbitMQ
{
    public sealed class RabbitMqPipeline : IRabbitMqPipeline, IDisposable
    {
        private RabbitMqSafeChannel safeChannel;
        private readonly PipelineType pipelineType;
        private RabbitMqSession session;

        private string name;

        public RabbitMqPipeline(string pipelineName, RabbitMqSession rabbitMqSession, PipelineType pipelineType)
        {
            this.pipelineType = pipelineType;
            this.name = pipelineName;
            this.session = rabbitMqSession;
        }

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            safeChannel = session.OpenSafeChannel();
        }

        public void Close()
        {
            safeChannel.Close();
            safeChannel = null;
        }

        public void Push(EndpointMessage message)
        {
            if (safeChannel == null)
            {
                throw new PipelineClosedException(String.Format("The Pipeline '{0}' is closed", name));
            }
            try
            {
                var properties = new BasicProperties();
                properties.Headers = message.Headers;
                properties.SetPersistent(true);
                properties.Priority = 9;
                safeChannel.Channel.BasicPublish(name, String.Empty, false, false, properties, message.Body);
            }
            catch (EndOfStreamException ex) { throw new PipelineClosedException(String.Format("The Pipeline '{0}' was closed", name), ex); }
            catch (AlreadyClosedException ex) { throw new PipelineClosedException(String.Format("The Pipeline '{0}' was closed", name), ex); }
            catch (OperationInterruptedException ex) { throw new PipelineClosedException(String.Format("The Pipeline '{0}' was closed", name), ex); }

        }
        public void Bind(IEndpoint endpoint)
        {
            //ClearOldHeaders
            if (safeChannel == null)
            {
                safeChannel = session.OpenSafeChannel();
            }
            safeChannel.Channel.QueueBind(endpoint.Name, name, endpoint.RoutingKey, endpoint.RoutingHeaders);
            safeChannel.Close();
            safeChannel = null;
        }

        public void Declare()
        {
            if (safeChannel == null)
            {
                safeChannel = session.OpenSafeChannel();
            }
            //safeChannel.Channel.ExchangeDeclare(name, pipelineType.ToString(), true, false, null);
            safeChannel.Channel.ExchangeDeclare(name, pipelineType.ToString());
            safeChannel.Close();
            safeChannel = null;
        }

        public sealed class PipelineType
        {
            private readonly string name;
            private readonly int value;

            public static readonly PipelineType Direct = new PipelineType(1, "direct");
            public static readonly PipelineType Fanout = new PipelineType(2, "fanout");
            public static readonly PipelineType Headers = new PipelineType(3, "headers");
            public static readonly PipelineType Topics = new PipelineType(4, "topic");

            private PipelineType(int value, string name)
            {
                this.name = name;
                this.value = value;
            }

            public override String ToString()
            {
                return name;
            }

        }
        public string Name
        {
            get { return name; }
        }

        public bool Equals(IPipeline other)
        {
            throw new NotImplementedException();
        }
    }
}