using UnityEngine;
using UnityEditor;
using System.Collections;

public enum BookObject { Two_Slice, Three_Slice, Four_Slice, Five_Slice, Six_Slice, Seven_Slice, Eight_Slice, Nine_Slice, Ten_Slice };

public class CreatorWindow : EditorWindow {

    string Product = "Radial Menus";
	string Version = "1.0";

	private BookObject m_objectType = BookObject.Two_Slice;

    float pixelWidth;
    float pixelHeight;

	bool ToolsFoldout = true;
	bool UsageFoldout = false;

    Color headerColor = new Color(0.65f, 0.65f, 0.65f, 1);

    [MenuItem("Tools/Labyrith/Radial Menus/Creator")]
	public static void Init() {
		EditorWindow.GetWindow(typeof(CreatorWindow), false, "SD Creator");
	}

	void OnGUI()
	{

        this.minSize = new Vector2(250, 410);

        pixelWidth = position.width - 350;

        if (!EditorGUIUtility.isProSkin)
        {
            headerColor = new Color(165 / 255f, 165 / 255f, 165 / 255f, 1);
        }
        else
        {
            headerColor = new Color(41 / 255f, 41 / 255f, 41 / 255f, 1);
        }
        GUILayout.Label("Radial Menus Creator", EditorStyles.boldLabel);

        ToolsFoldout = DrawHeaderTitle("Tools", ToolsFoldout, headerColor);
		if (ToolsFoldout) 
		{
            GUILayout.Label("Radial Menus Object Creator", EditorStyles.boldLabel);
			m_objectType = (BookObject)EditorGUILayout.EnumPopup("", m_objectType );
			if( GUILayout.Button("Create RM Object"))
			{
                if (m_objectType == BookObject.Two_Slice)
				{
					GameObject TwoSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/2_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(TwoSlice);
				}

                if (m_objectType == BookObject.Three_Slice)
				{
                    GameObject ThreeSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/3_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(ThreeSlice);
				}

                if (m_objectType == BookObject.Four_Slice)
				{
                    GameObject FourSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/4_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(FourSlice);
				}

                if (m_objectType == BookObject.Five_Slice)
				{
                    GameObject FiveSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/5_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(FiveSlice);
				}

                if (m_objectType == BookObject.Six_Slice)
                {
                    GameObject SixSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/6_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(SixSlice);
                }

                if (m_objectType == BookObject.Seven_Slice)
                {
                    GameObject SevenSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/7_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(SevenSlice);
                }

                if (m_objectType == BookObject.Eight_Slice)
                {
                    GameObject EightSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/8_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(EightSlice);
                }

                if (m_objectType == BookObject.Nine_Slice)
                {
                    GameObject NineSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/9_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(NineSlice);
                }

                if (m_objectType == BookObject.Ten_Slice)
                {
                    GameObject TenSlice = AssetDatabase.LoadAssetAtPath("Assets/Radial Menu/Prefabs/10_slice.prefab", typeof(GameObject)) as GameObject;
                    Instantiate(TenSlice);
                }
			}
		}

        UsageFoldout = DrawHeaderTitle("Usage", UsageFoldout, headerColor);
		if (UsageFoldout) 
		{
			GUILayout.Label ("Product Name: " + Product, EditorStyles.boldLabel);
			GUILayout.Label ("Version: " + Version, EditorStyles.boldLabel);
			GUILayout.Label ("Usage: ", EditorStyles.boldLabel);
			GUILayout.Label ("In the foldout above named Tools");
			GUILayout.Label ("choose a method from the dropdown list");
			GUILayout.Label ("and click 'Create RM Object' to create");
			GUILayout.Label ("a radial menu with those slices.");
            GUILayout.Label("");
            GUILayout.Label("After creating the radial menu,");
            GUILayout.Label("you will need to move it under a");
            GUILayout.Label("Canvas object and change the:");
            GUILayout.Label("   Z position to - 0");
            GUILayout.Label("   X, Y and Z scale to 1.");
			if( GUILayout.Button("About"))
			{
				AboutWindow window = (AboutWindow) EditorWindow.GetWindow( typeof(AboutWindow), false, "About");
				window.Show();
			}
		}

	}

    public bool DrawHeaderTitle(string title, bool foldoutProperty, Color backgroundColor)
    {

        GUILayout.Space(0);

        GUI.Box(new Rect(1, GUILayoutUtility.GetLastRect().y + 4, position.width, 27), "");
        EditorGUI.DrawRect(new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y + 5f, position.width + 1, 25f), headerColor);
        GUILayout.Space(4);

        GUILayout.Label(title, EditorStyles.largeLabel);
        GUI.color = Color.clear;
        if (GUI.Button(new Rect(0, GUILayoutUtility.GetLastRect().y - 4, position.width, 27), ""))
        {
            foldoutProperty = !foldoutProperty;
        }
        GUI.color = Color.white;
        return foldoutProperty;
    }


}