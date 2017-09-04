/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace FastOcean
{
    [CustomEditor(typeof(FOceanGrid))]
	[ExecuteInEditMode]
	public class FOceanGridEditor : Editor
	{
        SerializedProperty baseParam;
        SerializedProperty gwParam;
        SerializedProperty reflParam;
        
        SerializedProperty oceanMaterial;

        SerializedProperty renderEnabled;

        SerializedProperty projectedMesh;
        SerializedProperty usedGridSize;
        SerializedProperty oceanHeight;
        SerializedProperty boundPos;
        SerializedProperty boundRot;
        SerializedProperty boundSize;
        SerializedProperty minBias;
        
        SerializedProperty gWMode;
        SerializedProperty gwDir;
        SerializedProperty gwFlow;
        SerializedProperty gwScale;
        SerializedProperty gWamplitude;
        SerializedProperty gWaveLength;
        SerializedProperty gWChoppiness;
        SerializedProperty gWSpeed;
        SerializedProperty gWTime;
        
        SerializedProperty quality;
        SerializedProperty reflectionMask;
        SerializedProperty clipPlaneOffset;
        SerializedProperty blurEnabled;
        SerializedProperty blurSpread;

        void OnEnable()
        {
            baseParam = serializedObject.FindProperty("baseParam");
            gwParam = serializedObject.FindProperty("gwParam");
            reflParam = serializedObject.FindProperty("reflParam");

            oceanMaterial = serializedObject.FindProperty("oceanMaterial");
            
            renderEnabled = serializedObject.FindProperty("renderEnabled");


            projectedMesh = baseParam.FindPropertyRelative("projectedMesh");
            usedGridSize = baseParam.FindPropertyRelative("usedGridSize");
            oceanHeight = baseParam.FindPropertyRelative("oceanHeight");
            
            boundSize = baseParam.FindPropertyRelative("boundSize");
            boundPos = baseParam.FindPropertyRelative("boundPos");
            boundRot = baseParam.FindPropertyRelative("boundRotate");
            minBias = baseParam.FindPropertyRelative("minBias");

            gWMode = gwParam.FindPropertyRelative("mode");

            gwDir = gwParam.FindPropertyRelative("gwDirection");
            gwFlow = gwParam.FindPropertyRelative("gwFlow");
            gwScale = gwParam.FindPropertyRelative("gwScale");

            gWamplitude = gwParam.FindPropertyRelative("gWamplitude");
            gWaveLength = gwParam.FindPropertyRelative("gWaveLength");

            gWChoppiness = gwParam.FindPropertyRelative("gWChoppiness");
            gWSpeed = gwParam.FindPropertyRelative("gWSpeed");
            gWTime = gwParam.FindPropertyRelative("gWTime");
            
            quality = reflParam.FindPropertyRelative("quality");
            reflectionMask = reflParam.FindPropertyRelative("reflectionMask");
            clipPlaneOffset = reflParam.FindPropertyRelative("clipPlaneOffset");

            blurEnabled = reflParam.FindPropertyRelative("blurEnabled");
            blurSpread = reflParam.FindPropertyRelative("blurSpread");
        }

        void LayoutSurParam(FOceanGrid pg)
        {
            EditorGUILayout.PropertyField(baseParam);

            if (baseParam.isExpanded)
            {
                EditorGUI.indentLevel = 1;

                usedGridSize.intValue = EditorGUILayout.IntSlider("Used Grid Size", pg.baseParam.usedGridSize, 32, 254);

                projectedMesh.boolValue = EditorGUILayout.Toggle("Projected Mesh", pg.baseParam.projectedMesh);
                EditorGUI.indentLevel = 2;
                if (!projectedMesh.boolValue)
                {
                    boundPos.vector3Value = EditorGUILayout.Vector3Field("Bound Pos", pg.baseParam.boundPos);
                    boundRot.floatValue = EditorGUILayout.Slider("Bound Rot", pg.baseParam.boundRotate, 0 ,360);
                    boundSize.vector3Value = EditorGUILayout.Vector3Field("Bound Size", pg.baseParam.boundSize);
                }
                else
                {
                    oceanHeight.floatValue = EditorGUILayout.FloatField("Ocean Height ", pg.baseParam.oceanHeight);
                    minBias.vector4Value = EditorGUILayout.Vector4Field("Min Bias & Lod", pg.baseParam.minBias);
                }
                EditorGUI.indentLevel = 1;

                EditorGUI.indentLevel = 0;
            }
        }

        static bool gwParamOpen = true;
        void LayoutGWParam(FOceanGrid pg)
        {
            gwParamOpen = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), gwParamOpen, "Wave Param", true);
            if (gwParamOpen)
            {
                EditorGUI.indentLevel = 1;

                eFGWMode eMode = (eFGWMode)EditorGUILayout.EnumPopup("Mode", pg.gwParam.mode);
                gWMode.enumValueIndex = (int)eMode; //bit to index

                if (eMode != eFGWMode.eGWM_None)
                {
                    EditorGUILayout.Separator();

                    Color c = GUI.color;
                    
                    gwScale.floatValue = EditorGUILayout.Slider("Scale", pg.gwParam.gwScale, 0.5f, 2f);
                    gWChoppiness.floatValue = EditorGUILayout.Slider("Choppiness", pg.gwParam.gWChoppiness, 0f, 5f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.cyan;
                    gwDir.floatValue = EditorGUILayout.Slider("Direction", pg.gwParam.gwDirection, 0f, 360f);
                    gwFlow.floatValue = EditorGUILayout.Slider("Flow", pg.gwParam.gwFlow, 0f, 1f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.white;
                    gWSpeed.floatValue = EditorGUILayout.Slider("Speed", pg.gwParam.gWSpeed, 0f, 2f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.red;
                    float gax = EditorGUILayout.Slider("Wave AmpX", pg.gwParam.gWamplitude.x, 0f, 0.1f);
                    float glx = EditorGUILayout.Slider("Wave LenX", pg.gwParam.gWaveLength.x, 0.5f, 1f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.green;
                    float gay = EditorGUILayout.Slider("Wave AmpY", pg.gwParam.gWamplitude.y, 0f, 0.1f);
                    float gly = EditorGUILayout.Slider("Wave LenY", pg.gwParam.gWaveLength.y, 0.5f, 1f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.yellow;
                    float gaz = EditorGUILayout.Slider("Wave AmpZ", pg.gwParam.gWamplitude.z, 0f, 0.1f);
                    float glz = EditorGUILayout.Slider("Wave LenZ", pg.gwParam.gWaveLength.z, 0.5f, 1f);
                    EditorGUILayout.Separator();
                    GUI.color = Color.gray;
                    float gaw = EditorGUILayout.Slider("Wave AmpW", pg.gwParam.gWamplitude.w, 0f, 0.1f);
                    float glw = EditorGUILayout.Slider("Wave LenW", pg.gwParam.gWaveLength.w, 0.5f, 1f);
                    EditorGUILayout.Separator();
                    GUI.color = c;
                    gWamplitude.vector4Value = new Vector4(gax, gay, gaz, gaw);
                    gWaveLength.vector4Value = new Vector4(glx, gly, glz, glw);

                    gWTime.doubleValue = EditorGUILayout.DoubleField("Elapsed Time", pg.gwParam.gWTime);

                }
                EditorGUI.indentLevel = 0;
            }
        }

        static bool reflParamOpen = true;
        void LayoutReflParam(FOceanGrid pg)
        {
            reflParamOpen = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), reflParamOpen, "Reflection Param", true);
            if (reflParamOpen)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(quality);
                EditorGUILayout.PropertyField(reflectionMask);
                EditorGUILayout.PropertyField(clipPlaneOffset);
                EditorGUILayout.PropertyField(blurEnabled);
                if(blurEnabled.boolValue)
                    EditorGUILayout.PropertyField(blurSpread);

                EditorGUI.indentLevel = 0;
            }
        }

        public override void OnInspectorGUI()
	    {
	        serializedObject.Update();

            FOceanGrid pg = (FOceanGrid)target;

            if(pg == null)
                return;

           int tmp = EditorGUI.indentLevel;
            
            LayoutSurParam(pg);
            LayoutGWParam(pg);
            LayoutReflParam(pg);

            EditorGUILayout.PropertyField(oceanMaterial);

            EditorGUILayout.PropertyField(renderEnabled);

            serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = tmp;
		}
	}
}
