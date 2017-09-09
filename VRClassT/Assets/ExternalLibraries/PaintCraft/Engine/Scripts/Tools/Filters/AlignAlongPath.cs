using UnityEngine;
using System.Collections.Generic;
using NodeInspector;


namespace PaintCraft.Tools.Filters{
    [NodeMenuItem("Align/AlignAlongPath")]
    public class AlignAlongPath : FilterWithNextNode {        
        /// <summary>
		/// The angle add. in degree
		/// </summary>
		public float AngleAdd = 0.0f;

		#region implemented abstract members of FilterWithNextNode

        public override bool FilterBody (BrushContext brushLineContext)
		{
			LinkedListNode<Point> previousNode = null;
			LinkedListNode<Point> node = brushLineContext.Points.Last;
			Vector2 delta;
			float rotation;
			bool setLastPosition = true;

			while (node != null && node.Value.Status != PointStatus.CopiedToCanvas){

				if (!node.Value.IsBasePoint){
					// we need to count only not based points. because they could located at the same position as normal points
					if (previousNode != null){
						delta =  previousNode.Value.Position - node.Value.Position;
						rotation = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg + AngleAdd;
						node.Value.Rotation = rotation;
						if (setLastPosition){
							previousNode.Value.Rotation = rotation;
							setLastPosition = false;
						}
					} 
					previousNode = node;
				}

				node = node.Previous;
			}
            return true;
		}
		#endregion
	}
}