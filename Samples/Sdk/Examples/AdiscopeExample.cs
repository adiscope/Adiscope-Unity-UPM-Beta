using Adiscope;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AdiscopeSdk Example 
/// </summary>
public class AdiscopeExample : MonoBehaviour
{

    private string MEDIA_ID;
    private string USER_ID;
    private string REWARDED_CHECK_PARAM;
    private string RED_COLOR;
    private string GREEN_COLOR;
    private string BLUE_COLOR;
    private string ALPHA_COLOR;
    private string MESSAGE_ALERT;
    private string RV_ID;
    private string IT_ID;
    private string RI_ID;
    private string RI_ID1;
    private string RI_ID2;
    private string RI_ID3;
    private string RI_ID4;
    private string RI_ID5;
    private string ROULETTE_ID;
    private string OFFERWALL_ID;
    private string OFFERWALL_DETAIL_ID;
    private string OFFERWALL_DETAIL_URL;
    private string OFFERWALL_DEEPLINK_URL;
    private string Find_UNIT_ID;
    private string CALLBACK_TAG;
    private string CHILD_YN;
    private float top;
    private bool isIndicatorMedium = false;
    private bool isIndicatorHidden = false;
    private bool isAlertHidden = false;

    public AdiscopeExample()
    {
#if UNITY_IOS
            MEDIA_ID = "";
            USER_ID = "";
            RV_ID = "";
            IT_ID = "";
            RI_ID = "";
            RI_ID1 = "";
            RI_ID2 = "";
            RI_ID3 = "";
            RI_ID4 = "";
            RI_ID5 = "";
            OFFERWALL_ID = "";
            REWARDED_CHECK_PARAM = "";
            ROULETTE_ID = "";
            Find_UNIT_ID = "";
            CALLBACK_TAG = "";
            CHILD_YN = "";
#endif

#if UNITY_ANDROID
            MEDIA_ID = "";
            USER_ID = "";
            RV_ID = "";
            IT_ID = "";
            REWARDED_CHECK_PARAM = "";
            RI_ID = "";
            RI_ID1 = "";
            RI_ID2 = "";
            RI_ID3 = "";
            RI_ID4 = "";
            RI_ID5 = "";
            ROULETTE_ID = "";
            OFFERWALL_ID = "";
            OFFERWALL_DETAIL_ID = "";
            OFFERWALL_DETAIL_URL = "";
            OFFERWALL_DEEPLINK_URL = "";
            Find_UNIT_ID = "";
            CALLBACK_TAG = "";
            CHILD_YN = "";
#endif
    }

    // Adiscope Interface
    private Adiscope.Feature.Core core;
    private Adiscope.Feature.OfferwallAd offerwallAd;
    private Adiscope.Feature.RewardedVideoAd rewardedVideoAd;
    private Adiscope.Feature.InterstitialAd interstitialAd;
    private Adiscope.Feature.RewardedInterstitialAd rewaredInterstitialAd;
    private Adiscope.Feature.AdEvent adEvent;

    // Properties
    private string outputMessage;
    private readonly int fontSize = 30;
    private List<ContentView> views;

    // Mapper
    class AdiscopeItemFetcher {
        private static Dictionary<string, string> secretKeys = new Dictionary<string, string>() {
            { "", "" },
            { "", "" }
        };

        public static string FetchMediaScretKey(string mediaID) { return secretKeys[mediaID]; }
    }

    public Vector2 scrollPosition = Vector2.zero;

    private void Start() { this.core = Adiscope.Sdk.GetCoreInstance(); }

