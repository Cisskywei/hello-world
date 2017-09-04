using UnityEngine;

namespace PaintCraft.Utils{
	public class GOUtil {
        
		public static T AddComponentIfNoExists<T>(GameObject parentObject) where T:Component{
			T result = parentObject.GetComponent<T>();
			if (result == null){
				result = parentObject.AddComponent<T>();
			}
			return result;
		}

		public static T CreateGameObject<T>(string name, GameObject parent, float zOffset) where T:Component{
			return CreateGameObject<T>(name, parent, new Vector3(0.0f,0.0f,zOffset));
        }

		public static T CreateGameObject<T>(string name, GameObject parent) where T:Component{
			return CreateGameObject<T>(name, parent, Vector3.zero);
		}

		public static T CreateGameObject<T>(string name, GameObject parent, Vector3 localPosition) where T:Component{
			GameObject go = new GameObject(name);
			go.layer = parent.layer;
			go.transform.parent = parent.transform;
			ResetLocalPosition(go);
			go.transform.localPosition = localPosition;
			return go.AddComponent<T>();
		}

		public static void ResetLocalPosition(GameObject go){
			ResetLocalPosition(go.transform);
		}

		public static void ResetLocalPosition(Transform t){
			t.localPosition = Vector3.zero;
			t.localScale = Vector3.one;
			t.localRotation = Quaternion.identity;
		}

		public static T CreateComponentIfNoExists<T>(GameObject go) where T:Component{
			T result = go.GetComponent<T>();
			if (result == null){
				result = go.AddComponent<T>();
			}
			return result;
		}

	}
}
