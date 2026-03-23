/*
 * Created by Hyunchang Kim (mjgu@neowiz.com)
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Internal;
using Adiscope.Model;
using System;

namespace Adiscope.Feature
{
    public class Core
    {
        private readonly ICoreClient client;

        private static class ClassWrapper { public static readonly Core instance = new Core(); }

        public static Core Instance { get { return ClassWrapper.instance; } }

        private Core() { this.client = ClientBuilder.BuildCoreClient(); }


        public void Initialize(Action<bool> callback = null)
        {
            this.client.Initialize(callback);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(Action<bool> callback = null, string callbackTag = "")
        {
            this.client.Initialize(callback, callbackTag);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(Action<bool> callback = null, string callbackTag = "", string childYN = "")
        {
            this.client.Initialize(callback, callbackTag, childYN);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag = "", Action<bool> callback = null)
        {
            Initialize(mediaId, mediaSecret, callbackTag, "", callback);
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag = "", string childYN = "", Action<bool> callback = null)
        {
            this.client.Initialize(mediaId, mediaSecret, callbackTag, childYN, callback);
            UnityThread.initUnityThread(true);
        }

        public void InitializeTest(string mediaId, string mediaSecret, string callbackTag = "", Action<bool> callback = null)
        {
            InitializeTest(mediaId, mediaSecret, callbackTag, "", callback);
        }

        public void InitializeTest(string mediaId, string mediaSecret, string callbackTag = "", string childYN = "", Action<bool> callback = null)
        {
            this.client.InitializeTest(mediaId, mediaSecret, callbackTag, childYN, callback);
            UnityThread.initUnityThread(true);
        }

        public bool IsInitialized() { return this.client.IsInitialized(); }

        public void SetUserId(string userId) { this.client.SetUserId(userId); }

        public void SetUserIdChild(string userId, int child) { this.client.SetUserIdChild(userId, child); }

        public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback) { this.client.GetUnitStatus(unitId, callback); }

        public void SetRewardedCheckParam(string param) { this.client.SetRewardedCheckParam(param); }


        public void SetLuckyEventAppId(string eventId, string pid)
        {
            this.client.SetLuckyEventAppId(eventId, pid);
            UnityThread.initUnityThread(true);
        }
        
        public void SetLuckyEventUseSafeAreaWebView(bool useSafeArea)
        {
            this.client.SetLuckyEventUseSafeAreaWebView(useSafeArea);
            UnityThread.initUnityThread(true);
        }

        public void SetLuckyEventHashMark(string hashMark)
        {
            this.client.SetLuckyEventHashMark(hashMark);
            UnityThread.initUnityThread(true);
        }

        public void SetLuckyEventBaseUrl(string baseUrl)
        {
            this.client.SetLuckyEventBaseUrl(baseUrl);
            UnityThread.initUnityThread(true);
        }

        public void SetLuckyEventExtraParam(string key, string value)
        {
            this.client.SetLuckyEventExtraParam(key, value);
            UnityThread.initUnityThread(true);
        }

        public void ShowLuckyEvent()
        {
            this.client.ShowLuckyEvent();
            UnityThread.initUnityThread(true);
        }
    }
}
