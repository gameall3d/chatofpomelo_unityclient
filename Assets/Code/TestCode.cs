using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using SimpleJson;

public class TestCode : MonoBehaviour {
	PomeloClient pClient = new PomeloClient("127.0.0.1", 3014);
	// Use this for initialization
	void Start () {
		Debug.Log ("Begin to Connect");
		ConnectToServer ();
	}

	void ConnectToServer()
	{
		pClient.connect(null, (data)=>
		{
			Debug.Log(data);
			JsonObject msg = new JsonObject();
			msg["user"] = "hello pomelo";
			pClient.request("gate.gateHandler.queryEntry", msg, OnRequest);
		});
	}

	void OnRequest(JsonObject result)
	{
		Debug.Log ("Result:" + result);
	}

	public void OnClickButton()
	{
		Debug.Log ("Button Clicked");
		JsonObject msg = new JsonObject();
		msg["user"] = "hello pomelo";
		pClient.request("gate.gateHandler.queryEntry", msg, OnRequest);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
