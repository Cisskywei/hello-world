#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpringVr_Summary))]
public class SpringVr_Edit : Editor
{

    #region 数据
    private SerializedProperty      NumberOfSensors;
    private SerializedProperty      PositioningSystem;
    private SerializedProperty      HeadPositionTarget;
    private SerializedProperty      LeftHandPositionTarget;
    private SerializedProperty      RightHandPositionTarget;
    private SerializedProperty      reciveDataWay;
    private GUIContent              chineseManul;
    public  Texture2D               m_texIconTexture;
    public  Texture2D               m_texBigIconTexture;
    private Texture2D               m_texError01Texture;
    private Texture2D               m_texError02Texture;
    private GUIStyle                m_guiFoldoutStyle;
    private GUIStyle                m_guiLineStyle;
    private Vector2                 m_v2ScrolPos;
    private string                  m_strVersionNumber;
    private bool                    m_bolInfoSectionExpanded;
    #endregion

    private static SpringVr_Edit instance;
    public static SpringVr_Edit Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SpringVr_Edit)ScriptableObject.CreateInstance(typeof(SpringVr_Edit));
            }
            return instance;
        }
    }

    #region 函数

    #region 初始化

    private void Awake()
    {
        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
        DataInit();
        ResourcesLoad();
        FindSerializedProperties();
    }
    private void DataInit()
    {
        m_bolInfoSectionExpanded = true;
        m_strVersionNumber = "2.2.3";
    }
    private void ResourcesLoad()
    {
        m_texIconTexture    = Resources.Load("Texture/Logo") as Texture2D;
        m_texBigIconTexture = Resources.Load("Texture/weixin") as Texture2D;
        m_texError01Texture = Resources.Load("Texture/Error01") as Texture2D;
        m_texError02Texture = Resources.Load("Texture/Error02") as Texture2D;
    }
    private void FindSerializedProperties()
    {
        NumberOfSensors = serializedObject.FindProperty("numberOfSensors");
        PositioningSystem = serializedObject.FindProperty("positioningSystem");
        HeadPositionTarget = serializedObject.FindProperty("HeadTracker");
        RightHandPositionTarget = serializedObject.FindProperty("RightHandTracker");
        LeftHandPositionTarget = serializedObject.FindProperty("LeftHandTracker");
        reciveDataWay = serializedObject.FindProperty("reciveDataWay");
        chineseManul = new GUIContent();
    }
    #endregion

    #region OnInspectorGUI
    public override void OnInspectorGUI()
    {
        FindSerializedProperties();
        PositionCtrlSystemTransform();
        InitializeGUIStyles();
        DreawInstructions();
    }
    private void PositionCtrlSystemTransform()
    {
        chineseManul.text = "请选择数据接收模式";
        EditorGUILayout.PropertyField(reciveDataWay, chineseManul);
        switch (SpringVr_Summary.Instance.reciveDataWay)
        {
            case SpringVr_Summary.ReceiveDataWay.WindowsSerialPort:
#if UNITY_ANDROID
                EditorGUILayout.LabelField("请将BuildingSetting改为Windos模式！");
#endif
                break;
            case SpringVr_Summary.ReceiveDataWay.WindowsBLE:
#if UNITY_ANDROID
                EditorGUILayout.LabelField("请将BuildingSetting改为Windos模式！");
#endif
                EditorGUILayout.LabelField("BLE蓝牙接收只支持Win8/10！");
                break;
            case SpringVr_Summary.ReceiveDataWay.AndroidBLE:
#if UNITY_STANDALONE_WIN
                EditorGUILayout.LabelField("请将BuildingSetting改为Android模式！");
#endif
                break;
            default:
                break;
        }
        chineseManul.text = "请选择手套型号";
        EditorGUILayout.PropertyField(NumberOfSensors, chineseManul);
        chineseManul.text = "请选择定位模式";
        EditorGUILayout.PropertyField(PositioningSystem, chineseManul);
        switch (SpringVr_Summary.Instance.positioningSystem)
        {
            case SpringVr_HandPositionCtrl.PositioningSystem.ViveTracker:
            case SpringVr_HandPositionCtrl.PositioningSystem.OtherTracker:
                chineseManul.text = "将表示右手位置的物体拖入";
                EditorGUILayout.PropertyField(RightHandPositionTarget, chineseManul);
                EditorGUILayout.Space();
                chineseManul.text = "将表示左手位置的物体拖入";
                EditorGUILayout.PropertyField(LeftHandPositionTarget, chineseManul);
                EditorGUILayout.Space();
                break;
            case SpringVr_HandPositionCtrl.PositioningSystem.HeadPositioning:
                chineseManul.text = "将表示眼位置的物体拖入";
                EditorGUILayout.PropertyField(HeadPositionTarget, chineseManul);
                EditorGUILayout.Space();
                break;
            default:
                break;
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void GUIReceiveDataWay()
    {
        
    }
    public void DreawInstructions()
    {
        m_bolInfoSectionExpanded = DrawSectionHeader("开发必读", m_texIconTexture, m_bolInfoSectionExpanded);
        if (m_bolInfoSectionExpanded)
        {
            GUIContent2();
        }
    }
    public void GUIContent2()
    {
        EditorGUILayout.BeginVertical();
        m_v2ScrolPos = EditorGUILayout.BeginScrollView(m_v2ScrolPos,false,true);

        EditorGUILayout.LabelField("版本号 ：" + m_strVersionNumber);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("启动运行说明：");
        EditorGUILayout.LabelField("1、将HUB连接到电脑上，按照安装驱动说明安装驱动。 ");
        EditorGUILayout.LabelField("2、选择手套型号，根据手套上写的型号选择相应型号。");
        EditorGUILayout.LabelField("3、选择相应定位系统。");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("开发说明：");
        EditorGUILayout.LabelField("所有事件等命令均在“HandPlugins/开发必读说明文档”中,或点击下方按钮");
        var rect = GUILayoutUtility.GetRect(Screen.width / 2, 20, GUI.skin.box);
        if (GUI.Button(rect, "阅读开发说明"))
        {
            Application.OpenURL("http://www.spring-vr.com/API/UnityAPI/MiigloveManaul" + m_strVersionNumber + ".pdf");
        }
        EditorGUILayout.LabelField("如果遇到不明白的问题可以到官网的论坛中发帖求助");
        EditorGUILayout.LabelField("工作日每天11：00——12：00，17：00——18：00统一回复答疑");
        rect = GUILayoutUtility.GetRect(Screen.width / 2, 20, GUI.skin.box);
        if (GUI.Button(rect, "寻求支持"))
        {
            Application.OpenURL("http://www.spring-vr.com/forum.php?mod=forumdisplay&fid=2");
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("运行报错说明：");
        EditorGUILayout.LabelField("1、打包请一定打包64位的包，不然会读不到数据，本插件不支持32位系统！");
        EditorGUILayout.LabelField("2、如果运行360相关软件会报以下错误，退出360即可！");
        rect = GUILayoutUtility.GetRect(m_texError01Texture.width * 0.8f, m_texError01Texture.height * 0.8f, GUI.skin.box);
        GUI.DrawTexture(rect, m_texError01Texture);
        EditorGUILayout.LabelField("3、打包前需将API改为.NET2.0，不然打出来的工程会报以下错误");
        rect = GUILayoutUtility.GetRect(m_texError02Texture.width * 0.7f, m_texError02Texture.height * 0.7f, GUI.skin.box);
        GUI.DrawTexture(rect, m_texError02Texture);
        EditorGUILayout.Space();
        rect = GUILayoutUtility.GetRect(Screen.width / 2, 20, GUI.skin.box);
        if (GUI.Button(rect, "访问官网【微信关注，获取开发支持】"))
        {
            Application.OpenURL("http://www.spring-vr.com/portal.php");
        }
        rect = GUILayoutUtility.GetRect(Screen.width / 2, Screen.width / 2, GUI.skin.box);
        GUI.DrawTexture(rect, m_texBigIconTexture);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

    }

            #region OnInspectorGUI工具
    private void InitializeGUIStyles()
    {
        if (m_guiFoldoutStyle == null)
        {
            m_guiFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            m_guiFoldoutStyle.fontStyle = FontStyle.Bold;
            m_guiFoldoutStyle.fixedWidth = 2000.0f;
        }

        if (m_guiLineStyle == null)
        {
            m_guiLineStyle = new GUIStyle(GUI.skin.box);
            m_guiLineStyle.border.top = 1;
            m_guiLineStyle.border.bottom = 1;
            m_guiLineStyle.margin.top = 1;
            m_guiLineStyle.margin.bottom = 1;
            m_guiLineStyle.padding.top = 1;
            m_guiLineStyle.padding.bottom = 1;
        }
    }
    public bool DrawSectionHeader(string name, Texture2D icon, bool isExpanded)
    {
        GUILayout.Box(GUIContent.none, m_guiLineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1.0f));

        Rect position = GUILayoutUtility.GetRect(40.0f, 2000.0f, 16.0f, 16.0f, m_guiFoldoutStyle);
        isExpanded = EditorGUI.Foldout(position, isExpanded, new GUIContent(" " + name, icon), true, m_guiFoldoutStyle);

        return isExpanded;
    }
            #endregion

#endregion

#endregion
}
#endif