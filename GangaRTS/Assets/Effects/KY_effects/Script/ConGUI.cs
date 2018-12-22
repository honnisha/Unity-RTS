using UnityEngine;
using System.Collections;

public class ConGUI : MonoBehaviour {
	public Transform mainCamera;
	public Transform cameraTrs;
	public int rotSpeed = 20;
	public GameObject[] effectObj;
	public GameObject[] effectObProj;
	private int arrayNo = 0;
	
	private GameObject nowEffectObj;
	private string[] cameraState = {"Camera move" ,"Camera stop"};
	private int cameraRotCon = 1;
	
	private float num = 0F;
	private float numBck = 0F;
	private Vector3 initPos;
	
	private bool  haveProFlg = false;
	private GameObject nonProFX;

	private Vector3 tmpPos;

	bool visibleBt (){
		foreach(GameObject tmpObj in effectObProj){
			if( effectObj[ arrayNo ].name == tmpObj.name){
				nonProFX = tmpObj;
				return true;
			}
		}
		return false;
	}
	
	void  Start (){
		tmpPos = initPos = mainCamera.localPosition;
		
		haveProFlg = visibleBt();
	}
	
	void  Update (){
		if( cameraRotCon == 1)cameraTrs.Rotate(0 ,rotSpeed * Time.deltaTime ,0);
		
		if(num > numBck){
			numBck = num;
			tmpPos.y += 0.05f;
			tmpPos.z -= 0.3f;
		}else if(num < numBck){
			numBck = num;
			tmpPos.y -= 0.05f;
			tmpPos.z += 0.3f;
		}else if(num == 0){
			tmpPos.y = initPos.y;
			tmpPos.z = initPos.z;
		}
		
		if(tmpPos.y < initPos.y )tmpPos.y = initPos.y;
		if(tmpPos.z > initPos.z )tmpPos.z = initPos.z;

		mainCamera.localPosition = tmpPos;
	}
	
	void  OnGUI (){
		
		if (GUI.Button ( new Rect(20, 0, 30, 30), "←")) {//return
			arrayNo --;
			if(arrayNo < 0)arrayNo = effectObj.Length -1;
			effectOn();
			
			haveProFlg = visibleBt();
		}
		
		if (GUI.Button ( new Rect(50, 0, 200, 30), effectObj[ arrayNo ].name )) {
			effectOn();
		}
		
		if (GUI.Button ( new Rect(250, 0, 30, 30), "→")) {//next
			arrayNo ++;
			if(arrayNo >= effectObj.Length)arrayNo = 0;
			effectOn();
			
			haveProFlg = visibleBt();
		}
		
		if( haveProFlg ){
			if (GUI.Button ( new Rect(50, 30, 300, 70), "+Distorsion" )) {
				if(nowEffectObj != null)Destroy( nowEffectObj );
				nowEffectObj = Instantiate( nonProFX );
			}
		}
		
		
		if (GUI.Button ( new Rect(300, 0, 200, 30), cameraState[ cameraRotCon ] )) {
			if( cameraRotCon == 1){
				cameraRotCon = 0;
			}else{
				cameraRotCon = 1;
			}
		}
		
		num = GUI.VerticalSlider( new Rect(30, 100, 20, 200), num, 0, 20);
		
		
	}
	
	void  effectOn (){
		if(nowEffectObj != null)Destroy( nowEffectObj );
		nowEffectObj = Instantiate(effectObj[ arrayNo ] );
	}
}