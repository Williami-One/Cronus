﻿using System;
using System.Collections.Generic;
using NMSD.Cronus.Pipelining;

namespace NMSD.Cronus.Transport.InMemory
{
    public class InMemoryEndpointFactory : IEndpointFactory
    {
        private readonly IEndpointNameConvention endpointNameConvention;
        private readonly InMemoryPipelineFactory pipelineFactory;

        public InMemoryEndpointFactory(InMemoryPipelineFactory pipelineFactory, IEndpointNameConvention endpointNameConvention)
        {
            this.endpointNameConvention = endpointNameConvention;
            this.pipelineFactory = pipelineFactory;
        }

        public IEndpoint CreateEndpoint(EndpointDefinition definition)
        {
            return InMemoryQueue.Current.GetOrAddEndpoint(definition);
        }

        public IEnumerable<EndpointDefinition> GetEndpointDefinitions(params Type[] handlerTypes)
        {
            return endpointNameConvention.GetEndpointDefinitions(handlerTypes);
        }
    }
}