/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for rewarded Interstitial ad
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class RewardedInterstitialAdClient : IRewardedInterstitialAdClient
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

        private bool loaded;
        private string unitId;
        private string[] unitIds;

        public RewardedInterstitialAdClient()
        {
        }

        #region AD APIs 

        public void PreLoadAllRewardedInterstitial()
        {
// #if UNITY_EDITOR
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdLoaded, 1000)).Start();
// #else
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdFailedToLoad, 5)).Start();
// #endif
        }

        public void PreLoadRewardedInterstitial(string[] unitIds)
        {
//             this.unitIds = unitIds;
// #if UNITY_EDITOR
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdLoaded, 1000)).Start();
// #else
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdFailedToLoad, 5)).Start();
// #endif
        }

        public bool ShowRewardedInterstitial(string unitId)
        {
//             this.unitId = unitId;

// #if UNITY_EDITOR
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdLoaded, 1000)).Start();
// #else
//             new Thread(() => DelayedCallback(onRewardedInterstitialAdFailedToLoad, 5)).Start();
// #endif
            return true;
        }

        public bool GetUnitStatusRewardedInterstitial(string unitId)
        {
//             this.unitId = unitId;

// #if UNITY_EDITOR
//             new Thread(() => DelayedCallback(onRewardedInterstitialGetUnitStatus, 1000)).Start();
// #else
//             new Thread(() => DelayedCallback(onRewardedInterstitialGetUnitStatus, 5)).Start();
// #endif
            return true;
        }
        #endregion

        private void DelayedCallback(Action action, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke();
        }

        #region Callbacks

        public void onRewardedInterstitialGetUnitStatus()
        {
            if (this.OnGetUnitStatus != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnGetUnitStatus(this, new UnitStatus(true, true));
                });
            }

            if (this.OnGetUnitStatusBackground != null)
            {
                this.OnGetUnitStatusBackground(this, new UnitStatus(true, true));
            }
        }

        public void onRewardedInterstitialAdFailedToLoad()
        {
            AdiscopeError error = new AdiscopeError(-1, "Adiscope only supports following platforms: Android");

            if (this.OnSkip != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnSkip(this, new ShowResult(this.unitId));
                });
            }

            if (this.OnSkipBackground != null)
            {
                this.OnSkipBackground(this, new ShowResult(this.unitId));
            }
        }

        public void onRewardedInterstitialAdOpened()
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new ShowResult(this.unitId));
                });
            }

            if (this.OnOpenedBackground != null)
            {
                this.OnOpenedBackground(this, new ShowResult(this.unitId));
            }
        }

        public void onRewardedInterstitialAdClosed()
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new ShowResult(this.unitId));
                });
            }

            if (this.OnClosedBackground != null)
            {
                this.OnClosedBackground(this, new ShowResult(this.unitId));
            }
        }

        public void onRewarded()
        {
            RewardItem param = new RewardItem(this.unitId, "sample", 0);

            if (this.OnRewarded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnRewarded(this, param);
                });
            }

            if (this.OnRewardedBackground != null)
            {
                this.OnRewardedBackground(this, param);
            }
        }

        public void onRewardedInterstitialAdFailedToShow()
        {
            AdiscopeError error = new AdiscopeError(6, "No more ads to show");

            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new ShowFailure(this.unitId, error));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(this, new ShowFailure(this.unitId, error));
            }
        }

        public void onRewardedInterstitialAdFailedToShowUnsupported()
        {
            AdiscopeError error = new AdiscopeError(-1, "Adiscope only supports following platforms: Android");

            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new ShowFailure(this.unitId, error));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(this, new ShowFailure(this.unitId, error));
            }
        }
#endregion
    }
}
#endif