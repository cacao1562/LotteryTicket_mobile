using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SocketManager : MonoBehaviour
{
	private SocketIOComponent socket;
	private string roomNumber = "ticket";  //방 번호
	private JSONObject mobj;
	private bool check = false;
	private string uuid;
	private bool firstUser = false;
	public GameObject nextPopup;
	public GameObject buttonParticle;
	public GameObject internetPopup;
	public Drawable drawable;
	public GameObject soldOutImg;
	
	void Awake()
	{
	
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			//인터넷 연결 안되었을때
			internetPopup.SetActive(true);
		}

		int w = PlayerPrefs.GetInt("width");
		int h = PlayerPrefs.GetInt("height");
		// Screen.SetResolution(1920, 1080, false);
		float CameraSize = w / 100 * 1080 / 1920 * 0.5f; 
		Camera.main.orthographicSize = CameraSize;
		
		// uuid = SystemInfo.deviceUniqueIdentifier;
		// Debug.Log("uuid = " + uuid );

	}

	public void Start() 
	{
		
		// Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// Screen.SetResolution(Screen.width, Screen.width / 9 * 16 , true);
		
        Debug.Log("roomNum = "+roomNumber);
		
		socket = GameObject.Find ("SocketIO").GetComponent<SocketIOComponent> ();
		socket.On ("open", OnOpen);
		socket.On ("drawing", OnGetValue);
		socket.On ("error", OnError);
		socket.On ("close", OnClose);
		
		
	}



	
	// /** socket open될때 connect 소켓 보냄 */
	// public void sendConnect() {

	// 	JsonModel jm = new JsonModel();
	// 	jm.sendStr = "connect";
	// 	jm.result = uuid;
	// 	JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
	// 	socket.Emit("send", jo );
	// }

	// public void sendSuccess() {

	// 	JsonModel jm = new JsonModel();
	// 	jm.sendStr = "success";
	// 	JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
	// 	socket.Emit("send", jo );
	// }

	public void sendDrawing( Vector2 v2 ) {

		JsonModel jm = new JsonModel();
		jm.sendStr = "drawing";
		jm.result = v2.ToString();
		JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
		socket.Emit("send", jo );
	}

	public void sendReset() {

		JsonModel jm = new JsonModel();
		jm.sendStr = "reset";
		JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
		socket.Emit("send", jo );
	}



	// /** gameOver 팝업에 ok버튼 누르면 앱 종료 */
	// public void clickedOkbutton() {

	// 	if(Application.platform == RuntimePlatform.Android) {
		
	// 		Application.Quit();

	// 	}//else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			
	// 	// 	NativeGallery.AppExit();
		
	// 	// }
		
	// }

	/** 홈키나 메뉴버튼 화면 껐을때 게임 다시 시작하게 state 소켓 보냄 */
	// public void sendState() {

	// 	JsonModel jm = new JsonModel();
	// 	jm.sendStr = "state";
	// 	JSONObject jo = new JSONObject(JsonUtility.ToJson(jm) );
	// 	socket.Emit("send", jo );
	// }

	// /** 홈키, 메뉴, 화면종료했을때  씬 다시 시작 */
	// void OnApplicationPause(bool pauseStatus)
	// {
	// 	if (pauseStatus && firstUser ) {

	// 		sendState();
	// 		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );
	// 	}
	// }

	void Update()
	{


		if(!socket.IsConnected){

			Debug.Log("소켓 연결 안됨");
			return;
		}

		if(mobj == null){
			return;
		}

		if(mobj.Count <= 0){
			return;
		}	
   		
		

		if (check) {
			
			JsonModel jm = JsonUtility.FromJson<JsonModel>(mobj.ToString());

			if (jm.sendStr == "endFade") {

				nextPopup.SetActive(true);
				// buttonParticle.SetActive(true);
				check = false;

			} else if (jm.sendStr == "startFade") {

				drawable.startFade();
				check = false;

			} else if (jm.sendStr == "soldOut") {

				drawable.fadding = true;
				soldOutImg.SetActive(true);
				check = false;
			}
			// if(jm.sendStr == "connect" && firstUser == false) { //글라스에서 connect받으면 카운트 하나올려서 보내줌

			// 	if (jm.result == uuid ) { //한 사람만 접속 되게
					
			// 		firstUser = true;
			// 		sendSuccess();
			// 		Debug.Log(" glass connected ");
			// 		check = false;

			// 	} else {

			// 		check = false;
			// 		Debug.Log(" uuid 다름 ");
					
			// 	}
			// 	check = false;
				
			// }else if (jm.sendStr == "success" ) {
			
			// 	if(firstUser) {
			
			// 		// plateImg.sprite = Resources.Load<Sprite>( "dish/" + jm.result );
			// 		Debug.Log("dish Number = " + jm.result );
			// 		// StartCoroutine( closePopup() );
			// 		check = false;

			// 	}
				
			// }
			// 		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );

		}else{

		
		}

		
	}



	public void OnOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open(): " + e.data);
	
		socket.Emit("joinRoom", JSONObject.StringObject(roomNumber)); //방번호
		// sendConnect();	
	}
	
	public void OnGetValue(SocketIOEvent e)
	{
		// Debug.Log("get_Value: " + e.data);
		
		if (e.data == null) {
			// Debug.Log(" data nulllllll ");
			return; 
		}
		mobj = e.data; 
		check = true;
		
	}
	
	public void OnError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error(): " + e.data);
	}
	
	public void OnClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close(): " + e.data);
	}

	public void clickButton() {
		Application.Quit();
	}


}

[System.Serializable]
public class JsonModel{
	public string sendStr;
	public string result;
}