    private void OnDisable()
    {
        // Unregister Adiscope Callbacks
        this.offerwallAd.OnOpened -= OnOfferwallAdOpenedCallback;
        this.offerwallAd.OnClosed -= OnOfferwallAdClosedCallback;
        this.offerwallAd.OnFailedToShow -= OnOfferwallAdFailedToShowCallback;


        this.rewardedVideoAd.OnLoaded -= OnRewardedVideoAdLoadedCallback;
        this.rewardedVideoAd.OnFailedToLoad -= OnRewardedVideoAdFailedToLoadCallback;
        this.rewardedVideoAd.OnOpened -= OnRewardedVideoAdOpenedCallback;
        this.rewardedVideoAd.OnClosed -= OnRewardedVideoAdClosedCallback;
        this.rewardedVideoAd.OnRewarded -= OnRewardedCallback;
        this.rewardedVideoAd.OnFailedToShow -= OnRewardedVideoAdFailedToShowCallback;


        this.interstitialAd.OnLoaded -= OnInterstitialAdLoadedCallback;
        this.interstitialAd.OnFailedToLoad -= OnInterstitialAdFailedToLoadCallback;
        this.interstitialAd.OnOpened -= OnInterstitialAdOpenedCallback;
        this.interstitialAd.OnClosed -= OnInterstitialAdClosedCallback;
        this.interstitialAd.OnFailedToShow -= OnInterstitialAdFailedToShowCallback;

        this.rewaredInterstitialAd.OnGetUnitStatus -= OnRewardedInterstitialGetUnitStatusCallback;
        this.rewaredInterstitialAd.OnSkip -= OnRewardedInterstitialAdSkipCallback;
        this.rewaredInterstitialAd.OnOpened -= OnRewardedInterstitialAdOpenedCallback;
        this.rewaredInterstitialAd.OnClosed -= OnRewardedInterstitialAdClosedCallback;
        this.rewaredInterstitialAd.OnFailedToShow -= OnRewardedInterstitialAdFailedToShowCallback;
        this.rewaredInterstitialAd.OnRewarded -= OnRewardedInterstitialRewardedCallback;

        this.adEvent.OnOpened -= OnAdEventOpenedCallback;
        this.adEvent.OnClosed -= OnAdEventClosedCallback;
        this.adEvent.OnFailedToShow -= OnAdEventFailedToShowCallback;
    }

    private void RegisterAdiscopeCallback()
    {
        if (this.offerwallAd == null)
        {
            this.offerwallAd = Adiscope.Sdk.GetOfferwallAdInstance();

            this.offerwallAd.OnOpened += OnOfferwallAdOpenedCallback;

            this.offerwallAd.OnClosed += OnOfferwallAdClosedCallback;

            this.offerwallAd.OnFailedToShow += OnOfferwallAdFailedToShowCallback;

        }

        if (this.rewardedVideoAd == null)
        {
            this.rewardedVideoAd = Adiscope.Sdk.GetRewardedVideoAdInstance();

            this.rewardedVideoAd.OnLoaded += OnRewardedVideoAdLoadedCallback;

            this.rewardedVideoAd.OnFailedToLoad += OnRewardedVideoAdFailedToLoadCallback;

            this.rewardedVideoAd.OnOpened += OnRewardedVideoAdOpenedCallback;

            this.rewardedVideoAd.OnClosed += OnRewardedVideoAdClosedCallback;

            this.rewardedVideoAd.OnRewarded += OnRewardedCallback;

            this.rewardedVideoAd.OnFailedToShow += OnRewardedVideoAdFailedToShowCallback;

        }

        if (this.interstitialAd == null)
        {
            this.interstitialAd = Adiscope.Sdk.GetInterstitialAdInstance();

            this.interstitialAd.OnLoaded += OnInterstitialAdLoadedCallback;

            this.interstitialAd.OnFailedToLoad += OnInterstitialAdFailedToLoadCallback;

            this.interstitialAd.OnOpened += OnInterstitialAdOpenedCallback;

            this.interstitialAd.OnClosed += OnInterstitialAdClosedCallback;

            this.interstitialAd.OnFailedToShow += OnInterstitialAdFailedToShowCallback;

        }

        if (this.rewaredInterstitialAd == null)
        {
            this.rewaredInterstitialAd = Adiscope.Sdk.GetRewardedInterstitialAdInstance();

            this.rewaredInterstitialAd.OnGetUnitStatus += OnRewardedInterstitialGetUnitStatusCallback;

            this.rewaredInterstitialAd.OnSkip += OnRewardedInterstitialAdSkipCallback;

            this.rewaredInterstitialAd.OnOpened += OnRewardedInterstitialAdOpenedCallback;

            this.rewaredInterstitialAd.OnClosed += OnRewardedInterstitialAdClosedCallback;

            this.rewaredInterstitialAd.OnFailedToShow += OnRewardedInterstitialAdFailedToShowCallback;

            this.rewaredInterstitialAd.OnRewarded += OnRewardedInterstitialRewardedCallback;

        }

        if(this.adEvent == null)
        {
            this.adEvent = Adiscope.Sdk.GetAdEventInstance();
            this.adEvent.OnOpened += OnAdEventOpenedCallback;
            this.adEvent.OnClosed += OnAdEventClosedCallback;
            this.adEvent.OnFailedToShow += OnAdEventFailedToShowCallback;
        }
    }

