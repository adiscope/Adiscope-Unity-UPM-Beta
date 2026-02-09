/*
 * Created by Sunhak Lee (shlee@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS
{
	/// <summary>
	/// iOS client for option setter
	/// this class will call iOS native plugin's method
	/// </summary>	
	internal class OptionSetterClient : IOptionSetterClient
	{
		public OptionSetterClient ()
		{
		}

		#region APIs 

		[DllImport ("__Internal")]
		private static extern void setUseCloudFrontProxy(bool useCloudFrontProxy);
		public void SetUseCloudFrontProxy(bool useCloudFrontProxy)
		{
			setUseCloudFrontProxy (useCloudFrontProxy);
		}

		public void SetChildYN(string childYN)
		{
		    // nothing	
		}

		[DllImport("__Internal")]
		private static extern void setUseOfferwallWarningPopup(bool useOfferwallWarningPopup);
		public void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup)
        {
			setUseOfferwallWarningPopup(useOfferwallWarningPopup);
        }

		[DllImport("__Internal")]
		private static extern void setUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup);
		public void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup)
        {
			setUseAppTrackingTransparencyPopup(useAppTrackingTransparencyPopup);
		}

		[DllImport("__Internal")]
		private static extern void setEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting);
		public void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting)
        {
			setEnabledForcedOpenApplicationSetting(enabledForcedOpenApplicationSetting);
		}

		[DllImport ("__Internal")]
		private static extern void showMaxMediationDebugger();
		public void ShowMaxMediationDebugger() {
			showMaxMediationDebugger();
		}

        [DllImport("__Internal")]
        private static extern void setVolumeOff(bool isVolume);
        public void SetVolumeOff(bool isVolume) 
        {
            setVolumeOff(isVolume);
        }

		public void ShowAdmobMediationDebugger() {
		}
		
		[DllImport("__Internal")]
		private static extern void setShowWithLoad2BackgroundColor(string red, string green, string blue, string alpha);
        public void SetShowWithLoad2BackgroundColor(string red, string green, string blue, string alpha) {
			setShowWithLoad2BackgroundColor(red, green, blue, alpha);
        }

		[DllImport("__Internal")]
		private static extern void setShowWithLoad2IndicatorStyleMedium(bool isMedium, bool isHidden);
        public void SetShowWithLoad2IndicatorStyleMedium(bool isMedium, bool isHidden) {
            setShowWithLoad2IndicatorStyleMedium(isMedium, isHidden);
        }

		[DllImport("__Internal")]
		private static extern void setShowWithLoad2ErrorAlertMsg(string msg, bool isHidden);
        public void SetShowWithLoad2ErrorAlertMsg(string msg, bool isHidden) {
            setShowWithLoad2ErrorAlertMsg(msg, isHidden);
        }

		#endregion
	}
}

#endif