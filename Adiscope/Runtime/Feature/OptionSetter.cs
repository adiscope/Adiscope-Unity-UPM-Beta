using Adiscope.Internal;
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Adiscope.Feature
{
    /// <summary>
    /// OptionSetter singleton instance class
    /// </summary>
    public class OptionSetter
    {
        private IOptionSetterClient client;

        private static class InitializationOnDemandHolderIdiom
        {
            public static readonly OptionSetter SingletonInstance = new OptionSetter();
        }

        public static OptionSetter Instance
        {
            get
            {
                return InitializationOnDemandHolderIdiom.SingletonInstance;
            }
        }

        private OptionSetter()
        {
            this.client = ClientBuilder.BuildOptionSetterClient();
        }

        public void SetVolumeOff(bool isVolume) {
            client.SetVolumeOff(isVolume);
        }

        /// <summary>
        /// Set whether background is color, Only Using for iOS.
        /// </summary>
        public void SetShowWithLoad2BackgroundColor(string red, string green, string blue, string alpha) {
            client.SetShowWithLoad2BackgroundColor(red, green, blue, alpha);
        }

        /// <summary>
        /// Set whether indicator is style, Only Using for iOS.
        /// </summary>
        public void SetShowWithLoad2IndicatorStyleMedium(bool isMedium, bool isHidden) {
            client.SetShowWithLoad2IndicatorStyleMedium(isMedium, isHidden);
        }

        /// <summary>
        /// Set whether alert is style, Only Using for iOS.
        /// </summary>
        public void SetShowWithLoad2ErrorAlertMsg(string msg, bool isHidden) {
            client.SetShowWithLoad2ErrorAlertMsg(msg, isHidden);
        }

        /// <summary>
        /// Set whether to use Cloud Front Proxy or not
        /// </summary>
        public void SetUseCloudFrontProxy(bool useCloudFrontProxy)
        {
            client.SetUseCloudFrontProxy(useCloudFrontProxy);
        }

        /// <summary>
        /// Set whether user is child, Only Using for Android.
        /// </summary>
        public void SetChildYN(string childYN)
        {
            client.SetChildYN(childYN);
        }

        /// <summary>
        /// Setup the warning popup Flag in Offerwall. Only Using for iOS.
        /// </summary>
        public void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup)
        {
            client.SetUseAppTrackingTransparencyPopup(useOfferwallWarningPopup);
        }

        /// <summary>
        /// Setup the ATT popup Flag in Adiscope. Only Using for iOS.
        /// </summary>
        public void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup)
        {
            client.SetUseAppTrackingTransparencyPopup(useAppTrackingTransparencyPopup);
        }

        /// <summary>
        /// Setup the Flag: use Jump to Adiscope in Setting App. Only Using for iOS.
        /// </summary>
        public void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting)
        {
            client.SetUseAppTrackingTransparencyPopup(enabledForcedOpenApplicationSetting);
        }

        public void ShowMaxMediationDebugger()
        {
            client.ShowMaxMediationDebugger();
        }

        public void ShowAdmobMediationDebugger()
        {
            client.ShowAdmobMediationDebugger();
        }

        // ---- Unity pause / ad show guard ----

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void UnityPause(int pause);
        [DllImport("__Internal")]
        private static extern int UnityIsPaused();
#endif

        private static bool _pauseOnBackground = false;
        private static bool _blockMultiCallShow = false;
        private static long _showStartTimeMs = 0;
        private const long BLOCK_DURATION_MS = 2 * 60 * 1000;

        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            _pauseOnBackground = pause;
        }

        public static void BlockMultiAdShow(bool block)
        {
            _blockMultiCallShow = block;
        }

        public static bool IsPaused()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return UnityIsPaused() == 1;
#else
            return Time.timeScale == 0f;
#endif
        }

        public static bool canAdShow()
        {
            if (_blockMultiCallShow && _showStartTimeMs > 0)
            {
                long elapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _showStartTimeMs;
                if (elapsed < BLOCK_DURATION_MS)
                    return false;
            }
            return true;
        }

        public static void PresentFullscreen()
        {
            _showStartTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (_pauseOnBackground) Pause();
        }

        public static void DismissFullScreen()
        {
            _showStartTimeMs = 0;
            if (IsPaused()) Resume();
        }

        public static void Pause()
        {
#if UNITY_IOS && !UNITY_EDITOR
            UnityPause(1);
#elif UNITY_ANDROID && !UNITY_EDITOR
            // using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            // using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            // {
            //     activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            //     {
            //         player.CallStatic("pause");
            //     }));
            // }
#else
//            Time.timeScale = 0f;
//            AudioListener.pause = true;
#endif
        }

        public static void Resume()
        {
#if UNITY_IOS && !UNITY_EDITOR
            UnityPause(0);
#elif UNITY_ANDROID && !UNITY_EDITOR
            // using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            // {
            //     player.CallStatic("resume");
            // }
#else
//            Time.timeScale = 1f;
//            AudioListener.pause = false;
#endif
        }
    }
}