    private void OnInitializedCallback(object sender, Adiscope.Model.InitResult args) { this.AddOutputMessage("initialize - args: " + args); }
    private void OnOfferwallAdOpenedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= offerwallAd.OnOpened - args: " + args);
    }
    private void OnOfferwallAdClosedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= offerwallAd.OnClosed - args: " + args); }
    private void OnOfferwallAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= offerwallAd.OnFailedToShow - args: " + args); }


    private void OnRewardedVideoAdLoadedCallback(object sender, Adiscope.Model.LoadResult args) { this.AddOutputMessage("  <= rewardedVideoAd.OnLoaded" + args); }
    private void OnRewardedVideoAdFailedToLoadCallback(object sender, Adiscope.Model.LoadFailure args) { this.AddOutputMessage("  <= rewardedVideoAd.OnFailedToLoad - args: " + args); }
    private void OnRewardedVideoAdOpenedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= rewardedVideoAd.OnOpened - args: " + args); }
    private void OnRewardedVideoAdClosedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= rewardedVideoAd.OnClosed - args: " + args);
    }
    private void OnRewardedVideoAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= rewardedVideoAd.OnFailedToShow - args: " + args); }
    private void OnRewardedCallback(object sender, Adiscope.Model.RewardItem args) { this.AddOutputMessage("  <= rewardedVideoAd.OnRewarded - args: " + args); }


    private void OnInterstitialAdLoadedCallback(object sender, Adiscope.Model.LoadResult args) { this.AddOutputMessage("  <= interstitialAd.OnLoaded"); }
    private void OnInterstitialAdFailedToLoadCallback(object sender, Adiscope.Model.LoadFailure args) { this.AddOutputMessage("  <= interstitialAd.OnFailedToLoad - args: " + args); }
    private void OnInterstitialAdOpenedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= interstitialAd.OnOpened - args: " + args); }
    private void OnInterstitialAdClosedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= interstitialAd.OnClosed - args: " + args); }
    private void OnInterstitialAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= interstitialAd.OnFailedToShow - args: " + args); }


    private void OnRewardedInterstitialGetUnitStatusCallback(object sender, Adiscope.Model.UnitStatus args) { this.AddOutputMessage("  <= rewaredInterstitialAd.OnGetUnitStatus - args: " + args); }
    private void OnRewardedInterstitialAdSkipCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= rewaredInterstitialAd.OnSkip - args: " + args); }
    private void OnRewardedInterstitialAdOpenedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= rewaredInterstitialAd.OnOpened - args: " + args); }
    private void OnRewardedInterstitialAdClosedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= rewaredInterstitialAd.OnClosed - args: " + args);
    }
    private void OnRewardedInterstitialAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= rewaredInterstitialAd.OnFailedToShow - args: " + args); }
    private void OnRewardedInterstitialRewardedCallback(object sender, Adiscope.Model.RewardItem args) { this.AddOutputMessage("  <= rewaredInterstitialAd.OnRewarded - args: " + args); }

    private void OnAdEventOpenedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= adEvent.OnOpened - args: " + args);
    }
    private void OnAdEventClosedCallback(object sender, Adiscope.Model.ShowResult args)
    { 
        this.AddOutputMessage("  <= adEvent.OnClosed - args: " + args); 
    }
    private void OnAdEventFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args)
    {
        this.AddOutputMessage("  <= adEvent.OnFailedToShow - args: " + args);
    }


    #region GUI
    private void AddContentViews()
    {

        GUI.backgroundColor = Color.black;

        // Core
        this.AddLabel("Core");
        this.AddTextField("Media ID", TextFieldType.MediaID);
        this.AddTextField("User ID", TextFieldType.UserID);
        this.AddButton("Set User ID", () => {
            this.core.SetUserId(USER_ID);
            this.AddOutputMessage("Set User ID: " + USER_ID);
        });

        this.AddTextField("Callback Tag", TextFieldType.CallbackTag);
        this.AddTextField("Child YN", TextFieldType.ChildYN);

        this.AddTextField("Custom Data", TextFieldType.CustomData);
        this.AddButton("Set Rewarded Check Param", () => {
            this.core.SetRewardedCheckParam(REWARDED_CHECK_PARAM);
            this.AddOutputMessage("Set Rewarded Check Param: " + REWARDED_CHECK_PARAM);
        });

        this.AddButton("Initialize", () =>
        {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) =>
            {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            });
        });

        this.AddButton("Initialize(listener, callbackTag)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            }, CALLBACK_TAG);

        });

        this.AddButton("Initialize(listener, callbackTag, childYN)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            }, CALLBACK_TAG, CHILD_YN);

        });

        this.AddButton("Initialize(mediaId, mediaSecret, callbackTag, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize(MEDIA_ID, secretKey, CALLBACK_TAG, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            });
        });

        this.AddButton("Initialize(mediaId, mediaSecret, callbackTag, childYN, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize(MEDIA_ID, secretKey, CALLBACK_TAG, CHILD_YN, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            });
        });

        this.AddButton("InitializeTest(mediaId, mediaSecret, callbackTag, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.InitializeTest(MEDIA_ID, secretKey, CALLBACK_TAG, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            });
        });

        this.AddButton("InitializeTest(mediaId, mediaSecret, callbackTag, childYN, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.InitializeTest(MEDIA_ID, secretKey, CALLBACK_TAG, CHILD_YN, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID + ", Log: " + Adiscope.Sdk.GetOptionGetter().GetInitializeFailLog());
            });
        });


        this.AddButton("is Initialized", () => {
            this.AddOutputMessage("Initialized Flag: " + Adiscope.Sdk.GetCoreInstance().IsInitialized());
        });

        this.AddButton("Print SDK Version", () => {
            this.AddOutputMessage("SDK Versions => " + Adiscope.Sdk.GetOptionGetter().GetSDKVersion());
        });

        this.AddButton("Print Unity SDK Version", () => {
            this.AddOutputMessage("SDK Versions => " + Adiscope.Sdk.GetOptionGetter().GetUnitySDKVersion());
        });

        this.AddButton("Print Network Version", () => {
            this.AddOutputMessage("Network Versions => " + Adiscope.Sdk.GetOptionGetter().GetNetworkVersions());
        });

        this.AddTextField("Unit ID", TextFieldType.FindUnitID);

        this.AddButton("getUnitStatus", () => {
            this.core.GetUnitStatus(Find_UNIT_ID, (error, result) => {
                if (error != null) this.AddOutputMessage("  <= error : " + error);
                else this.AddOutputMessage("  <= result : " + result);
            });
        });

