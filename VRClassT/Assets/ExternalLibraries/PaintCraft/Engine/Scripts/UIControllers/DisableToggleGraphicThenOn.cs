using UnityEngine;
using UnityEngine.UI;


namespace PatinCraft.UI{    
    public class DisableToggleGraphicThenOn : MonoBehaviour
    {
        Toggle toggle;

        void Start ()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggleChange);
        }
        
        void OnToggleChange(bool arg0)
        {
            toggle.targetGraphic.gameObject.SetActive(!arg0);
        }       
    }
}
