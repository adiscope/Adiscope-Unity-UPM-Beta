/*
 * Created by Kyungbo shim (kyungbo.shim@neowiz.com)
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
	/// iOS client for rewarded video ad
	/// this class will call iOS native plugin's method
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

		public static RewardedInterstitialAdClient Instance;

		public RewardedInterstitialAdClient ()
		{
			Instance = this;
		}

		#region AD APIs 
		[DllImport ("__Internal")]
		private static extern void preLoadAllRewardedInterstitial();

		public void PreLoadAllRewardedInterstitial()
		{
			Debug.Log("PreLoadAllRewardedInterstitial");
			preLoadAllRewardedInterstitial();
		}

		[DllImport ("__Internal")]
		private static extern void preLoadRewardedInterstitial(string[] unitIds);

		public void PreLoadRewardedInterstitial(string[] unitIds)
		{
			Debug.Log("PreLoadRewardedInterstitial : " + unitIds);
			preLoadRewardedInterstitial(unitIds);
		}

		[DllImport ("__Internal")]
		private static extern bool showRewardedInterstitial(string unitId, onRewardedInterstitialAdOpenedCallback openedCallback, onRewardedInterstitialAdClosedCallback closedCallback, 
			onRewardedInterstitialRewardedCallback rewardedCallback, onRewardedInterstitialAdFailedToShowCallback failedToShowCallback, onRewardedInterstitialAdSkipCallback skipCallback);

		public bool ShowRewardedInterstitial(string unitId)
		{
			Debug.Log("ShowRewardedInterstitial : " + unitId);
			return showRewardedInterstitial(unitId, onRewardedInterstitialAdOpened, onRewardedInterstitialAdClosed, onRewardedInterstitialRewarded, onRewardedInterstitialAdFailedToShow, onRewardedInterstitialAdSkip);
		}

		[DllImport ("__Internal")]
		private static extern bool getUnitStatusRewardedInterstitial(string unitId, onRewardedInterstitialGetUnitStatusCallback skipCallback);

		public bool GetUnitStatusRewardedInterstitial(string unitId)
		{
			Debug.Log("GetUnitStatusRewardedInterstitial : " + unitId);
			return getUnitStatusRewardedInterstitial(unitId, onRewardedInterstitialGetUnitStatus);
		}

		#endregion

		#region Callbacks

		private delegate void onRewardedInterstitialGetUnitStatusCallback(string description, bool live, bool active);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialGetUnitStatusCallback))] 
		public static void onRewardedInterstitialGetUnitStatus(string description, bool live, bool active)
		{
            if (Instance != null)
            {
				Instance.RewardedInterstitialGetUnitStatusProc(description, live, active);
			}
		}

		private void RewardedInterstitialGetUnitStatusProc(string description, bool live, bool active) 
		{
			if (this.OnGetUnitStatus != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					this.OnGetUnitStatus(this, new UnitStatus(live, active));
				});
			}

			if (this.OnGetUnitStatusBackground != null)
			{
				this.OnGetUnitStatusBackground (this, new UnitStatus(live, active));
			}
		}

		private delegate void onRewardedInterstitialAdSkipCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialAdSkipCallback))] 
		public static void onRewardedInterstitialAdSkip(string unitId)
		{
            if (Instance != null)
            {
				Instance.RewardedInterstitialAdSkipProc(unitId);
			}
		}

		private void RewardedInterstitialAdSkipProc(string unitId)
		{
			if (this.OnSkip != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					this.OnSkip(
						this, new ShowResult(unitId));
				});
			}

			if (this.OnSkipBackground != null)
			{
				this.OnSkipBackground(
					Instance, new ShowResult(unitId));
			}
		}

		private delegate void onRewardedInterstitialAdOpenedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialAdOpenedCallback))] 
		public static void onRewardedInterstitialAdOpened(string unitId)
		{
			Debug.Log("onRewardedInterstitialAdOpened() unitId = " + unitId);
            if (Instance != null)
            {
				Instance.RewardedInterstitialAdOpenedProc (unitId);
			}
		}

		private void RewardedInterstitialAdOpenedProc(string unitId)
		{
			if (Instance.OnOpened != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnOpened(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnOpenedBackground != null)
			{
				Instance.OnOpenedBackground(Instance, new ShowResult(unitId));
			}
		}

		private delegate void onRewardedInterstitialAdClosedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialAdClosedCallback))] 
		public static void onRewardedInterstitialAdClosed(string unitId)
		{
			Debug.Log("onRewardedInterstitialAdClosed() unitId = " + unitId);
			string nullSafetyUnitID = unitId;
			if (nullSafetyUnitID == null) { nullSafetyUnitID = ""; }
            if (Instance != null)
            {
				Instance.RewardedInterstitialAdClosedProc (unitId);
			}
		}

		private void RewardedInterstitialAdClosedProc(string unitId)
		{
			if (Instance.OnClosed != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnClosed(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnClosedBackground != null)
			{
				Instance.OnClosedBackground(Instance, new ShowResult(unitId));
			}
		}

		private delegate void onRewardedInterstitialRewardedCallback(string unitId, string type, long amount);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialRewardedCallback))]
		public static void onRewardedInterstitialRewarded(string unitId, string type, long amount)
		{
			Debug.Log("onRewardedInterstitialRewarded() unitId = " + unitId + " type = " + type + " amount = " + amount);
            if (Instance != null)
            {
				Instance.RewardedInterstitialRewardedProc (unitId, type, amount);
			}
		}

		private void RewardedInterstitialRewardedProc(string unitId, string type, long amount)
		{
			if (Instance.OnRewarded != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnRewarded(Instance, new RewardItem(unitId, type, amount));
				});
			}

			if (Instance.OnRewardedBackground != null)
			{
				Instance.OnRewardedBackground(Instance, new RewardItem(unitId, type, amount));
			}		
		}

		private delegate void onRewardedInterstitialAdFailedToShowCallback(string unitId, int code, string description, string xb3TraceID);

		[MonoPInvokeCallback(typeof(onRewardedInterstitialAdFailedToShowCallback))]
		public static void onRewardedInterstitialAdFailedToShow(string unitId, int code, string description, string xb3TraceID)
		{
            if (Instance != null)
            {
				Instance.RewardedInterstitialAdFailedToShowProc (unitId, code, description, xb3TraceID);
			}
		}

		private void RewardedInterstitialAdFailedToShowProc(string unitId, int code, string description, string xb3TraceID)
		{
			if (Instance.OnFailedToShow != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnFailedToShow(
						Instance, new ShowFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
				});
			}

			if (Instance.OnFailedToShowBackground != null)
			{
				Instance.OnFailedToShowBackground (
					Instance, new ShowFailure(unitId, new AdiscopeError (code, description, xb3TraceID)));
			}
		}

		#endregion
	}
}

#endif