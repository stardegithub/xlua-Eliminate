using System;
using System.Collections.Generic;

public interface IController
{
    void RegisterCommand(string messageName, Type commandType);
    void RegisterObserver(IObserver observer, List<string> messageNames);

    void NotifyMessage(IMessage message);

	void RemoveCommand(string messageName);
    void RemoveObserver(IObserver observer, List<string> messageNames);

	bool HasCommand(string messageName);

    void ClearAll();
}
