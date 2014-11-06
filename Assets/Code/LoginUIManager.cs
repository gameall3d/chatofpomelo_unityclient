using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Pomelo.DotNetClient;
using SimpleJson;

/**************************************/
//FileName: LoginUIManager.cs
//Author: Star
//Data: 10/10/2014
//Describe: Manager for the login UI
/**************************************/
public class LoginUIManager : MonoBehaviour 
{
	/** reference object*/
	// for login
	public Text UserName;
	public Text Channel;

	public GameObject LoginPanel;
	
	private bool bLoginToRoom = false;

	PomeloClient pClient = new PomeloClient("127.0.0.1", 3014);
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (bLoginToRoom)
		{
			Application.LoadLevel ("chatroom");
		}
	}

	public void OnClickLoginButton()
	{
		if (UserName.text != "" && Channel.text != "")
			Login ();
	}

	void Login()
	{
		pClient.connect(null, (data)=>
		                {
			Debug.Log(data);
			JsonObject userMsg = new JsonObject();
			userMsg["uid"] = UserName.text;
			pClient.request("gate.gateHandler.queryEntry", userMsg, OnEntryRequest);
		});
	}

	void OnEntryRequest(JsonObject result)
	{
		Debug.Log ("Entry:"+result);
		System.Object code = null;
		if (result.TryGetValue("code", out code))
		{
			if (Convert.ToInt32(code) == 500)
			{
				return;
			}
			else
			{
				pClient.disconnect();
				pClient = null;
				System.Object host, port;
				if (result.TryGetValue("host", out host) && 
				    result.TryGetValue("port", out port))
				{
					Debug.Log(host.ToString()+":"+ Convert.ToInt32(port));
					pClient = new PomeloClient("127.0.0.1", 3050);
					JsonObject userMsg = new JsonObject ();
					userMsg.Add ("username", UserName.text);
					userMsg.Add ("rid", Channel.text);
					pClient.connect(userMsg, (data)=>
					{
						Debug.Log(data);
						Entry();
					});

				}
			}
		}
	}

	void Entry()
	{
		JsonObject userMsg = new JsonObject ();
		userMsg.Add ("username", UserName.text);
		userMsg.Add ("rid", Channel.text);

		Debug.Log (pClient);
		if (pClient != null)
		{
			Debug.Log("Send Connect Msg:"+UserName.text + "|"+ Channel.text);
			pClient.request("connector.entryHandler.enter", userMsg, (data)=>
			{
				Debug.Log(data);
				OnEnter(data);
			});
		}
	}

	void OnEnter(JsonObject userData)
	{
		PlayerManager.Instance.UserName = UserName.text;
		PlayerManager.Instance.Channel = Channel.text;
		PlayerManager.Instance.LoginUserData = userData;
		PlayerManager.Instance.pClient = pClient;

		bLoginToRoom = true;

	}
}
