/*
 * Created by Sooyeon Jo (sooyeon@neowiz.com)
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Feature
{
    public class AdEvent
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnOpenedBackground;

        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowResult> OnClosedBackground;

        public event EventHandler<ShowFailure> OnFailedToShow;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;
        
        private IAdEventClient client;

        private static class ClassWrapper { public static readonly AdEvent instance = new AdEvent(); }
        public static AdEvent Instance { get { return ClassWrapper.instance; } }

        private AdEvent()
        {
            this.client = ClientBuilder.BuildAdEventClient();

            this.client.OnOpened += (sender, args) => { OnOpened?.Invoke(sender, args); };
            this.client.OnOpenedBackground += (sender, args) => { OnOpenedBackground?.Invoke(sender, args); };

            this.client.OnClosed += (sender, args) => { OnClosed?.Invoke(sender, args); };
            this.client.OnClosedBackground += (sender, args) => { OnClosedBackground?.Invoke(sender, args); };

            this.client.OnFailedToShowBackground += (sender, args) => { OnFailedToShowBackground?.Invoke(sender, args); };
            this.client.OnFailedToShow += (sender, args) => { OnFailedToShow?.Invoke(sender, args); };
        }

        public bool Show(string unitId) { return client.Show(unitId); }
    }
}