#if UNITY_IOS
        this.AddButton("Show Max Mediation Debugger", () => {
            Adiscope.Sdk.GetOptionSetter().ShowMaxMediationDebugger();
            this.AddOutputMessage("Show Max Mediation Debugger");
        });

        this.AddButton("Show Admob Mediation Debugger", () => {
             Adiscope.Sdk.GetOptionSetter().ShowAdmobMediationDebugger();
            this.AddOutputMessage("Show Admob Mediation Debugger");
        });
#endif

#if UNITY_ANDROID
        this.AddLabel("미디에이션 디버거 - 애디스콥 이니셜라이즈 성공 및 맥스 이니셜라이즈 이력 있을 시 진입 가능");
        this.AddButton("MAX Mediation Debugger", () => {
             Adiscope.Sdk.GetOptionSetter().ShowMaxMediationDebugger();
        });

        this.AddLabel("애드몹 Ad Inspector - 미동작 시 애드몹 물량 로드 후 진입 가능 (이니셜라이즈 필요)");
        this.AddButton("Admob Mediation Debugger", () => {
             Adiscope.Sdk.GetOptionSetter().ShowAdmobMediationDebugger();
        });
#endif

        this.AddButton("Ad Sound On", () => {
            Adiscope.Sdk.GetOptionSetter().SetVolumeOff(false);
            this.AddOutputMessage("Ad Sound On");
        });

        this.AddButton("Ad Sound Off", () => {
            Adiscope.Sdk.GetOptionSetter().SetVolumeOff(true);
            this.AddOutputMessage("Ad Sound Off");
        });

        // Offerwall
        this.AddSpacer();
        this.AddTextField("Offerwall", TextFieldType.OfferwallUnit);
        this.AddButton("Show Offerwall", () => {
            string unitID = OFFERWALL_ID.ToUpper();
            if (unitID == null)
            {
                this.AddOutputMessage("Not Found unitID: " + MEDIA_ID);
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.Show(unitID, filter)) { } else { this.AddOutputMessage("offerwallAd.Show request is duplicated"); }
        });
        this.AddTextField("Offerwall Detail Id", TextFieldType.OfferwallDetailId);
        this.AddButton("Show Detail Func", () => {
            string unitID = OFFERWALL_ID.ToUpper();
            string itemId = OFFERWALL_DETAIL_ID;
            if (unitID == null)
            {
                this.AddOutputMessage("Not Found unitID: " + unitID);
                return;
            }
            if (itemId == null)
            {
                this.AddOutputMessage("Not Found itemId: " + itemId);
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.ShowOfferwallDetail(unitID, filter, itemId)) { } else { this.AddOutputMessage("offerwallAd.Show request is duplicated"); }
        });
        this.AddTextField("Offerwall Detail URL", TextFieldType.OfferwallDetailUrl);
        this.AddButton("Show Detail Func Offerwall URL", () => {
            string url = OFFERWALL_DETAIL_URL;
            if (url == null)
            {
                this.AddOutputMessage("Empty URL");
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.ShowOfferwallDetailFromUrl(url)) { } else { this.AddOutputMessage("URL is empty"); }
        });
        this.AddTextField("Deeplink URL", TextFieldType.OfferwallDeeplinkUrl);
        this.AddButton("Show Detail from Deeplink", () => {
            string url = OFFERWALL_DEEPLINK_URL;
            if (url == null)
            {
                this.AddOutputMessage("Empty URL");
                return;
            }
            Application.OpenURL(url);
        });

        // Rewarded Video
        this.AddSpacer();
        this.AddLabel("Rewared Video");
        this.AddTextField("Rewarded Unit", TextFieldType.RewardedUnit);
        this.AddButton("Video - Load", () => {
            this.rewardedVideoAd.Load(RV_ID);
        });
        this.AddButton("Video - IsLoaded", () => {
            bool isLoaded = this.rewardedVideoAd.IsLoaded(RV_ID);
            this.AddOutputMessage("rewardedVideoAd.IsLoaded => " + isLoaded);
        });
        this.AddButton("Video - Show", () => {
            this.rewardedVideoAd.Show();
        });
        
