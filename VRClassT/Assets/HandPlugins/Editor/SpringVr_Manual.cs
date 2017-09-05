#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[InitializeOnLoad]
public class SpringVr_Manual : EditorWindow
{

    static SpringVr_Manual window;


    static SpringVr_Manual()
    {
        EditorApplication.update += Init;
    }

    static void Init()
    {
        if (PlayerPrefs.GetInt("isBegin") == 0)
        {
            DrayWindow();
            PlayerPrefs.SetInt("isBegin", 1);
        }
        EditorApplication.update -= Init;
    }

    [MenuItem("Window/开发说明")]
    static void DrayWindow()
    {
        window = GetWindow<SpringVr_Manual>(true);
        window.minSize = new Vector2(320, 440);
        window.titleContent = new GUIContent("开发说明(此说明只会在第一次运行时自动显示)");
    }

    void OnGUI()
    {
        SpringVr_Edit.Instance.GUIContent2();
        EditorGUILayout.EndFadeGroup();
    }
}
#endif