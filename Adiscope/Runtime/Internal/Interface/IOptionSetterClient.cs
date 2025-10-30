using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for OptionSetter client
    /// </summary>
    internal interface IOptionSetterClient
    {
        void SetVolumeOff(bool isVolume);
        void SetUseCloudFrontProxy(bool useCloudFrontProxy);

        // Only Android
        void SetChildYN(string childYN);

        // Only iOS
        void SetShowWithLoad2BackgroundColor(string red, string green, string blue, string alpha);
        void SetShowWithLoad2IndicatorStyleMedium(bool isMedium, bool isHidden);
        void SetShowWithLoad2ErrorAlertMsg(string msg, bool isHidden);
        void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup);
        void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup);
        void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting);
        void ShowMaxMediationDebugger();
        void ShowAdmobMediationDebugger();
    }
}
