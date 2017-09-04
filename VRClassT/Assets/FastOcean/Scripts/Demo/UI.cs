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
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

namespace FastOcean
{
    public class UI : MonoBehaviour
    {
        public Text info = null;
        public Toggle autoNextToggle = null;
        public Toggle autoSailToggle = null;
        public Toggle gammaToggle = null;

        public ToggleGroup qualityGroup = null;

        public Slider gridSize = null;

        public float nextTime = 12;
        public bool autoNext = true;
        public bool autoSail = false;

        public GameObject sail = null;
        public GameObject fps = null;

        public GameObject stick = null;

        public FScaleScreen screenScale = null;
        public Slider screenScaleSlider = null;
        
        public Toggle antialiasingToggle = null;
        public bool isAntialiasing = true;

        public List<Color> baseColorKeys = new List<Color>();
        public List<Color> deepColorKeys = new List<Color>();

        public Slider baseColorSlider = null;
        public Slider deepColorSlider = null;

        public Text baseColorText = null;
        public Text deepColorText = null;

        private float timeTotal = 0.0f;

        void Start()
        {
            if (autoNextToggle != null)
                autoNextToggle.isOn = autoNext;

            if (autoSailToggle != null)
                autoSailToggle.isOn = autoSail;

            if (antialiasingToggle != null)
                antialiasingToggle.isOn = isAntialiasing;
        }

        // static int countLoop = 0;
        // Update is called once per frame
        public void OnClickNextScene() 
        {
             Scene scene = SceneManager.GetActiveScene();
             int i = scene.buildIndex;
             SceneManager.LoadScene((i + 1) % SceneManager.sceneCountInBuildSettings, LoadSceneMode.Single);
        }

        void FixedUpdate()
        {
            if (FOcean.instance == null)
                return;

            if (autoNext)
            {
                timeTotal += Time.fixedDeltaTime;
                if (timeTotal > nextTime)
                {
                    OnClickNextScene();
                }
            }
            else
                timeTotal = 0.0f;

            if(sail != null && fps != null && stick != null)
            {
                bool bSail = sail.activeSelf;
                sail.SetActive(autoSail);
                fps.SetActive(!autoSail);
                stick.SetActive(!autoSail);

                if(bSail != sail.activeSelf)
                {
                    TurnQuality();
                }

                FOcean.instance.trailer = autoSail ? sail.transform : null;
            }

            if (Camera.main != null)
            {
                FColorCorrection gammaCorrection = (FColorCorrection)Camera.main.gameObject.GetComponent(typeof(FColorCorrection));
                if (gammaCorrection != null && gammaToggle != null)
                {
                    gammaCorrection.enabled = gammaToggle.isOn;
                }
            }

            if(screenScale != null && screenScaleSlider != null)
               screenScaleSlider.value = screenScale.scale;

            if (Camera.main != null)
            {
                FAntialiasing antialiasing = (FAntialiasing)Camera.main.gameObject.GetComponent(typeof(FAntialiasing));
                if (antialiasing != null && antialiasingToggle != null)
                {
                    antialiasing.enabled = isAntialiasing;
                }
            }

            if (gridSize != null && FOcean.instance.mainPG != null)
            {
                gridSize.enabled = FOcean.instance.supportSM3;
                gridSize.value = FOcean.instance.mainPG.baseParam.usedGridSize;
            }

            if(baseColorSlider != null && baseColorText != null)
            {
                Color s = FOcean.SuppleColor(FOcean.instance.GetBaseColor());

                ColorBlock block = baseColorSlider.colors;
                block.highlightedColor = s;
                block.normalColor = s;
                baseColorSlider.colors = block;

                baseColorText.color = s;
            }

            if (deepColorSlider != null && deepColorText != null)
            {
                Color s = FOcean.SuppleColor(FOcean.instance.GetDeepColor());

                ColorBlock block = deepColorSlider.colors;
                block.highlightedColor = s;
                block.normalColor = s;
                deepColorSlider.colors = block;

                deepColorText.color = s;
            }

           
        }

        public void TurnNext()
        {
            autoNext = autoNextToggle.isOn;
        }

        public void TurnSail()
        {
            autoSail = autoSailToggle.isOn;
        }

        public void ChangeGridSize()
        {
            if (FOcean.instance == null)
                return;

            if (gridSize == null)
                return;

            if (FOcean.instance.mainPG != null)
            {
                FOcean.instance.mainPG.baseParam.usedGridSize = (int)gridSize.value;
            }
        }

        public void ChangeScreenScale()
        {
            if (screenScale == null || screenScaleSlider == null)
                return;

            screenScale.scale  = (int)screenScaleSlider.value;
        }

        public void ChangeBaseColor()
        {
            if (FOcean.instance == null || baseColorSlider == null)
                return;

            int i = (int)baseColorSlider.value;
            if (i < baseColorKeys.Count)
            {
                FOcean.instance.SetBaseColor(baseColorKeys[i]);
            }
        }

        public void ChangeDeepColor()
        {
            if (FOcean.instance == null || deepColorSlider == null)
                return;

            int i = (int)deepColorSlider.value;
            if (i < deepColorKeys.Count)
            {
                FOcean.instance.SetDeepColor(deepColorKeys[i]);
            }
        }

