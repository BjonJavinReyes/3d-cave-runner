using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillzManager : MonoBehaviour {
	
	public static SkillzManager instance;
	
	public bool enableGCM = false;
	public bool enableLocalytics = false;
	
	void Awake() {
		if (instance == null) {
			instance = this;
			
			DontDestroyOnLoad(gameObject);
			
			Skillz.Init(Application.unityVersion);
			
			if (enableGCM) {
				if (!Skillz.IsGCMRegistered()) {
					Skillz.RegisterGCM();
				}
			}
			
			if (enableLocalytics) {
				SkillzLocalytics.Instance.Open();
				SkillzLocalytics.Instance.Upload();
			}
			
		} else if (instance != this) {
			DestroyObject(gameObject);
		}
	}

	void OnApplicationPause (bool isPaused) {
		if (isPaused) {
			if (enableLocalytics) {
				SkillzLocalytics.Instance.Close();
				SkillzLocalytics.Instance.Upload();
			}
		} else {
			if (enableLocalytics) {
				SkillzLocalytics.Instance.Open();
				SkillzLocalytics.Instance.Upload();
			}
		}
	}

	void OnApplicationQuit () {
		if (enableLocalytics) {
			SkillzLocalytics.Instance.Close();
			SkillzLocalytics.Instance.Upload();
		}
	}
}