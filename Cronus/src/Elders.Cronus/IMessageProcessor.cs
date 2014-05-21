using System;
using System.Collections.Generic;

namespace Elders.Cronus
{
    public interface IMessageProcessor<TMessage>
    {
        ISafeBatchResult<TMessage> Handle(List<TMessage> messages);
        IEnumerable<Type> GetRegisteredHandlers();
    }
}