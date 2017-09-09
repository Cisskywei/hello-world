using UnityEngine;
using PaintCraft.Controllers;
using PaintCraft.Utils;
using System.Collections.Generic;
using PaintCraft.Canvas;
using PaintCraft;


namespace PaintCraft.Canvas{
    public class UndoManager
    {
        public SnapshotPool SnapshotPool { get; private set; }

        LinkedList<ICanvasCommand> commandHistory = new LinkedList<ICanvasCommand>();

        public UndoManager(CanvasController canvas, int snapshotSize)
        {
            SnapshotPool = GOUtil.AddComponentIfNoExists<SnapshotPool>(canvas.CanvasCameraController.gameObject);
            SnapshotPool.Init(snapshotSize, canvas.RenderTextureSize);
        }

        public void Reinit(Vector2 renderTextureSize){            
            SnapshotPool.ResetStatus(renderTextureSize);
            commandHistory.Clear();
            activeNode = null;
        }



        LinkedListNode<ICanvasCommand> activeNode;       
        public void AddNewCommandToHistory(ICanvasCommand canvasCommand)
        {
            if (activeNode == null && commandHistory.Count > 0)
            {
                //this happens then we used all undo history
                commandHistory.Clear();
            }
            else if (activeNode != commandHistory.Last)
            {
                //used some of undo but not all
                RemoveAllSnapshotAfterActive();
            }

            activeNode = new LinkedListNode<ICanvasCommand>(canvasCommand);
            commandHistory.AddLast(activeNode);
        }


        public void Undo()
        {
            if (activeNode != null)
            {
                AnalyticsWrapper.CustomEvent("Undo", new Dictionary<string, object>());
                activeNode.Value.Undo();
                activeNode = activeNode.Previous; //could be null here
            }
            else
            {
                Debug.LogWarning("don't have undo");
            }
            
        }

        public bool HasUndo()
        {
            return activeNode != null;
        }

        public void Redo()
        {
            if (activeNode == null && commandHistory.Count > 0)
            {
                activeNode = commandHistory.First;
            }
            else if (activeNode.Next != null)
            {
                activeNode = activeNode.Next;
            }
            else
            {
                Debug.LogWarning("don't have redo");
                return;
            }
            AnalyticsWrapper.CustomEvent("Redo", new Dictionary<string, object>());
            activeNode.Value.Redo();
        }

        public bool HasRedo()
        {
            return  ((activeNode == null && commandHistory.Count > 0) ||  (activeNode != null &&  activeNode.Next != null));
        }


        public void RemoveFirstSnapshotFromHistory(){
			var node = commandHistory.First;
			while (node!=null){
				var nextNode = node.Next;
				if (node.Value is SnapshotCommand){
					SnapshotPool.ReleaseFirstSnapshotSlot();
					commandHistory.RemoveFirst();
					return;
				}
				node = nextNode;
			}
			Debug.LogError("didn't remove any snapshots, probably cause an error here");
		}

		public void RemoveAllSnapshotAfterActive(){
			var node = commandHistory.Last;
			while (node != null){
				if (node == activeNode){
					return;
				}
				node = node.Previous;
				commandHistory.RemoveLast();
			}
		}
	}
}
