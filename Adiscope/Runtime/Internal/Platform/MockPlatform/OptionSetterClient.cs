#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for option setter
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class OptionSetterClient : IOptionSetterClient
    {

        public OptionSetterClient()
        {
        }

        #region APIs 
        public void SetUseCloudFrontProxy(bool useCloudFrontProxy)
        {
        }

        public void SetChildYN(string childYN)
        {
        }

        public void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup)
        {
        }

        public void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup)
        {
        }

        public void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting)
        {
        }

        public void ShowMaxMediationDebugger()
        {
        }

        public void SetVolumeOff(bool isVolume)
        {
        }

        public void ShowAdmobMediationDebugger()
        {
        }
                
        public void SetShowWithLoad2BackgroundColor(string red, string green, string blue, string alpha)
        { 
        }

        public void SetShowWithLoad2IndicatorStyleMedium(bool isMedium, bool isHidden)
        {
        }

        public void SetShowWithLoad2ErrorAlertMsg(string msg, bool isHidden)
        {
        }
        #endregion
    }
}
#endif