#if UNITY_IOS
        this.AddButton("Video - Show With Load", () => {
            this.rewardedVideoAd.ShowWithLoad(RV_ID);
        });
#endif

        // Interstitial
        this.AddSpacer();
        this.AddLabel("Interstitial");
        this.AddTextField("Interstitial Unit", TextFieldType.InterstitialUnit);
        this.AddButton("Interstitial - Load unit", () => {
            this.interstitialAd.Load(IT_ID);
        });
        this.AddButton("Interstitial - IsLoaded", () => {
            bool isLoaded = this.interstitialAd.IsLoaded(IT_ID);
            this.AddOutputMessage("Interstitial.IsLoaded => " + isLoaded);
        });
        this.AddButton("Interstitial - Show", () => {
            this.interstitialAd.Show();
        });
        
#if UNITY_IOS
        this.AddButton("Interstitial - Show With Load", () => {
            this.interstitialAd.ShowWithLoad(IT_ID);
        });
#endif

        // RewardedInterstitial
        this.AddSpacer();
        this.AddLabel("RewaredInterstitial");
        this.AddButton("RewaredInterstitial - PreLoadAll", () => {
            this.rewaredInterstitialAd.PreLoadAllRewardedInterstitial();
        });
        this.AddTextField("PreLoad Unit1", TextFieldType.RewardedInterstitialUnit1);
        this.AddTextField("PreLoad Unit2", TextFieldType.RewardedInterstitialUnit2);
        this.AddTextField("PreLoad Unit3", TextFieldType.RewardedInterstitialUnit3);
        this.AddTextField("PreLoad Unit4", TextFieldType.RewardedInterstitialUnit4);
        this.AddTextField("PreLoad Unit5", TextFieldType.RewardedInterstitialUnit5);
        this.AddButton("RewaredInterstitial - PreLoad", () => {
            this.rewaredInterstitialAd.PreLoadRewardedInterstitial(new string[] { RI_ID1, RI_ID2, RI_ID3, RI_ID4, RI_ID5 });
        });
        this.AddTextField("Show Unit", TextFieldType.RewardedInterstitialUnit);
        this.AddButton("RewaredInterstitial - Show", () => {
            this.rewaredInterstitialAd.ShowRewardedInterstitial(RI_ID);
        });
        this.AddButton("RewaredInterstitial - Print Status", () => {
            this.rewaredInterstitialAd.GetUnitStatusRewardedInterstitial(RI_ID);
        });

