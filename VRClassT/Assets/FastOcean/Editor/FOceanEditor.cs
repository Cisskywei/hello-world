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
using System.Collections.Generic;

namespace FastOcean
{
	[CustomEditor(typeof(FOcean))]
	[ExecuteInEditMode]
	public class FOceanEditor : Editor
	{
		static FOceanEditor()
	    {
	        EditorApplication.update += Update;
	    }
        
        SerializedProperty envParam;
        SerializedProperty trailer;
        SerializedProperty tmParam;
        SerializedProperty shaderPack;
        SerializedProperty layerDef;

        SerializedProperty foamEnabled;
        SerializedProperty depthBlendMode;
        SerializedProperty underWaterMode;
        SerializedProperty sunLight;
        SerializedProperty sunIntensity;
        SerializedProperty sunMode;
        SerializedProperty sunOccluMask;

        SerializedProperty renderSpace;
        SerializedProperty trailMaskSize;
        SerializedProperty trailMaskScale;
        SerializedProperty trailMaskFade;
        SerializedProperty trailIntensity;

        SerializedProperty underAmb;
        SerializedProperty skirt;
        SerializedProperty depthFade;
        SerializedProperty surfaceFade;
        SerializedProperty distortMag;
        SerializedProperty distortFrq;
        SerializedProperty underDistortMap;
        SerializedProperty underWaterShader;
        SerializedProperty underButtom;

        void OnEnable()
        {
            envParam = serializedObject.FindProperty("envParam");
            trailer = serializedObject.FindProperty("trailer");
            tmParam = serializedObject.FindProperty("tmParam");
            shaderPack = serializedObject.FindProperty("shaderPack");
            layerDef = serializedObject.FindProperty("layerDef");
            
            foamEnabled = envParam.FindPropertyRelative("foamEnabled");
            depthBlendMode = envParam.FindPropertyRelative("depthBlendMode");
            underWaterMode = envParam.FindPropertyRelative("underWaterMode");

            sunLight = envParam.FindPropertyRelative("sunLight");
            sunIntensity = envParam.FindPropertyRelative("sunIntensity");
            sunMode = envParam.FindPropertyRelative("sunMode");
            sunOccluMask = envParam.FindPropertyRelative("sunOccluMask");

            underAmb = envParam.FindPropertyRelative("underAmb");
            skirt = envParam.FindPropertyRelative("skirt");
            depthFade = envParam.FindPropertyRelative("depthFade");
            surfaceFade = envParam.FindPropertyRelative("surfaceFade");
            distortMag = envParam.FindPropertyRelative("distortMag");
            distortFrq = envParam.FindPropertyRelative("distortFrq");

            underDistortMap = envParam.FindPropertyRelative("underDistortMap");
            underWaterShader = envParam.FindPropertyRelative("underWaterShader");

            underButtom = envParam.FindPropertyRelative("underButtom");
           
            renderSpace = tmParam.FindPropertyRelative("renderSpace");
            trailMaskSize = tmParam.FindPropertyRelative("trailMaskSize");
            trailMaskScale = tmParam.FindPropertyRelative("trailMaskScale");
            trailMaskFade = tmParam.FindPropertyRelative("trailMaskFade");
            trailIntensity = tmParam.FindPropertyRelative("trailIntensity");
        }


	    static int tempFrame = 0;
	    static void Update()
	    {
	        if (!Application.isPlaying && !EditorApplication.isCompiling)
	        {
	            if (FOcean.instance && tempFrame < Time.renderedFrameCount)
	            {
                    FOcean.instance.ForceUpdate();
	                tempFrame = Time.renderedFrameCount + 2;
	            }
	        }
	    }

