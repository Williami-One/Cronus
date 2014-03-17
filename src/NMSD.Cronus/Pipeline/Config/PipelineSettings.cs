﻿using NMSD.Cronus.Pipelining.Strategy;

namespace NMSD.Cronus.Pipelining.Config
{
    public class PipelineSettings
    {
        public IPipelineNameConvention PipelineNameConvention { get; set; }

        public IEndpointNameConvention EndpointNameConvention { get; set; }
    }
}