using System;

/// <summary>
/// 事件命令类接口
/// </summary>
public interface ICommand
{
	/// <summary>
	/// 收到消息，执行命令
	/// </summary>
	/// <param name="message">消息</param>
    void Execute(IMessage message);
}

