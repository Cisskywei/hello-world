using PaintCraft.Controllers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace PatinCraft.UI
{
    public class UndoActiveButtonController : MonoBehaviour
    {
        public CanvasController Canvas;
        Button button;
        // Use this for initialization
        void Start ()
        {
            if (Canvas == null){
                Debug.LogError("you have to provide link to the Canvas for this component", gameObject);
            }

            button = GetComponent<Button>();
            button.onClick.AddListener(new UnityAction(() => {
                Canvas.Undo();
            }));
        }
	
        // Update is called once per frame
        void Update ()
        {
            button.interactable = Canvas.UndoManager.HasUndo();
        }
    }
}
