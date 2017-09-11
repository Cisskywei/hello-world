using UnityEngine;
using UnityEditor;

public class MenuCreation : EditorWindow {

    float pixelWidth;
    float pixelHeight;

    string couponCode = "jT16DZ9wQs";

    bool onlineFoldout = false;
    bool documentationFoldout = false;
    bool accessFoldout = true;

    Color headerColor = new Color(0.65f, 0.65f, 0.65f, 1);

    [MenuItem("Tools/Labyrith/Radial Menus/Documentation")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(MenuCreation), false, "Creation Instructions");
    }

	void OnGUI()
    {
        GUI.skin.label.wordWrap = true;

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
        GUILayout.Label("Radial Menus Creation Instructions", EditorStyles.boldLabel);

        onlineFoldout = DrawHeaderTitle("Online Angles & Calculator", onlineFoldout, headerColor);
        if (onlineFoldout)
        {
            GUILayout.Label("Radial Menus Angles", EditorStyles.boldLabel);
            GUILayout.Label("Here you can find a table of angles for use in creating Radial Menus in Photoshop using the method detailed in the PDF and online videos.");
            GUILayout.Label("If the angles in the table located by clicking the 'Angles' button do not go up to what you would like, you can use the 'Angles Calculator' button to use our online calculator to do so.");
            if (GUILayout.Button("Angles"))
            {
                Application.OpenURL("http://labyrith.co.uk/tools-for-unity/radial-menu#angle");
            }

            if (GUILayout.Button("Angles Calculator"))
            {
                Application.OpenURL("http://labyrith.co.uk/tools-for-unity/radial-menu#angleCalc");
            }
        }

        documentationFoldout = DrawHeaderTitle("Online Documentation", documentationFoldout, headerColor);
        if (documentationFoldout)
        {
             GUILayout.Label("Radial Menus Documentation", EditorStyles.boldLabel);
            GUILayout.Label("Using the button 'Documentation' below, you can open our PDF documentation which details how to use Radial Menus and how to create your own using Adobe Photoshop(Requires a PDF Reader).");
            GUILayout.Label("If you would rather use our online documentation, please click the 'Online Documentation' button, and you will be directed to our online documentation and tutorial videos.");
            
            if (GUILayout.Button("Online Documentation"))
            {
                Application.OpenURL("http://labyrith.co.uk/tools-for-unity/radial-menu/documentation");
            }
        }

        accessFoldout = DrawHeaderTitle("Access", accessFoldout, headerColor);
        if (accessFoldout)
        {
            GUILayout.Label("This is your Access code, this grants your Labyrith account access to the Radial Menu documentation and video pages, without it you will not have access to the documentation areas.");
            GUILayout.Label("To use it, click the 'Copy to Clipboard' button and click on the 'Online Documentation' button in the tab above, click on the product box and click Add to Cart, use the code copied to the clipboard in the Coupons area, make sure it has been enabled by checking the total pay amount is at 0, otherwise you risk paying for Radial Menus twice.");

            GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
            GUI.skin.textField.fontSize = 20;
            GUI.skin.textField.fontStyle = FontStyle.Normal;
            GUILayout.TextField(couponCode);

            GUI.color = Color.green;

            if (GUILayout.Button("Copy to Clipboard", GUILayout.Height(50)))
            {
                EditorGUIUtility.systemCopyBuffer = couponCode;
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
