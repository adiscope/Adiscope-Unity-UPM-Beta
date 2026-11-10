/*
 * Created by Kyungbo shim (kyungbo.shim@neowiz.com)
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Feature
{
    public class RewardedInterstitialAd
    {
        public event EventHandler<UnitStatus> OnGetUnitStatus;
        public event EventHandler<UnitStatus> OnGetUnitStatusBackground;

        public event EventHandler<LoadResult> OnLoaded;
        public event EventHandler<LoadResult> OnLoadedBackground;

        public event EventHandler<LoadFailure> OnFailedToLoad;
        public event EventHandler<LoadFailure> OnFailedToLoadBackground;

        public event EventHandler<ShowResult> OnSkip;
        public event EventHandler<ShowResult> OnSkipBackground;

        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnOpenedBackground;

        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowResult> OnClosedBackground;

        public event EventHandler<ShowFailure> OnFailedToShow;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        public event EventHandler<RewardItem> OnRewarded;
        public event EventHandler<RewardItem> OnRewardedBackground;
        

        private IRewardedInterstitialAdClient client;

        private static class ClassWrapper { public static readonly RewardedInterstitialAd instance = new RewardedInterstitialAd(); }

        public static RewardedInterstitialAd Instance { get
            {
                RewardedInterstitialAd abcd = ClassWrapper.instance;
                return abcd;
            }
        }

        private RewardedInterstitialAd()
        {
            this.client = ClientBuilder.BuildRewardedInterstitialAdClient();

            this.client.OnGetUnitStatus += (sender, args) => { OnGetUnitStatus?.Invoke(sender, args); };
            this.client.OnGetUnitStatusBackground += (sender, args) => { OnGetUnitStatusBackground?.Invoke(sender, args); };

            this.client.OnLoaded += (sender, args) => { OnLoaded?.Invoke(sender, args); };
            this.client.OnLoadedBackground += (sender, args) => { OnLoadedBackground?.Invoke(sender, args); };

            this.client.OnFailedToLoad += (sender, args) => { OnFailedToLoad?.Invoke(sender, args); };
            this.client.OnFailedToLoadBackground += (sender, args) => { OnFailedToLoadBackground?.Invoke(sender, args); };

            this.client.OnSkip += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnSkip?.Invoke(sender, args);
                };
            this.client.OnSkipBackground += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnSkipBackground?.Invoke(sender, args);
                };

            this.client.OnOpened += (sender, args) => { OnOpened?.Invoke(sender, args); };
            this.client.OnOpenedBackground += (sender, args) => { OnOpenedBackground?.Invoke(sender, args); };

            this.client.OnClosed += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnClosed?.Invoke(sender, args);
                };
            this.client.OnClosedBackground += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnClosedBackground?.Invoke(sender, args);
                };

            this.client.OnFailedToShow += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnFailedToShow?.Invoke(sender, args);
                };
            this.client.OnFailedToShowBackground += (sender, args) => {
                OptionSetter.DismissFullScreen();
                OnFailedToShowBackground?.Invoke(sender, args);
                };

            this.client.OnRewarded += (sender, args) => { OnRewarded?.Invoke(sender, args); };
            this.client.OnRewardedBackground += (sender, args) => { OnRewardedBackground?.Invoke(sender, args); };
        }

        public void LoadRewardedInterstitial(string unitId) { client.LoadRewardedInterstitial(unitId); }

        public void PreLoadAllRewardedInterstitial() { client.PreLoadAllRewardedInterstitial(); }

        public void PreLoadRewardedInterstitial(string[] unitIds) { client.PreLoadRewardedInterstitial(unitIds); }

        public bool IsLoadedRewardedInterstitial(string unitId) { return client.IsLoadedRewardedInterstitial(unitId); }

        public bool ShowRewardedInterstitial(string unitId) {
            // 06.07.15 중복 호출방지 추가(네이티브가 아닌 유니티단에서 처리)
            if(!OptionSetter.canAdShow()) return false;
            OptionSetter.PresentFullscreen();
            return client.ShowRewardedInterstitial(unitId);
        }

        public bool ShowWithPopupRewardedInterstitial(string unitId) { return client.ShowWithPopupRewardedInterstitial(unitId);}

        public bool GetUnitStatusRewardedInterstitial(string unitId) {return client.GetUnitStatusRewardedInterstitial(unitId); }
    }
}
