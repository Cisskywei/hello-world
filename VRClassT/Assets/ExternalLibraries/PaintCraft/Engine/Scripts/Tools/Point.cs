using UnityEngine;

namespace PaintCraft.Tools{
	public class Point {
		public Vector2 	Position;	
		public Vector2  Size;
		public float	Velocity;
		/// <summary>
		/// The rotation. in degree
		/// </summary>
		public float	Rotation;
		public float	Scale;
		public float	Time;
		public PointStatus Status;
        public Material Material;


		int		basePointId;
		/// <summary>
		/// The base point identifier.
		/// each point from input is market by id (1,2,3,...,n)
		/// 
		/// if it's not a base point return -1
		/// </summary>
		public int BasePointId {
			get {
				if (IsBasePoint){
					return basePointId;
				} else {
					return -1;
				}
			}
			set {
				basePointId = value;
			}
		}

		/// <summary>
		/// The is base point.
		/// <value>true</value> if comes from input
		/// <value>false</value> if artificial point created by filter
		/// </summary>
		public bool		IsBasePoint;


		/// <summary>
		/// Gets a value indicating whether this instance is first base point from input.
		/// </summary>
		/// <value><c>true</c> if this instance is first base point from input; otherwise, <c>false</c>.</value>
		public bool IsFirstBasePointFromInput{
			get{
				return (BasePointId == 0);
			}
		}

		public PointColor PointColor = PointColor.White;

		public void Reset(){
			Position.x 	= 0.0f;
            Position.y  = 0.0f;
            Velocity    = 0.0f;
			Rotation 	= 0.0f;
			Time 		= 0.0f;
			Scale 		= 1.0f;
			Size.x 		= 0.0f;
		    Size.y      = 0.0f;
			Status 		= PointStatus.NotSet;
            basePointId = 0;
            IsBasePoint = false;
            Material = null;
            PointColor =  PointColor.White;
		}

		public void CopyFrom(Point anotherPoint){
			Position    = anotherPoint.Position;
			CopyAllExceptPosition(anotherPoint);
		}

		public void CopyAllExceptPosition(Point anotherPoint){
			Velocity    = anotherPoint.Velocity;
			Rotation    = anotherPoint.Rotation;
			Status      = anotherPoint.Status;
			Scale       = anotherPoint.Scale;
			IsBasePoint = anotherPoint.IsBasePoint;
			basePointId = anotherPoint.basePointId;
			Time        = anotherPoint.Time;
			Size 		= anotherPoint.Size;
            Material    = anotherPoint.Material;
			PointColor.CopyFrom (anotherPoint.PointColor);
		}
		public override string ToString ()
		{
			return string.Format ("[Point: Pos={0}, Sz={1}, Vlcty={2}, Rot={3}, Scale={4}, Time={5}, Status={6}, basePointId={7}, IsBasePoint={8}, BasePointId={9}, IsFirstBasePointFromInput={10}]", Position, Size, Velocity, Rotation, Scale, Time, Status, basePointId, IsBasePoint, BasePointId, IsFirstBasePointFromInput);
		}		
	}
}

