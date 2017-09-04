using UnityEngine;
using System.Collections;

// For uGUI event system
using UnityEngine.EventSystems;

// For uGUI components
using UnityEngine.UI;

// For "List"
using System.Collections.Generic;

public class EP_DrawPadRT : MonoBehaviour {
	
	// Texture width and height
	private float textureW, textureH;
	
	// Texture is pixel perfect to fit the resolution when the scale is 1
	private float textureScale = 1f;
	
	// Canvas raw color
	private Color canvasColorRaw = Color.white;
	
	// For color selection
	private Color currentColor, savedColor;
	
	// Original size of the brush
	private float rawBrushSize = 6f;
	
	// Current size of the brush
	private float currentBrushSize = 6f;
	
	// Current size scale of the eraser
	private float eraserBrushSizeScale = 4f;
	
	// Store the drawing state for "Update"
	private bool isStartDrawing = false;
	
	// Is it continuous painting on the canvas
	private bool isConnectingPaint = false;
	
	// Store the position of the touch point
	private Vector2 currentTouchPos, previousTouchPos, previousPreviousTouchPos;

	// Store the RenderTexture source object
	private RenderTexture rtSource;

	// Store the Material for the canvas texture
	private Material canvasTexMat;

	// Store the Shader for the canvas
	public Shader canvasShader;

	// Store the Material for the brush texture
	private Material brushTexMat;

	// Store the brush texture
	public Texture2D brush;

	// Store the Shader for the brush
	public Shader brushShader;

	// Store the ToolBox object
	public GameObject toolBox;
	
	// Store the shadow object of the color button
	public GameObject colorShadow;
	
	// Store the Pen button
	public GameObject btnPen;
	
	// Store the Eraser button
	public GameObject btnEraser;
	
	// Store the current tool, eg. Pen or Eraser
	private GameObject currentTool;
	
	// Store the on/off state of the tool box
	private bool isToolBoxOpen = true;

	// Store the canvas to the history list
	private List<RenderTexture> historyRT = new List<RenderTexture>();

	// Max number of history can store
	public int maxHistoryRT;	// can undo n-1 steps

	// Current stored history
	private int totalHistoryRT = 0;
	
	void Start () {
		// If you want to scale the texture fit for difference devices, you can try this
		// For example:
		// if(iPhone.generation == iPhoneGeneration.iPad3Gen){
		//		texture = 0.5f;
		//	}
		// Or example:
		// if(Camera.main.pixelWidth > 1200){
		// 		texture = 0.5f;
		// }
		
		// Assign the width, height and the correct aspect ratio to the texture
		textureW = Camera.main.pixelWidth * textureScale;
		textureH = Camera.main.pixelHeight * textureScale;
		transform.localScale = new Vector3(textureW / textureH, 1f, 1f);

		// Assign new material to canvas and brush
		canvasTexMat = new Material(canvasShader);
		brushTexMat = new Material(brushShader);

		// Create new RenderTexture
		rtSource = new RenderTexture((int)textureW, (int)textureH, 0, RenderTextureFormat.ARGB32);
		rtSource.wrapMode = TextureWrapMode.Clamp;

		// Assign created RenderTexture to canvas and texture to brush
		GetComponent<Renderer>().material.SetTexture("_MainTex", rtSource);
		brushTexMat.SetTexture("_MainTex", brush);

		// Change the brush color to black
		ChangeCurrentColorTo(Color.black);
		
		// Change the canvas to the raw color
		ChangeCanvasColorTo(canvasColorRaw);
	}
	
