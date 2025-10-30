/*
 * Created by Minjae Gu (mjgu@neowiz.com)
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

    internal class InterstitialAdClient : IInterstitialAdClient
    {
        public event EventHandler<LoadResult> OnLoaded;
        public event EventHandler<LoadFailure> OnFailedToLoad;
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<LoadResult> OnLoadedBackground;
        public event EventHandler<LoadFailure> OnFailedToLoadBackground;
        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private static InterstitialAdClient Instance;

        public InterstitialAdClient ()
        {
            Instance = this;
        }

#region AD APIs

        [DllImport("__Internal")]
        private static extern void loadInterstitial(
            string unitId,
            onInterstitialAdLoadedCallback loadedCallback,
            onInterstitialAdFailedToLoadCallback failLoadedCallback);
        public void Load(string unitId)
        {
            loadInterstitial(unitId, onInterstitialAdLoaded, onInterstitialAdFailedToLoad);
        }

        [DllImport("__Internal")]
        private static extern bool isLoadedInterstitial(string unitId);
        public bool IsLoaded(string unitId)
        {
            return isLoadedInterstitial(unitId);
        }

        [DllImport("__Internal")]
        private static extern bool showInterstitial(
            onInterstitialWillPresentScreenCallback openedCallback,
            onInterstitialWillDismissScreenCallback closedCallback,
            onInterstitialDidFailToPresentScreenCallback failedToShowCallback);
        public bool Show()
        {
            return showInterstitial(
                onInterstitialWillPresentScreen,
                onInterstitialWillDismissScreen,
                onInterstitialDidFailToPresentScreen);
        }

        [DllImport("__Internal")]
        private static extern void showWithLoadInterstitial(
            string unitId,
            onInterstitialAdLoadedCallback loadedCallback,
            onInterstitialWillPresentScreenCallback openedCallback,
            onInterstitialWillDismissScreenCallback closedCallback,
            onInterstitialDidFailToPresentScreenCallback failedToShowCallback);
        public void ShowWithLoad(string unitId)
        {
            showWithLoadInterstitial(unitId, onInterstitialAdLoaded, onInterstitialWillPresentScreen, onInterstitialWillDismissScreen, onInterstitialDidFailToPresentScreen);
        }
#endregion

#region CallBacks
        private delegate void onInterstitialAdLoadedCallback(string unitId);

        [MonoPInvokeCallback(typeof(onInterstitialAdLoadedCallback))]
        public static void onInterstitialAdLoaded(string unitId)
        {
            Debug.Log("onInterstitialAdLoaded()");
            if (Instance != null)
            {
                Instance.InterstitialAdLoadedProc(unitId);
            }
        }

        public void InterstitialAdLoadedProc(string unitId)
        {
            if (this.OnLoaded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnLoaded(this, new LoadResult(unitId));
                });
            }

            if (this.OnLoadedBackground != null)
            {
                this.OnLoadedBackground(this, new LoadResult(unitId));
            }
        }


        private delegate void onInterstitialAdFailedToLoadCallback(string unitId, int code, string description, string xb3TraceID);
        [MonoPInvokeCallback(typeof(onInterstitialAdFailedToLoadCallback))]
        public static void onInterstitialAdFailedToLoad(string unitId, int code, string description, string xb3TraceID)
        {
            Debug.Log("onInterstitialAdFailedToLoad()");
            if (Instance != null)
            {
                Instance.InterstitialAdFailedToLoadProc(unitId, code, description, xb3TraceID);
            }
        }

       public void InterstitialAdFailedToLoadProc(string unitId, int code, string description, string xb3TraceID)
        {
            if (this.OnFailedToLoad != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToLoad(
                        this,
                        new LoadFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
                });
            }

            if (this.OnFailedToLoadBackground != null)
            {
                this.OnFailedToLoadBackground(
                    Instance,
                    new LoadFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
            }
        }

        private delegate void onInterstitialWillPresentScreenCallback(string unitId);
        [MonoPInvokeCallback(typeof(onInterstitialWillPresentScreenCallback))]
        public static void onInterstitialWillPresentScreen(string unitId)
        {
            Debug.Log("onInterstitialWillPresentScreen()");
            if (Instance != null)
            {
                Instance.InterstitialWillPresentScreen(unitId);
            }
        }

        public void InterstitialWillPresentScreen(string unitId)
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


        private delegate void onInterstitialWillDismissScreenCallback(string unitId);
        [MonoPInvokeCallback(typeof(onInterstitialWillDismissScreenCallback))]
        public static void onInterstitialWillDismissScreen(string unitId)
        {
            Debug.Log("onInterstitialWillDismissScreen()");
            if (Instance != null)
            {
                Instance.InterstitialWillDismissScreen(unitId);
            }
        }

        public void InterstitialWillDismissScreen(string unitId)
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


        private delegate void onInterstitialDidFailToPresentScreenCallback(string unitId, int code, string description, string xb3TraceID);
        [MonoPInvokeCallback(typeof(onInterstitialDidFailToPresentScreenCallback))]
        public static void onInterstitialDidFailToPresentScreen(string unitId, int code, string description, string xb3TraceID)
        {
            Debug.Log("onInterstitialDidFailToPresentScreen()");
            if (Instance != null)
            {
                Instance.InterstitialDidFailToPresentScreen(unitId, code, description, xb3TraceID);
            }
        }

        public void InterstitialDidFailToPresentScreen(string unitId, int code, string description, string xb3TraceID)
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
                Instance.OnFailedToShowBackground(
                    Instance, new ShowFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
            }
        }

#endregion
    }
}
#endif