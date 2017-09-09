using UnityEngine;
using System.Collections;
using ColoringBook.Controllers;


namespace PaintCraft.Canvas{
	public class SnapshotCommand : ICanvasCommand {
		UndoManager undoManager;
		SnapshotData stateBefore;
		SnapshotData stateAfter;


		public SnapshotCommand(UndoManager undoManager){
			this.undoManager = undoManager;
		}

		#region ICanvasCommand implementation

		public void BeforeCommand ()
		{
			stateBefore = undoManager.SnapshotPool.GetCurrentSnapshotData();
			if (stateBefore == null){
				//very first drawing here
				stateBefore = undoManager.SnapshotPool.MakeSnapshot();
			}
		}

		public void AfterCommand ()
		{
			if (!undoManager.SnapshotPool.HasFreeSnapshotSlot()){
				undoManager.RemoveFirstSnapshotFromHistory();
			}
			stateAfter = undoManager.SnapshotPool.MakeSnapshot();
		}

		public void Undo ()
		{
			undoManager.SnapshotPool.Undo(stateBefore);
		}

		public void Redo ()
		{
			undoManager.SnapshotPool.Redo(stateAfter);
		}
		#endregion
	}
}