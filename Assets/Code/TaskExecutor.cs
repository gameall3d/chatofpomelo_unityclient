using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**************************************/
//FileName: TaskExecutor.cs
//Author: Star
//Data: 11/01/2014
//Describe: Manager of task
/**************************************/
public class TaskExecutor : MonoBehaviour 
{
	private Queue<Task> TaskQueue = new Queue<Task>();
	private object _queueLock = new object();
	private const int MAX_TASK_NUM = 100;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		lock(_queueLock)
		{
			if (TaskQueue.Count > 0)
				TaskQueue.Dequeue().Execute();
		}
	}

	public void ScheduleTask(Task newTask)
	{
		lock(_queueLock)
		{
			if (TaskQueue.Count < MAX_TASK_NUM)
			{
				TaskQueue.Enqueue(newTask);
			}
		}
	}
}
