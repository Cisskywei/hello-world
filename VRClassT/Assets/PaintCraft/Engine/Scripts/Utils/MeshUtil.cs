using UnityEngine;

namespace PaintCraft.Utils {
    public class MeshUtil {

        static Vector2[] _uv2 = new Vector2[6];
		/// <summary>
		/// Updates the mesh U v2.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="swatchWidth">Swatch width.</param>
		/// <param name="swatchHeight">Swatch height.</param>
		/// <param name="swatchPosition">Swatch position.</param>
		/// <param name="rotation">Rotation. (degree)</param>
		/// <param name="canvasWidth">Canvas width.</param>
		/// <param name="canvasHeight">Canvas height.</param>
        /// <param name = "canvasPosition">Canvas global position.</param>
		public static void UpdateMeshUV2(Mesh mesh, float swatchWidth, float swatchHeight,
            Vector2 swatchPosition, float rotation, float canvasWidth, float canvasHeight, Vector2 canvasPosition){            
			float halfWidth = swatchWidth / 2.0f;
			float halfHeight = swatchHeight /2.0f;

			float cosAlfa = Mathf.Cos (rotation * Mathf.Deg2Rad);
			float sinAlfa = Mathf.Sin (rotation * Mathf.Deg2Rad);

			SetCoords (ref _uv2 [0], swatchPosition, -halfWidth, -halfHeight, sinAlfa, cosAlfa);
			SetCoords (ref _uv2 [1], swatchPosition, -halfWidth,  halfHeight, sinAlfa, cosAlfa);
			_uv2 [5].x = _uv2 [1].x;
			_uv2 [5].y = _uv2 [1].y;
			SetCoords (ref _uv2 [2], swatchPosition,  halfWidth, -halfHeight, sinAlfa, cosAlfa);
			_uv2 [3].x = _uv2 [2].x;
			_uv2 [3].y = _uv2 [2].y;
			SetCoords (ref _uv2 [4], swatchPosition,  halfWidth,  halfHeight, sinAlfa, cosAlfa);

			for (int i = 0; i < _uv2.Length; i++) {
                _uv2[i].x = (_uv2[i].x - canvasPosition.x) / canvasWidth + 0.5f;
                _uv2[i].y = (_uv2[i].y - canvasPosition.y) / canvasHeight + 0.5f;
			}
			mesh.uv2 = _uv2;           
		}

	    
	    /// <summary>
	    /// Store original click position in every vertex
	    /// </summary>
	    /// <param name="mesh"></param>
	    /// <param name="uv"></param>
	    public static void UpdateMeshUV3(Mesh mesh, Vector2 uv)
	    {
		    for (int i = 0; i < _uv2.Length; i++)
		    {
			    _uv2[i] = uv;
		    }
		    mesh.uv4 = _uv2;
	    }

			                          
		static void SetCoords(ref Vector2 vec, Vector2 position, float xOffset, float yOffset, float sinAlfa, float cosAlfa){
			vec.x = position.x + (xOffset * cosAlfa - yOffset * sinAlfa);
			vec.y = position.y + (xOffset * sinAlfa + yOffset * cosAlfa);
		}




        public static GameObject CreatePlaneGameObject(float width, float height, string name)
        {
            GameObject go = new GameObject();
            go.AddComponent(typeof(MeshRenderer));
            MeshFilter meshFilter = (MeshFilter)go.AddComponent(typeof(MeshFilter));

            Mesh mesh = CreatePlaneMesh(width, height);
            if (Application.isPlaying)
                meshFilter.mesh = mesh;
            else
                meshFilter.sharedMesh = mesh;



            return go;
        }

       

        public static Mesh CreatePlaneMesh(Vector2 size)
        {
            return CreatePlaneMesh(size.x, size.y);
        }

		static Color[] _colors = new Color[6];
		public static void ChangeMeshColor(Mesh mesh, Color color){
			SetRectVertexColors(_colors, color);
			mesh.colors = _colors;
		}


		static Vector3[] _vertices = new Vector3[6];
		public static void ChangeMeshSize(Mesh mesh, float width, float height){
			SetRectVertexPositions(_vertices, width, height);
			mesh.vertices = _vertices;
		}

        public static Mesh CreatePlaneMesh(float width, float height)
        {
            Mesh mesh = new Mesh();
            
            Vector3[] vertices = new Vector3[6];
			Color[] colors = new Color[6];
            int[] triangles = new int[6] { 0, 1, 2, 3, 5, 4 };
            Vector2[] uvs = new Vector2[6]{
                new Vector2 (0, 0),
                new Vector2 (0, 1),
                new Vector2 (1, 0),
                new Vector2 (1, 0),
                new Vector2 (1, 1),
                new Vector2 (0, 1)			
            };
            
			SetRectVertexPositions(vertices, width, height);
			SetRectVertexColors(colors, Color.white);
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
			mesh.colors = colors;
            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();
            return mesh;
        }


		static Vector3 _point;
		static void SetRectVertexPositions(Vector3[] vertices, float width, float height){
			_point = Vector3.zero;

			_point.z = 0.0f;
			_point.x -= width / 2;
			_point.y -= height / 2;
			vertices[0] = _point;
			
			_point.y += height;
			vertices[1] = _point;
			
			_point.x += width;
			_point.y -= height;
			vertices[2] = _point;
			vertices[3] = _point;
			_point.y += height;
			vertices[4] = _point;
			_point.x -= width;
			vertices[5] = _point;
		}

		static void SetRectVertexColors(Color[] colors, Color color){
			for (int i = 0; i < colors.Length; i++) {
				colors[i] = color;
			}
		}

    }
}