        public void ChangeAntialiasing()
        {
            isAntialiasing = antialiasingToggle.isOn;
        }

        public void TurnQuality()
        {
            if (FOcean.instance == null)
                return;

            if (qualityGroup == null)
                return;

            if (Camera.main == null)
                return;

            FSunShafts sunshaft = Camera.main.GetComponent<FSunShafts>();
            FClouds clouds = Camera.main.GetComponent<FClouds>();
            FGlareEffect glare = Camera.main.GetComponent<FGlareEffect>();

            if (clouds == null || glare == null || sunshaft == null)
                return;

            HashSet<FOceanGrid> grids = FOcean.instance.GetGrids();
            IEnumerable<Toggle> toggles = qualityGroup.ActiveToggles();
            var _e = toggles.GetEnumerator();
            if (_e.MoveNext())
            {
                switch (_e.Current.gameObject.name)
                {
                    case "Ultra":
                        if (!FOcean.instance.mobile && FOcean.instance.supportSM3)
                        {
                            FOcean.instance.envParam.foamEnabled = true;
                            FOcean.instance.envParam.depthBlendMode = eFDepthBlendMode.eDB_DepthBlend;
                            FOcean.instance.envParam.sunMode = eFSunMode.eSM_Phong;
                            FOcean.instance.envParam.underWaterMode = eFUnderWaterMode.eUM_Blend;
                            sunshaft.enableShaft = true;
                            clouds.enableCloud = true;
                            clouds.quality = eCLQuality.eCL_High;
                            glare.enableGlare = true;

                            if (grids != null)
                            {
                                var _ge = grids.GetEnumerator();
                                while (_ge.MoveNext())
                                {
                                    _ge.Current.baseParam.usedGridSize = 254;
                                    _ge.Current.gwParam.mode = eFGWMode.eGWM_RenderAndSimulation;
                                }
                            }
                        }

                        QualitySettings.masterTextureLimit = 0;
                        break;
                    case "High":
                        if (!FOcean.instance.mobile && FOcean.instance.supportSM3)
                        {
                            FOcean.instance.envParam.foamEnabled = true;
                            FOcean.instance.envParam.depthBlendMode = eFDepthBlendMode.eDB_DepthBlend;
                            FOcean.instance.envParam.sunMode = eFSunMode.eSM_Phong;
                            FOcean.instance.envParam.underWaterMode = eFUnderWaterMode.eUM_Blend;
                            sunshaft.enableShaft = false;
                            clouds.enableCloud = true;
                            clouds.quality = eCLQuality.eCL_Medium;
                            glare.enableGlare = true;

                            if (grids != null)
                            {
                                var _ge = grids.GetEnumerator();
                                while (_ge.MoveNext())
                                {
                                    _ge.Current.baseParam.usedGridSize = 254;
                                    _ge.Current.gwParam.mode = eFGWMode.eGWM_RenderAndSimulation;
                                }
                            }
                        }

                        QualitySettings.masterTextureLimit = 0;
                        break;
                    case "Medium":
                        if (FOcean.instance.supportSM3)
                        {
                            FOcean.instance.envParam.depthBlendMode = eFDepthBlendMode.eDB_DepthBlend;
                            FOcean.instance.envParam.underWaterMode = FOcean.instance.mobile ? eFUnderWaterMode.eUM_Simple : eFUnderWaterMode.eUM_Blend;
                            sunshaft.enableShaft = false;
                            clouds.enableCloud = true;
                            clouds.quality = eCLQuality.eCL_Fast;
                            glare.enableGlare = false;

                            FOcean.instance.envParam.foamEnabled = true;
                            FOcean.instance.envParam.sunMode = eFSunMode.eSM_Phong;

                            if (grids != null)
                            {
                                var _ge = grids.GetEnumerator();
                                while (_ge.MoveNext())
                                {
                                    _ge.Current.baseParam.usedGridSize = 128;
                                    _ge.Current.gwParam.mode = eFGWMode.eGWM_RenderAndSimulation;
                                }
                            }
                        }

                        QualitySettings.masterTextureLimit = 0;
                        break;
                    case "Fast":
                        FOcean.instance.envParam.foamEnabled = true;
                        FOcean.instance.envParam.sunMode = eFSunMode.eSM_Phong;
                        FOcean.instance.envParam.depthBlendMode = eFDepthBlendMode.eDB_None;
                        FOcean.instance.envParam.underWaterMode = eFUnderWaterMode.eUM_Simple;
                        sunshaft.enableShaft = false;
                        clouds.enableCloud = false;
                        glare.enableGlare = false;

                        if (grids != null)
                        {
                            var _ge = grids.GetEnumerator();
                            while (_ge.MoveNext())
                            {
                                _ge.Current.baseParam.usedGridSize = 32;
                                _ge.Current.gwParam.mode = eFGWMode.eGWM_Simulation;
                            }
                        }

                        QualitySettings.masterTextureLimit = 1;
                        break;
                }
            }
            
            FOcean.instance.UnderStateReset();
        }
    }
}
