using System;

public interface IMessage
{
	string Name { get; }

	object Body { get; set; }
		
    string ToString();
}

