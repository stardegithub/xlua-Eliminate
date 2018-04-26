using System;
public class Message : IMessage
{
    public Message(string name) : this(name, null)
    {
    }

    public Message(string name, object body)
    {
        _name = name;
        _body = body;
    }

    /// <summary>
    /// Get the string representation of the <c>Notification instance</c>
    /// </summary>
    /// <returns>The string representation of the <c>Notification</c> instance</returns>
    public override string ToString()
    {
        string msg = "Notification Name: " + Name;
        msg += "\nBody:" + ((Body == null) ? "null" : Body.ToString());
        return msg;
    }

    /// <summary>
    /// The name of the <c>Notification</c> instance
    /// </summary>
    /// <remarks>This accessor is thread safe</remarks>
    public virtual string Name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// The body of the <c>Notification</c> instance
    /// </summary>
    /// <remarks>This accessor is thread safe</remarks>
    public virtual object Body
    {
        get
        {
            // Setting and getting of reference types is atomic, no need to lock here
            return _body;
        }
        set
        {
            // Setting and getting of reference types is atomic, no need to lock here
            _body = value;
        }
    }

    /// <summary>
    /// The name of the notification instance
    /// </summary>
	private string _name;

    /// <summary>
    /// The body of the notification instance
    /// </summary>
	private object _body;
}

