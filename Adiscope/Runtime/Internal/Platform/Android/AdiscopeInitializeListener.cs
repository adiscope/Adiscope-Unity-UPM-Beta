#if UNITY_ANDROID
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    public class AdiscopeInitializeListener : AndroidJavaProxy
    {
        Action<bool> callback;

        public AdiscopeInitializeListener(Action<Boolean> callback) : base("com.nps.adiscope.listener.AdiscopeInitializeListener")
        {
            this.callback = callback;
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        void onInitialized(bool isSuccess)
        {
            Debug.Log("onInitialized : " + isSuccess);
            if (!isSuccess)
            {
                AndroidJavaObject activity = null;
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
                {
                    if (unityPlayer == null)
                    {
                        Debug.LogError("Android.InterstitialAdClient<Constructor> UnityPlayer: null");
                        return;
                    }
                    activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
                }

                using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
                {
                    if (jc == null)
                    {
                        Debug.LogError("Android.InterstitialAdClient<GetInterstitialAdClient> " +
                            Values.PKG_ADISCOPE + ": null");
                        return;
                    }

                    jc.CallStatic(Values.MTD_SEND_CURRENT_ACTIVITY, activity);
                }
            }
            callback.Invoke(isSuccess);
        }
    }
}
#endif