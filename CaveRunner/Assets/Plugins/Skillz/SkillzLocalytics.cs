using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillzLocalytics {

	private string SkillzLocalyticsClassName = "com.skillz.android.client.SkillzLocalyticsSession";
	private AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	private AndroidJavaObject obj_SkillzLocalytics;

	private static SkillzLocalytics _instance;
	
	public static SkillzLocalytics Instance {
		get {
			if (_instance == null) {
				_instance = new SkillzLocalytics();
				_instance.Init();
			}
			return _instance;
		}
	}

	private void Init() {
		using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
			obj_SkillzLocalytics = new AndroidJavaObject(SkillzLocalyticsClassName, obj_Activity);
		}
	}
	
	public void TagEvent(string eventId) {
		obj_SkillzLocalytics.Call("tagEvent", eventId);
	}
	
	public void TagEvent(string eventId, Dictionary<string, string> attributes) {
		obj_SkillzLocalytics.Call("tagEvent", eventId, dictionaryToHashmap(attributes));
	}
	
	public void TagEvent(string eventId, Dictionary<string, string> attributes, List<string> customDimensions) {
		obj_SkillzLocalytics.Call("tagEvent", eventId, dictionaryToHashmap(attributes), listToJavaList(customDimensions));
	}

	public void TagScreen(string screen) {
		obj_SkillzLocalytics.Call("tagScreen", screen);
	}
	
	public void Open() {
		obj_SkillzLocalytics.Call("open");
	}
	
	public void Open(List<string> customDimensions) {
		obj_SkillzLocalytics.Call("open", listToJavaList(customDimensions));
	}
	
	public void Close() {
		obj_SkillzLocalytics.Call("close");
	}
	
	public void Close(List<string> customDimensions) {
		obj_SkillzLocalytics.Call("close", listToJavaList(customDimensions));
	}

	public void Upload() {
		obj_SkillzLocalytics.Call("upload");
	}

	public void SetOptOut(bool isOptedOut) {
		obj_SkillzLocalytics.Call("setOptOut", isOptedOut);
	}

	private AndroidJavaObject listToJavaList(List<string> list) {
		using (AndroidJavaObject obj_List = new AndroidJavaObject("java.util.ArrayList")) {
			foreach (string item in list) {
				obj_List.Call("put", new AndroidJavaObject("java.lang.String", item));
			}

			return obj_List;
		}
	}
	
	private AndroidJavaObject dictionaryToHashmap(Dictionary<string, string> dict) {
		AndroidJavaObject hashmap = new AndroidJavaObject("java.util.HashMap");
		System.IntPtr methodPut = AndroidJNIHelper.GetMethodID(hashmap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		
		object[] args = new object[2];
		foreach(KeyValuePair<string, string> kvp in dict) {
			AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
			AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value);
			args[0] = k;
			args[1] = v;
			AndroidJNI.CallObjectMethod(hashmap.GetRawObject(), methodPut, AndroidJNIHelper.CreateJNIArgArray(args));
		}
		
		return hashmap;
	}
}