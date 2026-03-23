/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for adiscope core
    /// this class will call android native plugin's method
    /// </summary>
    internal class CoreClient : ICoreClient
    {
        private AndroidJavaObject activity;

        // Lucky Event builder 호출 시 사용할 캐시 값
        // _luckyEventChildYn은 OptionSetterClient.SetChildYN에서도 갱신
        internal static string _luckyEventUserId = "";
        internal static string _luckyEventChildYn = "NO";
        private string _luckyEventId = "";
        private string _luckyEventPid = "";
        private bool _luckyEventUseSafeArea = false;
        private string _luckyEventHashMark = "";
        private string _luckyEventBaseUrl = "";
        private readonly Dictionary<string, string> _luckyEventExtraParams = new Dictionary<string, string>();
             
        public CoreClient()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CoreClient<Constructor> UnityPlayer: null");
                    return;
                }

                this.activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }
        }

        public void Initialize(Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener);
            }
        }

        public void Initialize(Action<bool> callback, string callbackTag)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener, callbackTag);
            }
        }

        public void Initialize(Action<bool> callback, string callbackTag, string childYN)
        {
            _luckyEventChildYn = childYN;

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener, callbackTag, childYN);
            }
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag, Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, mediaId, mediaSecret, callbackTag, listener);
            }
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback)
        {
            _luckyEventChildYn = childYN;

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, mediaId, mediaSecret, callbackTag, childYN, listener);
            }
        }

        public void InitializeTest(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }

                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, null, mediaId, mediaSecret, callbackTag, childYN, listener);
            }
        }

        public void SetUserId(string userId)
        {
            _luckyEventUserId = userId;

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<SetUserId> " + Values.PKG_ADISCOPE + ": null");
                    return;
                }

                jc.CallStatic<bool>(Values.MTD_SET_USER_ID, userId);
            }
        }

        public void SetUserIdChild(string userId, int child)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<SetUserIdChild> " + Values.PKG_ADISCOPE + ": null");
                    return;
                }

                using (AndroidJavaClass userTypeClass = new AndroidJavaClass(Values.PKG_ADISCOPE_USER_TYPE))
                {
                    if (userTypeClass == null)
                    {
                        Debug.LogError("Android.CoreClient<SetUserIdChild> " + Values.PKG_ADISCOPE_USER_TYPE + ": null");
                        return;
                    }

                    AndroidJavaObject userType = userTypeClass.CallStatic<AndroidJavaObject>(Values.MTD_FROM_INT, child);
                    _luckyEventUserId = userId;
                    _luckyEventChildYn = userType.Call<string>(Values.MTD_GET_CHILD_YN);
                    jc.CallStatic<bool>(Values.MTD_SET_USER_ID_CHILD, userId, userType);
                }
            }
        }

        public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<GetUnitStatus> " + Values.PKG_ADISCOPE + ": null");
                }

                IUnitStatus status = new IUnitStatus(callback);
                jc.CallStatic(Values.MTD_GET_UNIT_STATUS, unitId, status);
            }
        }

        public bool IsInitialized()
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<IsInitialized> " + Values.PKG_ADISCOPE + ": null");
                }
                return jc.CallStatic<bool>(Values.MTD_ISINITIALIZE);
            }            
        }

        public void SetRewardedCheckParam(string param)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<SetRewardedCheckParam> " + Values.PKG_ADISCOPE + ": null");
                }
                jc.CallStatic(Values.MTD_SET_REWARDED_CHECK_PARAM, param);
            }
        }

        public void SetLuckyEventAppId(string eventId, string pid) {
            _luckyEventId = eventId;
            _luckyEventPid = pid;
        }

        public void SetLuckyEventUseSafeAreaWebView(bool useSafeArea) {
            _luckyEventUseSafeArea = useSafeArea;
        }

        public void SetLuckyEventHashMark(string hashMark) {
            _luckyEventHashMark = hashMark;
        }

        public void SetLuckyEventBaseUrl(string baseUrl) {
            _luckyEventBaseUrl = baseUrl;
        }

        public void SetLuckyEventExtraParam(string key, string value) {
            _luckyEventExtraParams[key] = value;
        }

        public void ShowLuckyEvent() {
            if (this.activity == null)
            {
                Debug.LogError("Android.CoreClient<ShowLuckyEvent> UnityPlayerActivity: null");
                return;
            }

            AndroidJavaObject builder = new AndroidJavaObject(Values.PKG_TNK_EVENT_BUILDER);
            builder = builder.Call<AndroidJavaObject>(Values.MTD_TNK_SET_USER_NAME, _luckyEventUserId);
            builder = builder.Call<AndroidJavaObject>(Values.MTD_TNK_SET_CHILD_YN, _luckyEventChildYn);
            builder = builder.Call<AndroidJavaObject>(Values.MTD_TNK_SET_EVENT_ID_TNK_APP_ID, _luckyEventId, _luckyEventPid);
            
            builder.Call(Values.MTD_TNK_SHOW, this.activity);
        }
    }
}
#endif