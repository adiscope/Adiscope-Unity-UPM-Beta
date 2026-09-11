/*
 * Created by Kyungbo shim (kyungbo.shim@neowiz.com)
 */
using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for RewardedInterstitialAd client
    /// </summary>
    internal interface IRewardedInterstitialAdClient
    {
        event EventHandler<UnitStatus> OnGetUnitStatus;
        event EventHandler<LoadResult> OnLoaded;
        event EventHandler<LoadFailure> OnFailedToLoad;
        event EventHandler<ShowResult> OnSkip;
        event EventHandler<ShowResult> OnOpened;
        event EventHandler<ShowResult> OnClosed;
        event EventHandler<RewardItem> OnRewarded;
        event EventHandler<ShowFailure> OnFailedToShow;

        event EventHandler<UnitStatus> OnGetUnitStatusBackground;
        event EventHandler<LoadResult> OnLoadedBackground;
        event EventHandler<LoadFailure> OnFailedToLoadBackground;
        event EventHandler<ShowResult> OnSkipBackground;
        event EventHandler<ShowResult> OnOpenedBackground;
        event EventHandler<ShowResult> OnClosedBackground;
        event EventHandler<RewardItem> OnRewardedBackground;
        event EventHandler<ShowFailure> OnFailedToShowBackground;

        void LoadRewardedInterstitial(string unitId);

        void PreLoadAllRewardedInterstitial();

        void PreLoadRewardedInterstitial(string[] unitIds);

        bool IsLoadedRewardedInterstitial(string unitId);

        bool ShowRewardedInterstitial(string unitId);

        bool ShowWithPopupRewardedInterstitial(string unitId);

        bool GetUnitStatusRewardedInterstitial(string unitId);
    }
}
