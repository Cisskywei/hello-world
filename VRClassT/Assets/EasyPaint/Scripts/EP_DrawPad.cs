using UnityEngine;
using System.Collections;

// For uGUI event system
using UnityEngine.EventSystems;

// For uGUI components
using UnityEngine.UI;

// For "List"
using System.Collections.Generic;

public class EP_DrawPad : MonoBehaviour {

	// Texture width and height
	private float textureW, textureH;
	
	// Texture for the canvas
	private Texture2D texture;
	
	// Store all of the pixels color on current texture, avoid to texture.GetPixel() on the runtime
	private Color[] nowTexture;
	
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
	
	// Smoothness of the curve
	private int smoothness = 2;

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
		
		// Create new texture
		texture = new Texture2D((int)textureW, (int)textureH, TextureFormat.RGBA32, false);
		GetComponent<Renderer>().material.mainTexture = texture;
		nowTexture = new Color[texture.width * texture.height];
		
		// Change the canvas to the raw color
		ChangeCanvasColorTo(canvasColorRaw, texture);
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
		previousPreviousTouchPos = pos * textureScale;
		previousTouchPos = pos * textureScale;
	}

	// Set the status of canvas and end the painting
	void OnPressUpDrawPad(){
		isConnectingPaint = false;
	}

	// Set the status of canvas and painting
	void OnDrawing(Vector3 pos){
		// Get touch position
		currentTouchPos = pos * textureScale;
		
		// If touch on the canvas and get ready to paint just one point or continuous line
		if(!isConnectingPaint || (currentTouchPos != previousTouchPos)){
			PaintOnPos(currentTouchPos, texture);
			previousPreviousTouchPos = previousTouchPos;
			previousTouchPos = currentTouchPos;
			isConnectingPaint = true;
		}
	}

	void PaintOnPos(Vector2 targetPos, Texture2D targetTex){
		int radius = (int)(currentBrushSize / 2);
		
		// If Continuous line
		// Find out 2 mid-point and use prevoius point for the control point to calculate the curve
		if(currentTouchPos != previousTouchPos){
			Vector2 mid1 = MidPoint(previousTouchPos, previousPreviousTouchPos);
			Vector2 mid2 = MidPoint(currentTouchPos, previousTouchPos);
			
			float segmentDistance = Vector2.Distance(mid1, mid2);
			
			ColorThePointOnCurve(mid1, mid2, previousTouchPos, (int)segmentDistance * smoothness, radius, targetTex);
		}
		
		// If just touch one only or the starting point of the line
		if(!isConnectingPaint){
			ColorThePoint(currentTouchPos, radius, targetTex);
		}
		
		// Apply the pixels change on the texture
		targetTex.Apply();
	}
	
	// Calculate all of the point on this Quadratic Bezier Curve
	void ColorThePointOnCurve(Vector2 startPos, Vector2 endPos, Vector2 control1, int segments, float radius, Texture2D targetTex){
		Vector2[] linePoints = new Vector2[segments];
		float t = 0f;
		float step = 1f / segments;
		for(int i = 0; i < segments; i++){
			Vector2 qPos = (1f - t) * (1f - t) * startPos + 2f * (1f - t) * t * control1 + t * t * endPos;
			linePoints[i] = qPos;
			t += step;
			ColorThePoint(qPos, radius, targetTex);
		}
	}
	
	// Color the point by compare the radius and the distance between test point and origin
	void ColorThePoint(Vector2 pos, float radius, Texture2D targetTex){
		Vector2 start = new Vector2(Mathf.Clamp(pos.x - radius, 0 , targetTex.width), Mathf.Clamp(pos.y - radius, 0 , targetTex.height));
		Vector2 end = new Vector2(Mathf.Clamp(pos.x + radius, 0, targetTex.width), Mathf.Clamp(pos.y + radius, 0, targetTex.height));
		int lenghtX = (int)(end.x - start.x);
		int lenghtY = (int)(end.y - start.y);
		Color[] pixels = new Color[lenghtX * lenghtY];
		
		for(int y = 0; y < lenghtY; y++){
			int indexNowY = (int)start.y + y;
			for(int x = 0; x < lenghtX; x++){
				int indexNow = indexNowY * texture.width + (int)start.x + x;
				int index = y * lenghtX + x;
				float dist = Vector2.Distance(pos, new Vector2(pos.x + x - lenghtX / 2, pos.y + y - lenghtY / 2));
				pixels[index] = Color.Lerp(currentColor, nowTexture[indexNow], dist / radius);
				nowTexture[indexNow] = pixels[index];
			}
		}
		targetTex.SetPixels((int)start.x, (int)start.y, lenghtX, lenghtY, pixels, 0);
	}

	// Change the canvas to new color
	void ChangeCanvasColorTo(Color newColor, Texture2D targetTex){
		for(int i = 0; i < nowTexture.Length; i++){
			nowTexture[i] = newColor;
		}
		targetTex.SetPixels(nowTexture);
		targetTex.Apply();
	}

	// Change the current painting color
	void ChangeCurrentColorTo(Color newColor){
		currentColor = newColor;
	}

	// Change the current tool name and take care on Eraser
	void ChangeToolTo(string toolName){
		if(toolName == "Pen"){
			currentBrushSize = rawBrushSize;
			currentColor = savedColor;
		}

		if(toolName == "Eraser"){
			currentBrushSize = rawBrushSize * eraserBrushSizeScale;
			savedColor = currentColor;
			currentColor = canvasColorRaw;
		}
	}

	// Clear the canvas
	void ClearCanvas(){
		ChangeCanvasColorTo(canvasColorRaw, texture);
	}

	// Find out the mid-point on 2 given Vector2
	Vector2 MidPoint(Vector2 p1, Vector2 p2){
		return new Vector2((p1.x + p2.x) * 0.5f, (p1.y + p2.y) * 0.5f);
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
	
	// Clear the canvas
	public void OnClickClear(){
		ClearCanvas();
	}
	
	// Change the drawing color and update the shadow position
	public void OnClickColor(GameObject obj){
		if(currentTool != btnEraser){
			colorShadow.GetComponent<RectTransform>().localPosition = obj.GetComponent<RectTransform>().localPosition;
			ChangeCurrentColorTo(obj.GetComponent<Image>().color);
		}
	}
}
