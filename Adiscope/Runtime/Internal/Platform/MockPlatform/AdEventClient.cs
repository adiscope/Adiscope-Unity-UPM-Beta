/*
 * Created by Sooyeon Jo (sooyeon@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for ad event
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class AdEventClient : IAdEventClient
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private string unitId;
        private bool showing;

        public AdEventClient()
        {
        }

        #region AD APIs 
        public bool Show(string unitId)
        {
            if (this.showing)
            {
                return false;
            }

            this.showing = true;

            this.unitId = unitId;
#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onAdEventOpened, 100)).Start();
            new Thread(() => DelayedCallback(onAdEventClosed, 5000)).Start();
#else
            new Thread(() => DelayedCallback(onAdEventFailedToShow, 5)).Start();
#endif
            return true;
        }
        #endregion

        static void DelayedCallback(Action action, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke();
        }

        #region Callbacks
        public void onAdEventOpened()
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

        public void onAdEventClosed()
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

            this.showing = false;
        }

        public void onAdEventFailedToShow()
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