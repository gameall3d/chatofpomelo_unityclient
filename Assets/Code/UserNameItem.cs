using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserNameItem : MonoBehaviour {
	
	// UI Object Renference
	public Text NameLabel;
	public GlobalDefines.UserType CurUserType { get; set; }

	public string Name { get; set; }

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnToggleItem(bool pressed)
	{
		ChatUIManager.Instance.OnUserItemSelect (this);
	}

	public void SetName(string name, Color color)
	{
		Name = name;
	}

	public void SetStateActive()
	{
		NameLabel.text = "<color=#99ff00>" + Name + "</color>";
	}

	public void SetStateDeActive()
	{
		NameLabel.text = "<color=#111111>" + Name + "</color>";
	}

}
