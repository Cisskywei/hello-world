using UnityEngine;
using UnityEditor;
using System.Collections;

public class AboutWindow : EditorWindow {

	string Product = "Radial Menus";
	string Version = "1.0";

	bool AboutFoldout = true;
	bool VersionFoldout = false;

    float pixelWidth;
    float pixelHeight;

    Color headerColor = new Color(0.65f, 0.65f, 0.65f, 1);
    //Color backgroundColor = new Color(0.75f, 0.75f, 0.75f);

	[MenuItem("Tools/Labyrith/Radial Menus/About")]
	public static void Init() {
		EditorWindow.GetWindow(typeof(AboutWindow), false, "About");
	}

	void OnGUI()
	{

        this.minSize = new Vector2(450, 375);

        pixelWidth = position.width - 350;

        if (!EditorGUIUtility.isProSkin)
        {
            headerColor = new Color(165 / 255f, 165 / 255f, 165 / 255f, 1);
            //backgroundColor = new Color(193 / 255f, 193 / 255f, 193 / 255f, 1);
        }
        else
        {
            headerColor = new Color(41 / 255f, 41 / 255f, 41 / 255f, 1);
            //backgroundColor = new Color(56 / 255f, 56 / 255f, 56 / 255f, 1);
        }

        GUILayout.Label("About Radial Menus", EditorStyles.boldLabel);
		//GUILayout.Label ("About", EditorStyles.foldout);

        AboutFoldout = DrawHeaderTitle("About", AboutFoldout, headerColor);
		//AboutFoldout = EditorGUILayout.Foldout(AboutFoldout, "About");
		if (AboutFoldout) 
		{
            GUILayout.Label("Radial Menus - Created By Kyle Briggs,Labyrith and Don Briggs Ltd", EditorStyles.boldLabel);
			GUILayout.Label ("Copyright 2013-2015 - Kyle Briggs, Labyrith and Don Briggs Ltd", EditorStyles.boldLabel);
            GUILayout.Label("Radial Menus ");
			if( GUILayout.Button("labyrith.co.uk"))
				Application.OpenURL("http://www.labyrith.co.uk/");
		}

        VersionFoldout = DrawHeaderTitle("Version", VersionFoldout, headerColor);
		//VersionFoldout = EditorGUILayout.Foldout(VersionFoldout, "Version");
		if (VersionFoldout) 
		{
			GUILayout.Label ("Product Name: " + Product, EditorStyles.boldLabel);
			GUILayout.Label ("Version: " + Version, EditorStyles.boldLabel);
			GUILayout.Label ("Changelog: ", EditorStyles.boldLabel);
			GUILayout.Label ("- Unity 5 UI Support Added");
			GUILayout.Label ("- C# Versions Introduced");
			GUILayout.Label ("- Re-optimised Code");
			if( GUILayout.Button("Contact Us"))
                Application.OpenURL("mailto:kyle@kylebriggs.co.uk?subject=Radial Menus");
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