using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SkillzDelegate : MonoBehaviour {
	
	public void startGame(Dictionary<string, string> gameParams) {
		PlayerPrefs.SetInt("SkillzGame", 1);
		Application.LoadLevel("game");
	}
	
	/// <summary>
	/// Receives the message from AndroidSDK.
	/// </summary>
	/// <param name='message'>
	/// Message.
	/// </param>
	public void receiveSkillzMessage(string message) {
		startGame(Skillz.stringToDictionary(message));	
	}
	
}