	    public override void OnInspectorGUI()
	    {
            serializedObject.Update();

            FOcean no = (FOcean)target;

			if (GUILayout.Button("Refresh Material"))
			{
                HashSet<FOceanGrid> grids = no.GetGrids();
                var _e = grids.GetEnumerator();
                while(_e.MoveNext())
                {
                    Material mat = _e.Current.oceanMaterial;
                    if (mat == null)
                        continue;

                    Material tmpMat = new Material(mat.shader);
                    tmpMat.shaderKeywords = mat.shaderKeywords;
                    tmpMat.renderQueue = mat.renderQueue;

                    int pc = ShaderUtil.GetPropertyCount(tmpMat.shader);
                    for (int i = 0; i < pc; i++)
                    {
                        string name = ShaderUtil.GetPropertyName(tmpMat.shader, i);
                        switch (ShaderUtil.GetPropertyType(tmpMat.shader, i))
                        {
                            case ShaderUtil.ShaderPropertyType.Color:
                                tmpMat.SetColor(name, mat.GetColor(name));
                                break;
                            case ShaderUtil.ShaderPropertyType.Range:
                                tmpMat.SetFloat(name, mat.GetFloat(name));
                                break;
                            case ShaderUtil.ShaderPropertyType.Float:
                                tmpMat.SetFloat(name, mat.GetFloat(name));
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                tmpMat.SetVector(name, mat.GetVector(name));
                                break;
                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                tmpMat.SetTexture(name, mat.GetTexture(name));
                                break;
                        }
                    }

                    mat.CopyPropertiesFromMaterial(tmpMat);
                    DestroyImmediate(tmpMat);
                }
               

                if (FOcean.instance != null)
                    FOcean.instance.ForceReload(true);
			}

            if (GUILayout.Button("Reset Time"))
            {
                if (FOcean.instance != null)
                    FOcean.instance.ResetTime();
            }

            int tmp = EditorGUI.indentLevel;
            
            if (no != null)
            {
                EditorGUILayout.PropertyField(envParam);

                if (envParam.isExpanded)
                {
                    EditorGUI.indentLevel = 1;
 
                    foamEnabled.boolValue = EditorGUILayout.Toggle("Foam Enabled", no.envParam.foamEnabled);

                    EditorGUILayout.PropertyField(depthBlendMode);

                    underWaterMode.enumValueIndex = (int)(eFUnderWater)EditorGUILayout.EnumPopup("UnderWater Mode", no.envParam.underWaterMode);

                    if (underWaterMode.enumValueIndex == (int)eFUnderWaterMode.eUM_Blend)
                    {
                        EditorGUI.indentLevel = 2;


                        EditorGUILayout.PropertyField(skirt);
                        EditorGUILayout.PropertyField(underButtom);
                        EditorGUILayout.PropertyField(depthFade);
                        EditorGUILayout.PropertyField(surfaceFade);

                        EditorGUILayout.Space();

                        underAmb.colorValue = EditorGUILayout.ColorField("UnderWater Ambient", no.envParam.underAmb);

                        EditorGUILayout.PropertyField(distortMag);
                        EditorGUILayout.PropertyField(distortFrq);
                        EditorGUILayout.PropertyField(underDistortMap);
                        EditorGUILayout.PropertyField(underWaterShader);

                        EditorGUILayout.Space();

                        EditorGUI.indentLevel = 1;
                    }

                    sunLight.objectReferenceValue = (Light)EditorGUILayout.ObjectField("Sun Light", no.envParam.sunLight, typeof(Light), true);
                    sunMode.enumValueIndex = (int)(eFSunMode)EditorGUILayout.EnumPopup("Sun Mode", no.envParam.sunMode);
                    sunIntensity.floatValue = EditorGUILayout.Slider("Sun Intensity", no.envParam.sunIntensity, 0, 1);
                    EditorGUILayout.PropertyField(sunOccluMask);
                }

                EditorGUI.indentLevel = 0;
                EditorGUILayout.PropertyField(trailer);

                if (no.trailer != null)
                {
                    EditorGUILayout.PropertyField(tmParam);
                    if (tmParam.isExpanded)
                    {
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.PropertyField(renderSpace);
                        EditorGUILayout.PropertyField(trailMaskSize);
                        EditorGUILayout.PropertyField(trailMaskScale);
                        EditorGUILayout.PropertyField(trailMaskFade);
                        EditorGUILayout.PropertyField(trailIntensity);
                        EditorGUI.indentLevel = 0;
                    }
                }
            }

            EditorGUILayout.PropertyField(shaderPack, true);
            EditorGUILayout.PropertyField(layerDef, true);

            serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = tmp;
	    }
	}
}
