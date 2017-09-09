using System;
using UnityEngine;
using UnityEditor;
using PaintCraft.Utils;
using System.IO;
using System.Linq;


namespace PaintCraft.Editor{
	[CustomPropertyDrawer(typeof(TexturePathAttribute))]
	public class TexturePathPropertyDrawer : PropertyDrawer {

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
					
			if (property.propertyType == SerializedPropertyType.String){
				string path = property.stringValue;
				Texture2D tex = null;
				bool showRed = false;

				if (string.IsNullOrEmpty(path)){
					tex = null;
				} else if (path.StartsWith("Assets")){
					tex = AssetDatabase.LoadAssetAtPath<Texture2D> (path);
					showRed = true;
				} else {                    
				    string name = Path.GetFileNameWithoutExtension(path);
                    string[] assets = AssetDatabase.FindAssets(name);
                    if (assets.Length == 0){
                        showRed = true;
                    } else {                        
                        string asset = assets.ToList().First(s => AssetDatabase.GUIDToAssetPath(s).Contains(path));
                        if (string.IsNullOrEmpty(asset)){
                            Debug.LogError("can't find asset path for "+name);
                            showRed = true;
                        } else {
                            string fullPath = AssetDatabase.GUIDToAssetPath(asset);
                            tex = AssetDatabase.LoadAssetAtPath<Texture2D>( fullPath);                                            
                        }
                    }
				}

				if (showRed){
					GUI.color = Color.red;
				}
				Texture2D newTex = EditorGUI.ObjectField(position, label.text,  tex, typeof(Texture2D), false) as Texture2D;
				GUI.color = Color.white;

				if (newTex != tex){
					if (newTex == null){
						property.stringValue = "";
					} else {
						string assetPath = AssetDatabase.GetAssetPath(newTex);

						if (!assetPath.Contains("/Resources/") && !assetPath.Contains("\\Resources\\")){
							Debug.LogErrorFormat("Texture {0} must beLocated at {1} folder, " +
								"otherwise it would to much memory on device", newTex, "Resources");
							property.stringValue = assetPath;
						}
                        else
						{
						    string resourcePath = assetPath.Substring(assetPath.IndexOf("Resources", StringComparison.Ordinal) + "Resources".Length + 1);
                            string dirName = Path.GetDirectoryName(resourcePath);
                            string finalResourcePath;
                            if (dirName == ""){
                                finalResourcePath = Path.GetFileNameWithoutExtension(resourcePath);                                
                            } else {
                                finalResourcePath = Path.GetDirectoryName(resourcePath) + "/" + Path.GetFileNameWithoutExtension(resourcePath);                            
                            }

						    property.stringValue = finalResourcePath;
						}
                    }
				}

			} else {
				Debug.LogErrorFormat("property {0} must have string type", property.ToString());
				EditorGUI.PropertyField(position, property);
			}
		}
	}
}