#if UNITY_IOS
        this.AddTextField("Show With Load Red", TextFieldType.Red);
        this.AddTextField("Show With Load Green", TextFieldType.Green);
        this.AddTextField("Show With Load Blue", TextFieldType.Blue);
        this.AddTextField("Show With Load Alpha", TextFieldType.Alpha);
        this.AddButton("ShowWithLoad Background Color", () => {
            Adiscope.Sdk.GetOptionSetter().SetShowWithLoad2BackgroundColor(RED_COLOR, GREEN_COLOR, BLUE_COLOR, ALPHA_COLOR);
            this.AddOutputMessage("ShowWithLoad Background Color");
        });
        this.AddButton("ShowWithLoad IndicatorView Style", () => {
            this.isIndicatorMedium = !this.isIndicatorMedium;
            Adiscope.Sdk.GetOptionSetter().SetShowWithLoad2IndicatorStyleMedium(this.isIndicatorMedium, this.isIndicatorHidden);
            this.AddOutputMessage("ShowWithLoad IndicatorView Style");
        });
        this.AddButton("ShowWithLoad IndicatorView Hidden", () => {
            this.isIndicatorHidden = !this.isIndicatorHidden;
            Adiscope.Sdk.GetOptionSetter().SetShowWithLoad2IndicatorStyleMedium(this.isIndicatorMedium, this.isIndicatorHidden);
            this.AddOutputMessage("ShowWithLoad IndicatorView Hidden");
        });
        this.AddTextField("Show With Load Alert Msg", TextFieldType.Message);
        this.AddButton("ShowWithLoad Alert Message", () => {
            Adiscope.Sdk.GetOptionSetter().SetShowWithLoad2ErrorAlertMsg(MESSAGE_ALERT, this.isAlertHidden);
            this.AddOutputMessage("ShowWithLoad Alert Message");
        });
        this.AddButton("ShowWithLoad IndicatorView Style", () => {
            this.isAlertHidden = !this.isAlertHidden;
            Adiscope.Sdk.GetOptionSetter().SetShowWithLoad2ErrorAlertMsg(MESSAGE_ALERT, this.isAlertHidden);
            this.AddOutputMessage("ShowWithLoad IndicatorView Style");
        });
