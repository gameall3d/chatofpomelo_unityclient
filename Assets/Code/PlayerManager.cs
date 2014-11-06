using System;
using SimpleJson;
using Pomelo.DotNetClient;

/**************************************/
//FileName: PlayerManager.cs
//Author: Star
//Data: 10/10/2014
//Describe: Manager of player
/**************************************/
public class PlayerManager
{
	private static PlayerManager instance = null;
	public static PlayerManager Instance
	{
		get
		{
			if (instance == null)
				instance = new PlayerManager();

			return instance;
		}
	}

	public string UserName { get; set; }
	public string Channel  { get; set; }
	public JsonObject LoginUserData { get; set; }
	public PomeloClient pClient { get; set; }

	public PlayerManager ()
	{
	}
}


