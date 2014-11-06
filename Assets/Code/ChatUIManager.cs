using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Pomelo.DotNetClient;
using SimpleJson;

/**************************************/
//FileName: ChatUIManager.cs
//Author: Star
//Data: 10/10/2014
//Describe: Manager for the chat UI
/**************************************/
public class ChatUIManager : MonoBehaviour 
{
	/** reference object*/
	// for chat
	public GameObject ChatPanel;
	public Text InputContent;
	public Text ChatShowContent;
	public GameObject UserItemGroup;
	public GameObject UserItemPre;
	private UserNameItem CurUserNameItem;

	// for logic data
	private float TabItemWidth = 0;
	private float UserItemHeight = 0;
	private Toggle CurTabItem = null;
	private ChatContent CurChatContent = null;
	private List<ChatRecord> chatRecords = new List<ChatRecord>();
	private List<string> UserNameList = new List<string>();
	private float UserListScrollHeight = 450;
	private TaskExecutor mTaskExecutor = null;

	private static ChatUIManager instance = null;
	public static ChatUIManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}
		
	// Use this for initialization
	void Start () 
	{
		UserItemHeight = UserItemPre.GetComponent<RectTransform> ().rect.height;
		UserListScrollHeight = UserItemGroup.GetComponent<RectTransform> ().rect.height;
		Debug.Log (UserListScrollHeight);
		mTaskExecutor = GetComponent<TaskExecutor> ();
		if (mTaskExecutor == null)
			mTaskExecutor = gameObject.AddComponent<TaskExecutor> ();

		InitUserList ();

		InitNetEvent ();
	}

	void InitNetEvent()
	{
		PomeloClient pClient = PlayerManager.Instance.pClient;

		if (pClient != null)
		{
			pClient.on ("onAdd", (data) => {
				OnUserAdd(data);		
			});

			pClient.on ("onLeave", (data) => {
				OnUserLeave(data);		
			});

			pClient.on ("onChat", (data) => {
				OnChatAdd(data);		
			});
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnChat(string target, string content)
	{
		JsonObject message = new JsonObject ();
		message.Add ("rid", PlayerManager.Instance.Channel);
		message.Add ("content", content);
		message.Add ("from", PlayerManager.Instance.UserName);
		message.Add ("target", target);

		PlayerManager.Instance.pClient.request("chat.chatHandler.send", message, (data) => {
			if (target != "*" && target != PlayerManager.Instance.UserName){
				chatRecords.Add(new ChatRecord(PlayerManager.Instance.UserName, content));	
			}
		});
	}

	// init the user list when first login
	void InitUserList()
	{
	 	CurUserNameItem = AddNewUserItem ("All", GlobalDefines.UserType.Admin);

		JsonObject jsonObj = PlayerManager.Instance.LoginUserData;
		System.Object users = null;
		if (jsonObj != null && jsonObj.TryGetValue("users", out users))
		{
			string userStr = users.ToString();
			string[] initUsers = userStr.Substring(1, userStr.Length - 2).Split(',');
			for (int i = 0; i < initUsers.Length; i++)
			{
				string s = initUsers[i];
				UserNameList.Add(s.Substring(1, s.Length - 2));
			}
		}

		UpdateUserListWindow ();
	}

	// Update the user name list window
	void UpdateUserListWindow()
	{
		UserNameItem uni = null;
		foreach(string name in UserNameList)
		{
			bool isFind = false;
			for (int i = 0; i < UserItemGroup.transform.childCount; i++)
			{
				uni = UserItemGroup.transform.GetChild(i).GetComponent<UserNameItem>();
				if (uni.Name.Equals(name))
				{
					uni.SetStateActive();
					isFind = true;
					break;
				}
			}

			if (!isFind)
			{
				AddNewUserItem(name, GlobalDefines.UserType.User);
			}
		}
	}

	// find the user name item by name
	UserNameItem FindUserNameItemByName(string name)
	{
		UserNameItem uni = null;

		for (int i = 0; i < UserItemGroup.transform.childCount; i++)
		{
			uni = UserItemGroup.transform.GetChild(i).GetComponent<UserNameItem>();
			if (uni.Name.Equals(name))
			{
				break;
			}
		}
			
		return uni;
	}

	// deactive the user item in the window
	void RemoveUserFromUserWindow(string name)
	{
		UserNameItem uni = FindUserNameItemByName(name);
		uni.SetStateDeActive();
	}

	void OnUserAdd(JsonObject data)
	{
		System.Object user = null;
		if (data.TryGetValue("user", out user))
		{
			UserNameList.Add(user.ToString());
			mTaskExecutor.ScheduleTask(new Task(new Action(UpdateUserListWindow)));
		}
	}

	void OnUserLeave(JsonObject data)
	{
		System.Object user = null;
		if (data.TryGetValue("user", out user))
		{
			string userName = user.ToString();
			UserNameList.Remove(userName);
			mTaskExecutor.ScheduleTask(new Task(new Action<string>(RemoveUserFromUserWindow), userName));
		}
	}

	void AddChatMsgToContent(string fromName, string targetName, string msg)
	{
		if (targetName.Equals ("*"))
			targetName = "All";
		ChatShowContent.text += "[" + fromName + "]" + " say to " + "[" + targetName + "]" + ":" + msg + "\n";
	}

	// event handler when recieve chat message from server
	void OnChatAdd(JsonObject data)
	{
		System.Object msg = null;
		System.Object fromName = null;
		System.Object targetName = null;

		if (data.TryGetValue("msg", out msg) &&
		    data.TryGetValue("from", out fromName) &&
		    data.TryGetValue("target", out targetName))
		{
			Debug.Log("Chat Add"+msg);
			chatRecords.Add(new ChatRecord(fromName.ToString(), msg.ToString()));
			mTaskExecutor.ScheduleTask(new Task(new Action<string, string, string>(this.AddChatMsgToContent), fromName.ToString(), targetName.ToString(), msg.ToString()));
		}
	}

	// add a new user item to the user name list window
	UserNameItem AddNewUserItem(string name, GlobalDefines.UserType usertype)
	{
		int itemCount = UserItemGroup.transform.childCount;

		GameObject itemObj = Instantiate (UserItemPre) as GameObject;
		itemObj.transform.SetParent (UserItemGroup.transform, false);
		itemObj.transform.localPosition += new Vector3 (0, -itemCount * UserItemHeight, 0);

		// adjust vertical scroll view postion
		float curTotalHeight = (itemCount + 1) * UserItemHeight;
		if (curTotalHeight > UserListScrollHeight)
		{
			UserItemGroup.GetComponent<RectTransform> ().sizeDelta -= new Vector2(0, -UserItemHeight);
			Vector3 oriPos = UserItemGroup.GetComponent<RectTransform> ().position;
			UserItemGroup.GetComponent<RectTransform> ().position -= new Vector3 (0, UserItemHeight, 0);
		}

		itemObj.GetComponent<Toggle> ().group = UserItemGroup.GetComponent<ToggleGroup> ();
		itemObj.GetComponent<Toggle> ().isOn = false;

		// user name item
		UserNameItem uni = itemObj.GetComponent<UserNameItem> ();
		uni.SetName (name, Color.white);
		uni.CurUserType = usertype;
		uni.SetStateActive ();
		itemObj.GetComponent<Toggle> ().onValueChanged.AddListener (uni.OnToggleItem);

		return uni;
	}

	// call when the user item was toggled
	public void OnUserItemSelect(UserNameItem uni)
	{
		CurUserNameItem = uni;
	}
	
	#region UI Event
	public void OnClickSendButton()
	{
		if (CurUserNameItem != null)
		{
			GlobalDefines.UserType type = CurUserNameItem.CurUserType;
			if (type == GlobalDefines.UserType.Admin)
			{
				OnChat ("*", InputContent.text);
			}
			else
			{
				OnChat(CurUserNameItem.Name, InputContent.text);
				AddChatMsgToContent (PlayerManager.Instance.UserName, CurUserNameItem.Name, InputContent.text);
			}

		}
	}
	#endregion


}
