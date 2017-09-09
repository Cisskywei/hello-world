using UnityEngine;
using PaintCraft.Controllers;
using UnityEngine.UI;
using UnityEngine.Events;

namespace PatinCraft.UI{   
    public class RedoActiveButtonController : MonoBehaviour {
        public CanvasController Canvas;
        Button button;

        void Start()
        {
            if (Canvas == null){
                Debug.LogError("you have to provide link to the Canvas for this component", gameObject);     
            }
            
            button = GetComponent<Button>();
            button.onClick.AddListener(new UnityAction(() => {
                Canvas.Redo();
            }));
        }

        void Update()
        {
            button.interactable = Canvas.UndoManager.HasRedo();
        }
    }
}
