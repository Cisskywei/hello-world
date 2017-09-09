
namespace PaintCraft.Canvas{
	public interface ICanvasCommand {
		void BeforeCommand();
		void AfterCommand();

		void Undo();
		void Redo();
	}
}