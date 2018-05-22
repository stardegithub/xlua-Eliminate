
namespace Facade.Core {

	/// <summary>
	/// 事件命令基类
	/// </summary>
	public abstract class ControllerCommand : ICommand
	{
		public virtual void Execute(IMessage message)
		{
		}
	}
}