#endif

        // AdEvent
        this.AddSpacer();
        this.AddLabel("AdEvent");
        this.AddTextField("Roulette", TextFieldType.RouletteUnit);
        this.AddButton("AdEvent - Show", () => {
            string unitID = ROULETTE_ID.ToUpper();
            if (unitID == null)
            {
                this.AddOutputMessage("Not Found unitID: " + MEDIA_ID);
                return;
            }

            if (this.adEvent.Show(unitID)) { } else { this.AddOutputMessage("adEvent.Show request is duplicated"); }
        });
    }

    private void OnGUI()
    {

        // Support Only Portrait
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float horizontalMargin = (float)(screenWidth * 0.03);                   // Horizon Margin Height
        float verticalMargin = (float)(screenHeight * 0.015);                   // Vertical Margin Height
#if UNITY_IOS
        top = verticalMargin * 3;
#endif

#if UNITY_ANDROID
        top = verticalMargin;
#endif
        float contentHeight = (float)(screenHeight * 0.3);
        Rect rect = new Rect(horizontalMargin, top, screenWidth - (horizontalMargin * 2), contentHeight);
        GUIStyle logTextStyle = new GUIStyle(GUI.skin.textArea);
        logTextStyle.fontSize = fontSize;
        GUI.Label(rect, this.outputMessage, logTextStyle);

        ClearContentViews();
        AddContentViews();

        contentHeight = (float)(50);                                            // Content Height : 5% for Screen Height
        top = rect.yMax + verticalMargin;

        Rect position = new Rect(0, top, Screen.width, Screen.height - top);
        Rect viewRect = new Rect(0, 0, Screen.width, 0);
        foreach (ContentView view in this.views)
        {
            viewRect.height += view.height;
            viewRect.height += verticalMargin;
        }

        GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.04f;
        GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.04f;

        scrollPosition = GUI.BeginScrollView(
            position, scrollPosition, viewRect
        );

        top = 0;
        float contentWidth = screenWidth - (horizontalMargin * 2);
        foreach (ContentView view in this.views)
        {

            if (view.type == ContentViewType.Spacer)
            {
                rect = new Rect(0, 0, 0, view.height);

            }
            else if (view.type == ContentViewType.Button)
            {
                rect = new Rect(horizontalMargin, top, contentWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fontSize = fontSize;
                if (GUI.Button(rect, view.title, style))
                {
                    this.AddOutputMessage("Button Tapped: " + view.title);
                    view.buttonAction();
                };

            }
            else if (view.type == ContentViewType.Label)
            {
                rect = new Rect(horizontalMargin, top, contentWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.fontSize = fontSize;
                GUI.Label(rect, view.title, style);

            }
            else if (view.type == ContentViewType.TextField)
            {
                float descriptionWidth = (float)(contentWidth * 0.33);          // Description Label Width 33%
                float textFieldWidth = (float)(contentWidth * 0.63);            // Text Field Width 63%
                                                                                // Margin 4%

                Rect descriptionRect = new Rect(horizontalMargin, top, descriptionWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.fontSize = fontSize;
                GUI.Label(descriptionRect, view.title, style);

                float maxX = horizontalMargin + descriptionWidth;               // Calulate Textfield MinX
                maxX += (float)(contentWidth - (descriptionWidth + textFieldWidth));

                rect = new Rect(maxX, top, textFieldWidth, view.height);

                switch (view.textFieldType)
                {
                    case TextFieldType.MediaID: MEDIA_ID = GUI.TextField(rect, MEDIA_ID, style); break;
                    case TextFieldType.UserID: USER_ID = GUI.TextField(rect, USER_ID, style); break;
                    case TextFieldType.CustomData: REWARDED_CHECK_PARAM = GUI.TextField(rect, REWARDED_CHECK_PARAM, style); break;
                    case TextFieldType.OfferwallUnit: OFFERWALL_ID = GUI.TextField(rect, OFFERWALL_ID, style); break;
                    case TextFieldType.OfferwallDetailId: OFFERWALL_DETAIL_ID = GUI.TextField(rect, OFFERWALL_DETAIL_ID, style); break;
                    case TextFieldType.OfferwallDetailUrl: OFFERWALL_DETAIL_URL = GUI.TextField(rect, OFFERWALL_DETAIL_URL, style); break;
                    case TextFieldType.OfferwallDeeplinkUrl: OFFERWALL_DEEPLINK_URL = GUI.TextField(rect, OFFERWALL_DEEPLINK_URL, style); break;
                    case TextFieldType.FindUnitID: Find_UNIT_ID = GUI.TextField(rect, Find_UNIT_ID, style); break;
                    case TextFieldType.RewardedUnit: RV_ID = GUI.TextField(rect, RV_ID, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit: RI_ID = GUI.TextField(rect, RI_ID, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit1: RI_ID1 = GUI.TextField(rect, RI_ID1, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit2: RI_ID2 = GUI.TextField(rect, RI_ID2, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit3: RI_ID3 = GUI.TextField(rect, RI_ID3, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit4: RI_ID4 = GUI.TextField(rect, RI_ID4, style).ToUpper(); break;
                    case TextFieldType.RewardedInterstitialUnit5: RI_ID5 = GUI.TextField(rect, RI_ID5, style).ToUpper(); break;
                    case TextFieldType.InterstitialUnit: IT_ID = GUI.TextField(rect, IT_ID, style).ToUpper(); break;
                    case TextFieldType.RouletteUnit: ROULETTE_ID = GUI.TextField(rect, ROULETTE_ID, style).ToUpper(); break;
                    case TextFieldType.CallbackTag: CALLBACK_TAG = GUI.TextField(rect, CALLBACK_TAG, style).ToUpper(); break;
                    case TextFieldType.ChildYN: CHILD_YN = GUI.TextField(rect, CHILD_YN, style).ToUpper(); break;
                    case TextFieldType.Red: RED_COLOR = GUI.TextField(rect, RED_COLOR, style); break;
                    case TextFieldType.Green: GREEN_COLOR = GUI.TextField(rect, GREEN_COLOR, style); break;
                    case TextFieldType.Blue: BLUE_COLOR = GUI.TextField(rect, BLUE_COLOR, style); break;
                    case TextFieldType.Alpha: ALPHA_COLOR = GUI.TextField(rect, ALPHA_COLOR, style); break;
                    case TextFieldType.Message: MESSAGE_ALERT = GUI.TextField(rect, MESSAGE_ALERT, style); break;
                }

            }


            top += rect.height;
            top += verticalMargin;
        }

        GUI.EndScrollView();
    }

    private void AddOutputMessage(string message)
    {
        if (this.outputMessage == null || this.outputMessage.Length == 0) { this.outputMessage = message; }
        else { this.outputMessage = GetCurrentDateTime() + message + "\n" + this.outputMessage; }
    }
    
    private string GetCurrentDateTime()
    {
        return "["+DateTime.Now.ToString("HH:mm:ss")+"] ";
    }

    private void ClearContentViews()
    {
        if (this.views == null) { this.views = new List<ContentView>(); }
        else { this.views.Clear(); }
    }

    private void AddSpacer()
    {
        this.views.Add(new ContentView(ContentViewType.Spacer));
    }

    private void AddLabel(string title)
    {
        this.views.Add(new ContentView(ContentViewType.Label, title));
    }

    private void AddButton(string title, Action function)
    {
        this.views.Add(new ContentView(ContentViewType.Button, title, function));
    }

    private void AddTextField(string title, TextFieldType type)
    {
        this.views.Add(new ContentView(ContentViewType.TextField, title, type));
    }

#endregion
}

public enum ContentViewType { Button, TextField, Label, Spacer }
public enum TextFieldType
{
    MediaID, UserID, FindUnitID, RewardedUnit, InterstitialUnit, OfferwallUnit, OfferwallDetailId,
    OfferwallDetailUrl, OfferwallDeeplinkUrl, OfferwallApplinkUrl, CallbackTag, ChildYN, RewardedInterstitialUnit,
    RewardedInterstitialUnit1, RewardedInterstitialUnit2, RewardedInterstitialUnit3, RewardedInterstitialUnit4,
    RewardedInterstitialUnit5, RouletteUnit, CustomData, Red, Green, Blue, Alpha, Message
}

class ContentView
{
    public TextFieldType textFieldType;
    public ContentViewType type;
    public String title;
    public Action buttonAction;

    public ContentView(ContentViewType type) { this.type = type; }

    public ContentView(ContentViewType type, String title, Action action = null)
    {
        this.type = type;
        this.title = title;
        this.buttonAction = action;
    }

    public ContentView(ContentViewType type, String title, TextFieldType textFieldType)
    {
        this.type = type;
        this.title = title;
        this.textFieldType = textFieldType;
    }

    public float height
    {
        get
        {
            switch (this.type)
            {
                case ContentViewType.Button:
                    return 100;
                case ContentViewType.TextField:
                case ContentViewType.Label:
                    return 50;
                case ContentViewType.Spacer:
                    return 25;
                default:
                    return 0;
            }
        }
    }
}