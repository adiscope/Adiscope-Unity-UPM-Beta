/*
 * Created by Sooyeon Jo (sooyeon@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for ad event
    /// this class will call android native plugin's method
    /// </summary>
    internal class AdEventClient : AndroidJavaProxy, IAdEventClient
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private AndroidJavaObject adEvent;

        public AdEventClient() : base(Values.PKG_AD_EVENT_LISTENER)
        {
            this.adEvent = GetAdEventInstance();

            if (this.adEvent == null)
            {
                Debug.LogError("Android.AdEventClient<Constructor> AdEvent: null");
                return;
            }

            this.adEvent.Call(Values.MTD_SET_AD_EVENT_LISTENER, this);
        }

        private AndroidJavaObject GetAdEventInstance()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.AdEventClient<Constructor> UnityPlayer: null");
                    return null;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.AdEventClient<GetAdEventClient> " +
                        Values.PKG_ADISCOPE + ": null");
                    return null;
                }

                AndroidJavaObject adEvent = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_AD_EVENT_INSTANCE, activity);

                return adEvent;
            }
        }

        public bool Show(string unitId)
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.AdEventClient<Show> UnityPlayer: null");
                    return false;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (adEvent == null)
            {
                Debug.LogError("Android.AdEventClient<Show> AdEvent: null");
                return false;
            }
            return adEvent.Call<bool>(Values.MTD_SHOW, activity, unitId);

        }

        #region Callbacks
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onAdEventOpened(string unitId)
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new ShowResult(unitId));
                });
            }

            if (this.OnOpenedBackground != null)
            {
                this.OnOpenedBackground(this, new ShowResult(unitId));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onAdEventClosed(string unitId)
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new ShowResult(unitId));
                });
            }

            if (this.OnClosedBackground != null)
            {
                this.OnClosedBackground(this, new ShowResult(unitId));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onAdEventFailedToShow(string unitId, AndroidJavaObject error)
        {
            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(
                        this, new ShowFailure(unitId, Utils.ConvertToAdiscopeError(error)));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(
                    this, new ShowFailure(unitId, Utils.ConvertToAdiscopeError(error)));
            }
        }
        #endregion
    }
}
#endif