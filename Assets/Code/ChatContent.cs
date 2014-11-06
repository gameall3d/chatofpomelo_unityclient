using UnityEngine;
using System.Collections;

/**************************************/
//FileName: ChatContent.cs
//Author: Star
//Data: 10/10/2014
//Describe: class for the chat content
/**************************************/
public class ChatContent : MonoBehaviour 
{
	public string Name { get; set; }

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnToggleTabItem(bool pressed)
	{
		Debug.Log ("ChatContent Pressed:" + pressed);
		gameObject.SetActive (pressed);
	}
}
