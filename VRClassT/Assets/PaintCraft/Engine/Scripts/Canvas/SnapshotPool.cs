using UnityEngine;
using PaintCraft.Utils;
using PaintCraft.Controllers;

namespace PaintCraft.Canvas{

	public class SnapshotPool : MonoBehaviour {
		RenderTexture cameraTargetTexture;
		Texture2D snapshot;
		SnapshotData[] snapshotHistory; //temp value
		int historySize;

		/// <summary>
		/// The size of the undo back.
		/// How many times we can undo from current point
		/// </summary>
		public int UndoBackSize {get; private set;}

		/// <summary>
		/// The size of the redo ahead.
		/// How many times we can redo from current point
		/// </summary>
		public int RedoAheadSize {get; private set;}


		/// <summary>
		/// The current snapshot loop identifier.
		/// this is id in historySnapshot array (increment within loop)
		/// </summary>
		int snapshotOrderId;


		int maxSnapshotId;

        public void Init(int historySize, Vector2 renderTextureSize){
            this.historySize = historySize;
            ResetStatus(renderTextureSize);
			snapshotHistory = new SnapshotData[historySize];						
			for (int i = 0; i < historySize; i++) {
				snapshotHistory[i] = new SnapshotData(){
                    RenderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 0, RenderTextureFormat.ARGB32)
				};	
			}           
		}

        public void ResetStatus(Vector2 renderTextureSize){
			maxSnapshotId = historySize - 1;
			snapshotOrderId = -1;
			UndoBackSize = 0;
			RedoAheadSize = 0;
            cameraTargetTexture = GetComponent<Camera>().targetTexture;
            // following code won't be execute first time as snapshothistory will be create later in the method above
            if (snapshotHistory != null){
                for (int i = 0; i < historySize; i++) {
                    snapshotHistory[i].RenderTexture = TextureUtil.UpdateRenderTextureSize(snapshotHistory[i].RenderTexture, renderTextureSize.x, renderTextureSize.y);
                }  
            }
		}



		/// <summary>
		/// Determines whether this instance has current snapshot data.
		/// In case of first step after initialization we won't have current state
		/// </summary>
		/// <returns><c>true</c> if this instance has current snapshot data; otherwise, <c>false</c>.</returns>
		public bool HasCurrentSnapshotData(){
			return snapshotOrderId > -1;
		}

		public SnapshotData GetCurrentSnapshotData(){
			if (HasCurrentSnapshotData()){
				return snapshotHistory[snapshotOrderId];
			}
			return null;
		}


		/// <summary>
		/// Makes the snapshot.
		/// </summary>
		/// <returns>The snapshot id.</returns>
		public SnapshotData MakeSnapshot(){
			if (HasFreeSnapshotSlot()){			
				IncrementLoopId();

                snapshotHistory[snapshotOrderId].RenderTexture.DiscardContents();
                Graphics.Blit(cameraTargetTexture, snapshotHistory[snapshotOrderId].RenderTexture);

				UndoBackSize++;
				if (HasRedo()){
					RedoAheadSize = 0;
				}
				return snapshotHistory[snapshotOrderId];
			} else {
				Debug.LogError("Don't have free snapshot slot, please free one manually");
				return null;
			}
		}

		public bool HasFreeSnapshotSlot(){
			return snapshotOrderId != maxSnapshotId;
		}

		public void ReleaseFirstSnapshotSlot(){
			maxSnapshotId = MathUtil.IncrementIntLoop( maxSnapshotId, 0, historySize);
			UndoBackSize--;
		}
            	
		public bool HasUndo(){
			return UndoBackSize > 0;
		}

		public bool HasRedo(){
			return RedoAheadSize > 0;
		}
		
		public void Undo(SnapshotData snapshotData){
			if (!HasUndo()){
				Debug.LogError("don't have undo snapshots");
				return;
			}
			UndoBackSize--;
			RedoAheadSize++;
			//just double check that we passed correct data
			DecrementLoopId();
			if (snapshotData != snapshotHistory[snapshotOrderId]){
				Debug.LogError("wrong snapshot data here");
			}
			Graphics.Blit(snapshotData.RenderTexture, cameraTargetTexture);
		}

		public void Redo(SnapshotData snapshotData){
			if (!HasRedo()){
				Debug.LogError("don't have redo snapshots");
				return;
			}
			UndoBackSize++;
			RedoAheadSize--;
			IncrementLoopId();
			//just double check that we passed correct data
			if (snapshotData != snapshotHistory[snapshotOrderId]){
				Debug.LogError("wrong snapshot data here");
			}
			Graphics.Blit(snapshotHistory[snapshotOrderId].RenderTexture, cameraTargetTexture);
		}


		void IncrementLoopId(){
			snapshotOrderId = MathUtil.IncrementIntLoop(snapshotOrderId, 0, historySize);
		}

		void DecrementLoopId(){
			snapshotOrderId = MathUtil.DecrementIntLoop(snapshotOrderId, 0, historySize);
		}

	}
}