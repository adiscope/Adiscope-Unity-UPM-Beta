/*
 * Created by mjgu (mjgu@neowiz.com)
 */
#if UNITY_ANDROID

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.Android
{
	internal class OptionGetterClient : IOptionGetterClient
	{
		private AndroidJavaObject activity;

		public OptionGetterClient() {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CoreClient<Constructor> UnityPlayer: null");
                    return;
                }

                this.activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }
		}

		#region APIs 

		public string GetNetworkVersions()
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_NETWORK_VERSIONS);
			}
		}

		private static string getSDKVersion() 
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_SDK_VERSION);
			}
		}

		public string GetSDKVersion() 
		{
			return getSDKVersion();
		}

		public string GetUnitySDKVersion() 
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return "";
                }
				
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_UNITY_SDK_VERSION, this.activity);
			}
		}

		public string GetInitializeFailLog() 
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_INITIALIZE_FAIL_LOG);
			}
		}

		#endregion
	}
}

#endif