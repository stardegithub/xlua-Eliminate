using System;
using System.Collections.Generic;

public interface IObserver
{
    List<string> ObserverMessages { get; }
    void OnMessage(IMessage message);
}
