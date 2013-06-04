using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

public class Skillz {
	
	public static void Init(String unityVersion) {
		GetSkillz().CallStatic("init", GetContext());
		GetSkillz().CallStatic("setUnityPlayerVersion", unityVersion);
	}
	
	public static void StartSkillzActivity() {
		GetSkillz().CallStatic("startSkillzActivity", GetContext());
	}

	public static bool IsSkillzEnabled() {
		return GetSkillz().CallStatic<bool>("isSkillzEnabled", GetContext());
	}

	public static void ReportStats(int score, Dictionary<string, string> metrics) {
		AndroidJavaObject hashmap = Skillz.dictionaryToHashmap(metrics);
		GetSkillz().CallStatic("reportStats", GetContext(), score, hashmap);
	}
	
	public static void ReportFinalScore(Dictionary<string, string> metrics) {
		AndroidJavaObject hashmap = Skillz.dictionaryToHashmap(metrics);
		GetSkillz().CallStatic("reportFinalScore", GetContext(), hashmap);
	}

	public static void ReportScore(Dictionary<string, string> metrics) {
		AndroidJavaObject hashmap = Skillz.dictionaryToHashmap(metrics);
		GetSkillz().CallStatic("reportScore", GetContext(), hashmap);
	}

	public static void ReportPublisherDefinedValues(Dictionary<string, string> pars) {
		AndroidJavaObject hashmap = Skillz.dictionaryToHashmap(pars);
		GetSkillz().CallStatic("reportPublisherDefinedValues", GetContext(), hashmap);
	}
	
	public static void AbortGame() {
		AbortGame(true);
	}

	public static void AbortGame(bool returnToSkillz) {
		GetSkillz().CallStatic("abortGame", GetContext(), returnToSkillz);
	}
	
	public static double RandomValue() {
		AndroidJNI.AttachCurrentThread();
		AndroidJNI.PushLocalFrame(0);
		double val = GetSkillz().CallStatic<double>("random");
		AndroidJNI.PopLocalFrame(IntPtr.Zero);
		return val;
	}

	public static string Version() {
		IntPtr clazz = AndroidJNI.FindClass("com.skillz.android.client.Skillz");
		IntPtr fieldID = AndroidJNI.GetStaticFieldID(clazz, "VERSION", "Ljava/lang/String;");
		string version = AndroidJNI.GetStaticStringField(clazz, fieldID);
		return version;
	}

	public static string Id() {
		IntPtr clazz = AndroidJNI.FindClass("com.skillz.android.client.Skillz");
		IntPtr fieldID = AndroidJNI.GetStaticFieldID(clazz, "ID", "Ljava/lang/String;");
		string id = AndroidJNI.GetStaticStringField(clazz, fieldID);
		return id;
	}
	
	public static string GCMSenderId() {
		IntPtr clazz = AndroidJNI.FindClass("com.skillz.android.client.Skillz");
		IntPtr fieldID = AndroidJNI.GetStaticFieldID(clazz, "GCM_SENDER_ID", "Ljava/lang/String;");
		string id = AndroidJNI.GetStaticStringField(clazz, fieldID);
		return id;
	}

	public static void CheckBaseManifest() {
		GetSkillz().CallStatic("checkBaseManifest", GetContext());
	}

	public static void CheckGCMManifest() {
		GetSkillz().CallStatic("checkGCMManifest", GetContext());
	}
	
	public static void RegisterGCM() {
		GetSkillz().CallStatic("registerGCM", GetContext());	
	}
	
	public static void SetGCMRegistered() {
		GetSkillz().CallStatic("setGCMRegistered", GetContext());	
	}
	
	public static bool IsGCMRegistered() {
		return GetSkillz().CallStatic<bool>("isGCMRegistered", GetContext());
	}

	private static AndroidJavaObject GetContext() {
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		return jc.GetStatic<AndroidJavaObject>("currentActivity");
	}
	
	private static AndroidJavaClass GetSkillz() {
		return new AndroidJavaClass("com.skillz.android.client.Skillz");
	}
	
	private static AndroidJavaClass GetSkillzUnityPlayerActivity() {
		return new AndroidJavaClass("com.skillz.android.client.SkillzUnityPlayerActivity");
	}
	
	//helpers
	
	public static AndroidJavaObject dictionaryToHashmap(Dictionary<string, string> dict) {
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
	
	public static Dictionary<string,string> stringToDictionary(string str) {
		if (str != null && str.Length > 0) {
			Dictionary<string, string> dict = new Dictionary<string, string>();
			String[] chunks = str.Split('&');
			
			for (int i = 0; i < chunks.Length; i++) {
				String[] nvp = chunks[i].Split('=');
				
				dict[nvp[0]] = nvp[1];
			}
			
			return dict;
		}
		return null;
	}
	
	public static class Random {
	
		/**
		 * Seeded random value from Skillz.
		 **/
		public static float Value() {
			float val = (float) Skillz.RandomValue();
			return val;
		}
		
		/**
		 * Find a point inside the unit sphere.
		 **/
		public static Vector3 InsideUnitSphere() {
			float r = Value();
			float phi = Value() * Mathf.PI;
			float theta = Value() * Mathf.PI * 2;
			
			float x = r * Mathf.Cos(theta) * Mathf.Sin(phi);
			float y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
			float z = r * Mathf.Cos(phi);
			
			return new Vector3(x, y ,z);
		}
		
		/**
		 * Find a point inside the unit circle.
		 **/
		public static Vector2 InsideUnitCircle() {
			float radius = 1.0f;
			float rand = Value() * 2 * Mathf.PI;
			Vector2 val = new Vector2();
			
			val.x = radius * Mathf.Cos(rand);
			val.y = radius * Mathf.Sin(rand);
			
			return val;
		}
		
		/**
		 * Hybrid rejection / trig method to generate points on a sphere using Skillz random.
		 **/
		public static Vector3 OnUnitSphere() {
			Vector3 val = new Vector3();
			float s;
			
			do {
				val.x = 2 * (float) Value() - 1;
				val.y = 2 * (float) Value() - 1;
				s = Mathf.Pow(val.x, 2) + Mathf.Pow(val.y, 2);
			} while (s > 1);
			
			float r = 2 * Mathf.Sqrt(1 - s);
			
			val.x *= r;
			val.y *= r;
			val.z = 2 * s - 1;
			
			return val;
		}
		
		/**
		 * Quaternion random from Skillz random.
		 **/
		public static Quaternion RotationUniform() {
			float u1 = Value();
			float u2 = Value();
			float u3 = Value();
			
			float u1sqrt = Mathf.Sqrt(u1);
			float u1m1sqrt = Mathf.Sqrt(1 - u1);
			float x = u1m1sqrt * Mathf.Sin(2 * Mathf.PI * u2);
			float y = u1m1sqrt * Mathf.Cos(2 * Mathf.PI * u2);
			float z = u1sqrt * Mathf.Sin(2 * Mathf.PI * u3);
			float w = u1sqrt * Mathf.Cos(2 * Mathf.PI * u3);
			
			return new Quaternion(x, y, z, w);
		}
		
		/**
		 * Quaternion random from Skillz random.
		 **/
		public static Quaternion Rotation() {
			return RotationUniform();
		}
		
		public static float Range(float min, float max) {
			float rand = Value();
			return min + (rand * (max-min));
		}
		
		public static int Range(int min, int max) {
			float rand = Value();
			return min + (int) (rand * (max-min));
		}
	}
}