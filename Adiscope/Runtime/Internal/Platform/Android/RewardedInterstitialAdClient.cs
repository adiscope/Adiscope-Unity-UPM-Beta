/*
 * Created by Kyungbo shim (kyungbo.shim@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for rewarded Interstitial ad
    /// this class will call android native plugin's method
    /// </summary>
    internal class RewardedInterstitialAdClient : AndroidJavaProxy, IRewardedInterstitialAdClient
    {
		public event EventHandler<UnitStatus> OnGetUnitStatus;
		public event EventHandler<ShowResult> OnSkip;
		public event EventHandler<ShowResult> OnOpened;
		public event EventHandler<ShowResult> OnClosed;
		public event EventHandler<RewardItem> OnRewarded;
		public event EventHandler<ShowFailure> OnFailedToShow;

		public event EventHandler<UnitStatus> OnGetUnitStatusBackground;
		public event EventHandler<ShowResult> OnSkipBackground;
		public event EventHandler<ShowResult> OnOpenedBackground;
		public event EventHandler<ShowResult> OnClosedBackground;
		public event EventHandler<RewardItem> OnRewardedBackground;
		public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private AndroidJavaObject rewardedInterstitialAd;

        public RewardedInterstitialAdClient(): base(Values.PKG_REWARDED_INTERSTITIAL_AD_LISTENER)
        {
            this.rewardedInterstitialAd = GetRewardedInterstitialAdInstance();

            if (this.rewardedInterstitialAd == null)
            {
                Debug.LogError("Android.RewardedInterstitialAdClient<Constructor> RewardedInterstitialAd: null");
                return;
            }
            
            this.rewardedInterstitialAd.Call(Values.MTD_SET_REWARDED_INTERSTITIAL_AD_LISTENER, this);
        }

        private AndroidJavaObject GetRewardedInterstitialAdInstance()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.RewardedInterstitialAdClient<Constructor> UnityPlayer: null");
                    return null;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.RewardedInterstitialAdClient<GetRewardedInterstitialAdClient> " +
                        Values.PKG_ADISCOPE + ": null");
                    return null;
                }

                AndroidJavaObject rewardedInterstitialAd = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_REWARDED_INTERSTITIAL_AD_INSTANCE, activity);
                return rewardedInterstitialAd;
            }
        }

        public void PreLoadAllRewardedInterstitial()
        {
            if (rewardedInterstitialAd == null)
            {
                Debug.LogError("Android.RewardedInterstitialAdClient<PreLoadAll> RewardedInterstitialAd: null");
                return;
            }

            rewardedInterstitialAd.Call(Values.MTD_REWARDED_INTERSTITIAL_LOAD_ALL);
        }

        public void PreLoadRewardedInterstitial(string[] unitIds)
        {
            if (rewardedInterstitialAd == null)
            {
                Debug.LogError("Android.RewardedInterstitialAdClient<PreLoad> RewardedInterstitialAd: null");
                return;
            }

            rewardedInterstitialAd.Call(Values.MTD_REWARDED_INTERSTITIAL_LOAD, unitIds);
        }

        public bool ShowRewardedInterstitial(string unitId)
        {
            if (rewardedInterstitialAd == null)
            {
                Debug.LogError("Android.RewardedInterstitialAdClient<Show> RewardedInterstitialAd: null");
                return false;
            }

            rewardedInterstitialAd.Call(Values.MTD_SHOW, unitId);
            return true;
        }

        public bool GetUnitStatusRewardedInterstitial(string unitId)
        {
            AdiscopeError unitStatusError = null;
            bool status = false;

            if (rewardedInterstitialAd == null)
            {
                Debug.LogError("Android.RewardedInterstitialAdClient<GetUnitStatus> RewardedInterstitialAd: null");
                return false;
            }

            GetUnitStatusRewardedInterstitial(unitId, (error, unitStatus) => {
                if(error != null) unitStatusError = error;
                status = unitStatus.Live && unitStatus.Active;
                onRewardedInterstitialGetUnitStatus(unitId, unitStatus.Live, unitStatus.Active);
            });

            if(unitStatusError != null) return false;

            return status;
        }

        private void GetUnitStatusRewardedInterstitial(string unitId, Action<AdiscopeError, UnitStatus> callback){
            IUnitStatus status = new IUnitStatus(callback);
            rewardedInterstitialAd.Call(Values.MTD_GET_UNIT_STATUS, unitId, status);
        }

#region Callbacks
        public void onRewardedInterstitialGetUnitStatus(string unitId, bool isLive, bool isActive)
        {
            if (this.OnGetUnitStatus != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnGetUnitStatus(this, new UnitStatus(isLive, isActive));
                });
            }

            if (this.OnGetUnitStatusBackground != null)
            {
                this.OnGetUnitStatusBackground(this, new UnitStatus(isLive, isActive));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedInterstitialAdSkipped(string unitId)
        {
            if (this.OnSkip != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnSkip(this, new ShowResult(unitId));
                });
            }

            if (this.OnSkipBackground != null)
            {
                this.OnSkipBackground(this, new ShowResult(unitId));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedInterstitialAdOpened(string unitId)
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
        public void onRewardedInterstitialAdClosed(string unitId)
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
        public void onRewardedInterstitialAdRewarded(string unitId, AndroidJavaObject rewardItem)
        {
            if (this.OnRewarded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnRewarded(this, Utils.ConvertToRewardItem(unitId, rewardItem));
                });
            }

            if (this.OnRewardedBackground != null)
            {
                this.OnRewardedBackground(this, Utils.ConvertToRewardItem(unitId, rewardItem));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedInterstitialAdFailedToShow(string unitId, AndroidJavaObject error)
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