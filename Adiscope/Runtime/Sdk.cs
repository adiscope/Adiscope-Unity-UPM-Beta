/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Feature;
using Adiscope.Internal;
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model;
using System;

namespace Adiscope
{
    /// <summary>
    /// adiscope core class
    /// this class must be used to get rewarded video ad or offerwall ad instance.
    /// </summary>
    public class Sdk
    {
        /// <summary>
        /// prevent manual construction
        /// </summary>
        private Sdk()
        {
        }

        /// <summary>
        /// get Core singleton instance
        /// </summary>
        /// <returns>singleton instance of Adiscope Core</returns>
        public static Core GetCoreInstance()
        {
            return Core.Instance;
        }

        /// <summary>
        /// get rewarded video ad singleton instance
        /// </summary>
        /// <returns>singleton instance of RewardedVideoAd</returns>
        public static RewardedVideoAd GetRewardedVideoAdInstance()
        {
            return RewardedVideoAd.Instance;
        }

        /// <summary>
        /// get offerwall ad singleton instance
        /// </summary>
        /// <returns>singleton instance of OfferwallAd</returns>
        public static OfferwallAd GetOfferwallAdInstance()
        {
            return OfferwallAd.Instance;
        }

        /// <summary>
        /// get interstitial ad singleton instance
        /// </summary>
        /// <returns>singleton instance of InterstitialAd</returns>
        public static InterstitialAd GetInterstitialAdInstance()
        {
            return InterstitialAd.Instance;
        }

        /// <summary>
        /// get rewarded interstitial video ad singleton instance
        /// </summary>
        /// <returns>singleton instance of RewardedInterstitialVideoAd</returns>
        public static RewardedInterstitialAd GetRewardedInterstitialAdInstance()
        {
            return RewardedInterstitialAd.Instance;
        }

        /// <summary>
        /// get ad event singleton instance
        /// </summary>
        /// <returns>singleton instance of AdEvent</returns>
        public static AdEvent GetAdEventInstance()
        {
            return AdEvent.Instance;
        }

        /// <summary>
        /// get option setter singleton instance
        /// </summary>
        /// <returns>singleton instance of option setter</returns>
        public static OptionSetter GetOptionSetter()
        {
            return OptionSetter.Instance;
        }

        public static OptionGetter GetOptionGetter()
        {
            return OptionGetter.Instance;
        }
    }
}