	void Update () {
		// Check if touching on screen
		if(Input.touchCount == 1 || Input.GetKey(KeyCode.Mouse0)){
			Vector3 currentPointerPos = Input.mousePosition;
			
			// Check the touch is start on the DrawPad
			if(Input.GetMouseButtonDown(0)){
				
				// Raycast hit test, check is it touch on the UI object
				PointerEventData pointer = new PointerEventData(EventSystem.current);
				pointer.position = currentPointerPos;
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointer, raycastResults);
				
				if(raycastResults.Count > 0){
					// Touch start on the UI object, so do not start any drawing
				}else{
					// Touch start on the DrawPad, so start the drawing
					isStartDrawing = true;
					OnPressDownDrawPad(currentPointerPos);
				}
			}
			
			// If drawing started and moving the touch, so draw it
			if(Input.GetMouseButton(0) && isStartDrawing){
				OnDrawing(currentPointerPos);
			}
		}
		
		// If drawing started and no more touch on screen, so drawing end
		if(Input.GetMouseButtonUp(0) && isStartDrawing){
			isStartDrawing = false;
			OnPressUpDrawPad();
		}
	}
	
	// Set the status of canvas and ready the points for painting
	void OnPressDownDrawPad(Vector3 pos){
		KeepForUndo();
		previousPreviousTouchPos = pos * textureScale;
		previousTouchPos = pos * textureScale;
	}
	
	// Set the status of canvas and end the painting
	void OnPressUpDrawPad(){
		KeepForRedo();
		isConnectingPaint = false;
	}
	
	// Set the status of canvas and painting
	void OnDrawing(Vector3 pos){
		// Get touch position
		currentTouchPos = pos * textureScale;
		
		// If touch on the canvas and get ready to paint just one point or continuous line
		if(!isConnectingPaint || (currentTouchPos != previousTouchPos)){
			PaintOnPos(currentTouchPos);
			previousPreviousTouchPos = previousTouchPos;
			previousTouchPos = currentTouchPos;
			isConnectingPaint = true;
		}
	}
	
	void PaintOnPos(Vector2 targetPos){
		// If Continuous line
		// Find out 2 mid-point and use prevoius point for the control point to calculate the curve
		if(currentTouchPos != previousTouchPos){
			Vector2 mid1 = MidPoint(previousTouchPos, previousPreviousTouchPos);
			Vector2 mid2 = MidPoint(currentTouchPos, previousTouchPos);
			
			float segmentDistance = Vector2.Distance(mid1, mid2) * 2f;
			
			ColorThePointOnCurve(mid1, mid2, previousTouchPos, (int)segmentDistance);
		}

		// If just touch one only or the starting point of the line
		if(!isConnectingPaint){
			ColorThePoint(currentTouchPos);
		}
	}
	
	// Calculate all of the point on this Quadratic Bezier Curve
	void ColorThePointOnCurve(Vector2 startPos, Vector2 endPos, Vector2 control1, int segments){
		float t = 0f;
		float step = 1f / segments;
		for(int i = 0; i < segments; i++){
			Vector2 qPos = (1f - t) * (1f - t) * startPos + 2f * (1f - t) * t * control1 + t * t * endPos;
			t += step;
			ColorThePoint(qPos);
		}
	}
	
	// Color the point to the target position
	void ColorThePoint(Vector2 pos){
		RenderTexture.active = rtSource;
		GL.PushMatrix ();
		GL.LoadPixelMatrix (0, textureW, textureH, 0);
		Graphics.DrawTexture (new Rect (pos.x - currentBrushSize / 2, textureH - pos.y - currentBrushSize / 2, currentBrushSize, currentBrushSize), brush, brushTexMat);
		GL.PopMatrix ();
		RenderTexture.active = null;
	}
	
	// Change the canvas to new color
	void ChangeCanvasColorTo(Color newColor){
		canvasTexMat.SetVector("initialValue", newColor);
		Graphics.Blit(null, rtSource, canvasTexMat);
	}
	
	// Change the current painting color
	void ChangeCurrentColorTo(Color newColor){
		currentColor = newColor;
		brushTexMat.SetVector("_Color", currentColor);
	}
	
	// Change the current tool name and take care on Eraser
	void ChangeToolTo(string toolName){
		if(toolName == "Pen"){
			currentBrushSize = rawBrushSize;
			ChangeCurrentColorTo(savedColor);
		}
		
		if(toolName == "Eraser"){
			currentBrushSize = rawBrushSize * eraserBrushSizeScale;
			savedColor = currentColor;
			ChangeCurrentColorTo(canvasColorRaw);
		}
	}
	
	// Clear the canvas
	void ClearCanvas(){
		ChangeCanvasColorTo(canvasColorRaw);
	}
	
	// Find out the mid-point on 2 given Vector2
	Vector2 MidPoint(Vector2 p1, Vector2 p2){
		return new Vector2((p1.x + p2.x) * 0.5f, (p1.y + p2.y) * 0.5f);
	}

	// Release the oldest history and add the new history
	void KeepForUndo(){
		if(historyRT.Count > 0){
			for(int i = 0; i < historyRT.Count - totalHistoryRT; i++){
				historyRT[i + totalHistoryRT].Release();
			}
			historyRT.RemoveRange(totalHistoryRT, historyRT.Count - totalHistoryRT);
		}
		
		AddToHistoryRT();
		totalHistoryRT = Mathf.Min(totalHistoryRT + 1, maxHistoryRT - 1);
	}

	// Add the new history
	void KeepForRedo(){
		AddToHistoryRT();
	}

	// Store the RT and blit it to the history
	void AddToHistoryRT(){
		if(historyRT.Count == maxHistoryRT){
			historyRT[0].Release();
			historyRT.RemoveAt(0);
		}
		historyRT.Add(new RenderTexture((int)textureW, (int)textureH, 0, RenderTextureFormat.ARGB32));
		historyRT[historyRT.Count - 1].wrapMode = TextureWrapMode.Clamp;
		Graphics.Blit(rtSource, historyRT[historyRT.Count - 1]);
	}
	
	// ========== For uGUI button click events ==========
	
	// On/Off the tool box
	public void OnClickSwitch(){
		isToolBoxOpen = !isToolBoxOpen;
		
		Vector3 newPos = toolBox.GetComponent<RectTransform>().localPosition;
		
		if(isToolBoxOpen){
			newPos.y += 100f;
		}else{
			newPos.y -= 100f;
		}
		
		toolBox.GetComponent<RectTransform>().localPosition = newPos;
	}
	
	// Change the tool to Pen
	public void OnClickPen(){
		if(currentTool != btnPen){
			currentTool = btnPen;
			btnPen.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			btnEraser.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			ChangeToolTo("Pen");
		}
	}
	
	// Change the tool to eraser
	public void OnClickEraser(){
		if(currentTool != btnEraser){
			currentTool = btnEraser;
			btnEraser.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			btnPen.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			ChangeToolTo("Eraser");
		}
	}
	
	// Clear the canvas and clear the history
	public void OnClickClear(){
		ClearCanvas();
		historyRT.Clear();
		totalHistoryRT = 0;
	}

	// Blit the previous RT history to the canvas
	public void OnClickUndo(){
		if(historyRT.Count > 0){
			totalHistoryRT = Mathf.Max(totalHistoryRT - 1, 0);
			Graphics.Blit(historyRT[totalHistoryRT], rtSource);
		}
	}

	// Blit the latest RT history to the canvas
	public void OnClickRedo(){
		if(historyRT.Count > 0){
			totalHistoryRT = Mathf.Min(totalHistoryRT + 1, historyRT.Count - 1);
			Graphics.Blit(historyRT[totalHistoryRT], rtSource);
		}
	}
	
	// Change the drawing color and update the shadow position
	public void OnClickColor(GameObject obj){
		if(currentTool != btnEraser){
			colorShadow.GetComponent<RectTransform>().localPosition = obj.GetComponent<RectTransform>().localPosition;
			ChangeCurrentColorTo(obj.GetComponent<Image>().color);
		}
	}
}
