using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintCraft.Canvas.Configs;
using UnityEngine.UI;
using PaintCraft.Canvas;
using UnityEngine.SceneManagement;
using PaintCraft.Tools;
using PatinCraft.UI;
using PaintCraft.Controllers;

namespace PaintCraft.Demo{
    public class SwitchRegionsToggleController : MonoBehaviour {
        public PageConfig NormalConfig;
        public PageConfig RegionConfig;

        public Brush NormalBrush;
        public Brush RegionBrush;

        public ChangeBrushOnClickController BrushBtnCtrl;

        void Start(){
            
            if (AppData.SelectedPageConfig != null){                
                GetComponent<Toggle>().isOn = (AppData.SelectedPageConfig == RegionConfig);
                if (AppData.SelectedPageConfig == RegionConfig){
                    BrushBtnCtrl.Brush = RegionBrush;
                } else {
                    BrushBtnCtrl.Brush = NormalBrush;
                }
            }
        }
                           
        // Update is called once per frame
        public void UpdateCurrentPage (bool useRegionConfig) {
            if (useRegionConfig){
                SetNewPageConfig(RegionConfig);
            } else {
                SetNewPageConfig(NormalConfig);
            }

        }

        void SetNewPageConfig(PageConfig pageConfig){
            if (AppData.SelectedPageConfig != pageConfig){                
                AppData.SelectedPageConfig = pageConfig;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }       
}