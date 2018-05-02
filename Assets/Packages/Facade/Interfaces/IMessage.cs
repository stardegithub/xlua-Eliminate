using System;

/// <summary>
/// 消息类接口
/// </summary>
public interface IMessage
{
	/// <summary>
	/// 消息名字
	/// </summary>
	/// <returns></returns>
	string Name { get; }

	/// <summary>
	/// 消息体
	/// </summary>
	/// <returns></returns>
	object Body { get; set; }

	/// <summary>
	/// 消息转字符串
	/// </summary>
	/// <returns></returns>	
    string ToString();
}

