using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using Adiscope.Editor;

namespace Adiscope
{
    public static class FrameworkSettingsRegister
    {
        public const string SERVICE_JSON_KEY_ADMOB         = "com.google.android.gms.ads.APPLICATION_ID";
        public const string SERVICE_JSON_KEY_APPLOVIN      = "applovin.sdk.key";
        public const string SERVICE_JSON_KEY_AD_REVIEW     = "applovin.ad.review.key";

        private const string SERVICE_JSON_KEY_ADISCOPE      = "adiscope";
        private const string SERVICE_JSON_KEY_NETWORK       = "network";
        private const string SERVICE_JSON_KEY_ADS           = "ads";
        private const string SERVICE_JSON_KEY_SETTINGS      = "settings";
        private const string SERVICE_JSON_KEY_REWARDEDVIDEO = "rewardedVideoAd";
        private const string SERVICE_JSON_KEY_INTERSTITIAL  = "interstitialAd";

        private const string PATH_ADISCOPE_EDITOR           = "/Adiscope/Editor";
        private static string SettingsPath                  = "Assets/Adiscope/Editor/Adiscope.asset";
        private const string PATH_LAUNCHER_TEMPLATE         = "Assets/Plugins/Android/launcherTemplate.gradle";

        private static string[] OS_Type     = { "None", "AOS & iOS", "AOS", "iOS" };
        private static string[] AOS_Type    = { "None", "AOS(iOS 기능 추가 시 자동 추가)", "AOS", "None(iOS 기능 추가 시 자동 추가)" };
        private static string[] iOS_Type    = { "None", "iOS(AOS 기능 추가 시 자동 추가)", "None(AOS 기능 추가 시 자동 추가)", "iOS" };
        private static string[] AOS_fix_Type    = { "AOS", "AOS & iOS" };



        /// <summary>
        /// install plugins without UI prompt
        /// </summary>
        /// <param name="androidJsonFilePath">path of (mediaId)_AndroidAdiscope.json</param>
        /// <param name="iOSJsonFilePath">path of (mediaId)_iOSAdiscope.json</param>
        public static bool AdiscopeImportJson(string androidJsonFilePath, string iOSJsonFilePath) {
            bool isAndroidPath  = (androidJsonFilePath != string.Empty && androidJsonFilePath.Trim().Length > 0);
            bool isiOSPath      = (iOSJsonFilePath != string.Empty && iOSJsonFilePath.Trim().Length > 0);

            if (isAndroidPath) {
                SettingsJson(androidJsonFilePath, true);
            }

            if (isiOSPath) {
                SettingsJson(iOSJsonFilePath, false);
            }

            if (isAndroidPath && isiOSPath) {
                AssetDatabase.SaveAssets();
                return BuildPostProcessorForAndroid.CreateAdiscopeAndroidFiles(false) 
                        && BuildPostProcessorForIosEdm4u.CreateAdiscopeIosFiles(false);
            } else if (isAndroidPath) {
                AssetDatabase.SaveAssets();
                return BuildPostProcessorForAndroid.CreateAdiscopeAndroidFiles(false);
            } else if (isiOSPath) {
                AssetDatabase.SaveAssets();
                return BuildPostProcessorForIosEdm4u.CreateAdiscopeIosFiles(false);
            } else {
                return false;
            }
        }



        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/AdiscopeSDK", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    string filePath = "Packages/com.tnk.adiscope/package.json";
                    string json = File.ReadAllText(filePath);
                    ParsingPackageJson.PackageJson pj = JsonUtility.FromJson<ParsingPackageJson.PackageJson>(json);
                    
                    var serialized = new SerializedObject(Load());
                    EditorGUILayout.HelpBox(string.Format("Version {0}", pj.version), MessageType.None);
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Settings Android from json file", GUILayout.Height(30))) {
                        SettingsJson(EditorUtility.OpenFilePanel("Settings Android from json file", null, "json"), true);
                    }
                    if (GUILayout.Button("Settings iOS from json file", GUILayout.Height(30))) {
                        SettingsJson(EditorUtility.OpenFilePanel("Settings iOS from json file", null, "json"), false);
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serialized.FindProperty("_mediaID_aos"), new GUIContent("Media ID(AOS)"));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_mediaSecret_aos"), new GUIContent("Media Secret(AOS)"));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_mediaID_ios"), new GUIContent("Media ID(iOS)"));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_mediaSecret_ios"), new GUIContent("Media Secret(iOS)"));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_subDomain"), new GUIContent("Sub Domain"));
                    EditorGUILayout.LabelField(new GUIContent("Tracking Desc(iOS)"));
                    SerializedProperty myStringProperty = serialized.FindProperty("_trackingDesc");
                    myStringProperty.stringValue = EditorGUILayout.TextArea(myStringProperty.stringValue, GUILayout.Height(50));
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    int tnkpubAdapter = serialized.FindProperty("_tnkpubAdapter").intValue;
                    tnkpubAdapter = EditorGUILayout.Popup("TNKPub Adapter", tnkpubAdapter, OS_Type);
                    serialized.FindProperty("_tnkpubAdapter").intValue = tnkpubAdapter;
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    int maxAdapter = serialized.FindProperty("_maxAdapter").intValue;
                    maxAdapter = EditorGUILayout.Popup("Max Adapter", maxAdapter, OS_Type);
                    serialized.FindProperty("_maxAdapter").intValue = maxAdapter;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    bool applovinAdReview = serialized.FindProperty("_applovinAdReview").boolValue;
                    applovinAdReview = EditorGUILayout.Toggle("Applovin Ad Review", applovinAdReview);
                    serialized.FindProperty("_applovinAdReview").boolValue = applovinAdReview;
                    GUILayout.EndHorizontal();

                    EditorGUI.BeginDisabledGroup(!applovinAdReview);
                    EditorGUILayout.PropertyField(serialized.FindProperty("_applovinAdReviewKey"), new GUIContent("AppLovin Ad Review Key"));
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    int admobAdapter = serialized.FindProperty("_admobAdapter").intValue;
                    admobAdapter = EditorGUILayout.Popup("AdMob Adapter", admobAdapter, OS_Type);
                    serialized.FindProperty("_admobAdapter").intValue = admobAdapter;
                    int admanagerAdapter = serialized.FindProperty("_admanagerAdapter").intValue;
                    admanagerAdapter = EditorGUILayout.Popup("Admanager Adapter", admanagerAdapter, iOS_Type);
                    serialized.FindProperty("_admanagerAdapter").intValue = admanagerAdapter;
                    GUILayout.EndHorizontal();
                    EditorGUI.BeginDisabledGroup((admobAdapter == 0 || admobAdapter == 3) && (maxAdapter == 0 || maxAdapter == 3));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_admobAppKey_aos"), new GUIContent("AdMob App Key(AOS)"));
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup((admobAdapter == 0 || admobAdapter == 2) && (admanagerAdapter == 0 || admanagerAdapter == 2) && (maxAdapter == 0 || maxAdapter == 2));
                    EditorGUILayout.PropertyField(serialized.FindProperty("_admobAppKey_ios"), new GUIContent("AdMob App Key(iOS)"));
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    int chartboostAdapter = serialized.FindProperty("_chartboostAdapter").intValue;
                    chartboostAdapter = EditorGUILayout.Popup("Chartboost Adapter", chartboostAdapter, OS_Type);
                    serialized.FindProperty("_chartboostAdapter").intValue = chartboostAdapter;

                    int pangleAdapter = serialized.FindProperty("_pangleAdapter").intValue;
                    pangleAdapter = EditorGUILayout.Popup("Pangle Adapter", pangleAdapter, OS_Type);
                    serialized.FindProperty("_pangleAdapter").intValue = pangleAdapter;
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    int vungleAdapter = serialized.FindProperty("_vungleAdapter").intValue;
                    vungleAdapter = EditorGUILayout.Popup("Vungle Adapter", vungleAdapter, OS_Type);
                    serialized.FindProperty("_vungleAdapter").intValue = vungleAdapter;

                    int luckyeventAdapter = serialized.FindProperty("_luckyeventAdapter").intValue;
                    luckyeventAdapter = EditorGUILayout.Popup("Lucky Event", luckyeventAdapter, OS_Type);
                    serialized.FindProperty("_luckyeventAdapter").intValue = luckyeventAdapter;
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.EndFoldoutHeaderGroup();
                    if (EditorGUI.EndChangeCheck())
                    {
                        serialized.ApplyModifiedProperties();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Create Adiscope Android & iOS Files", GUILayout.Height(30)))
                    {
                        string mediaId     = serialized.FindProperty("_mediaID_aos").stringValue;
                        string mediaSecret = serialized.FindProperty("_mediaSecret_aos").stringValue;
                        string subDomain   = serialized.FindProperty("_subDomain").stringValue;
                        UpdateLauncherGradleManifestPlaceholders(mediaId, mediaSecret, subDomain);

                        if (BuildPostProcessorForAndroid.CreateAdiscopeAndroidFiles(true)       // Manifest 파일 생성
                            && BuildPostProcessorForIosEdm4u.CreateAdiscopeIosFiles(true)) {
                            EditorUtility.ClearProgressBar();
                            AssetDatabase.SaveAssets();
                            EditorUtility.DisplayDialog("Succeed to install", "파일이 정상적으로 생성되었습니다.", "닫기");
                        } else {
                            EditorUtility.ClearProgressBar();
                            EditorUtility.DisplayDialog("Failed to install", "파일 생성 실패", "닫기");
                        }
                    }
                },
                
                keywords = new HashSet<string>(new[] { "Adiscope", "AdiscopeSDK", "MediaId" })
            };
        }

        public static FrameworkSettings Load()
        {
            var settings = AssetDatabase.LoadAssetAtPath<FrameworkSettings>(SettingsPath);
            if (settings == null)
            {
                CreateAdiscopeFrameworksDirectory();
                settings = ScriptableObject.CreateInstance<FrameworkSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        // json 파일로 세팅값들 설정
        private static void SettingsJson(string filePath, bool isAndroid) {
            if (filePath == string.Empty || filePath.Trim().Length < 1) {
                return;
            }

            FileManager fm = new FileManager();
            Dictionary<string, object> settings = fm.ReadJsonFile(filePath);
            if (settings == null) {
                Debug.LogError("can't get service setting from: " + filePath);
                return;
            }

            var serialized = new SerializedObject(Load());
            Dictionary<string, object> adiscopeInfoSettings = null;
            // adiscope 값 설정
            if (!settings.ContainsKey(SERVICE_JSON_KEY_ADISCOPE) || settings[SERVICE_JSON_KEY_ADISCOPE] == null) {
                Debug.LogError("missing json key [" + SERVICE_JSON_KEY_ADISCOPE + "] from service setting");
            } else {
                Dictionary<string, object> adiscopeInfo = settings[SERVICE_JSON_KEY_ADISCOPE] as Dictionary<string, object>;
                if (!adiscopeInfo.ContainsKey(SERVICE_JSON_KEY_SETTINGS) || adiscopeInfo[SERVICE_JSON_KEY_SETTINGS] == null) {
                    Debug.LogError("missing json key [" + SERVICE_JSON_KEY_SETTINGS + "] from service setting");
                } else {
                    adiscopeInfoSettings = adiscopeInfo[SERVICE_JSON_KEY_SETTINGS] as Dictionary<string, object>;
                    if (isAndroid) {
                        serialized.FindProperty("_mediaID_aos").stringValue = adiscopeInfoSettings["adiscope_media_id"].ToString();
                        serialized.FindProperty("_mediaSecret_aos").stringValue = adiscopeInfoSettings["adiscope_media_secret"].ToString();
                        serialized.FindProperty("_subDomain").stringValue = adiscopeInfoSettings["adiscope_sub_domain"].ToString();
                    } else {
                        serialized.FindProperty("_mediaID_ios").stringValue = adiscopeInfoSettings["adiscope_media_id"].ToString();
                        serialized.FindProperty("_mediaSecret_ios").stringValue = adiscopeInfoSettings["adiscope_media_secret"].ToString();
                    }
                    serialized.ApplyModifiedProperties();
                }
            }

            // adapter 사용 유무 및 Key 설정
            if (!settings.ContainsKey(SERVICE_JSON_KEY_NETWORK) || settings[SERVICE_JSON_KEY_NETWORK] == null) {
                Debug.LogError("missing json key [" + SERVICE_JSON_KEY_NETWORK + "] from service setting");
            } else {
                Dictionary<string, object> adiscopeNetworks = settings[SERVICE_JSON_KEY_NETWORK] as Dictionary<string, object>;
                foreach (string adNetworkName in adiscopeNetworks.Keys) {
                    if (AdiscopeAdapterSettings.GetIsSetting(adNetworkName, isAndroid)) {
                        Dictionary<string, object> networkInfo = adiscopeNetworks[adNetworkName] as Dictionary<string, object>;
                        Dictionary<string, object> networkInfoAds = networkInfo[SERVICE_JSON_KEY_ADS] as Dictionary<string, object>;
                        Dictionary<string, object> networkInfoSettings = networkInfo[SERVICE_JSON_KEY_SETTINGS] as Dictionary<string, object>;
                        bool rewardedVideoAdEnabled = Boolean.Parse(networkInfoAds[SERVICE_JSON_KEY_REWARDEDVIDEO].ToString());
                        bool interstitialAdEnabled = Boolean.Parse(networkInfoAds[SERVICE_JSON_KEY_INTERSTITIAL].ToString());
                        int adapter = serialized.FindProperty("_" + adNetworkName + "Adapter").intValue;
                        if (rewardedVideoAdEnabled || interstitialAdEnabled) {
                            if (isAndroid) {
                                if (adapter < 1) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 2;
                                } else if (adapter == 1 || adapter == 3) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 1;
                                }
                            } else {
                                if (adapter < 1) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 3;
                                } else if (adapter == 1 || adapter == 2) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 1;
                                }
                            }
                        } else {
                            if (isAndroid) {
                                if (adapter == 1) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 3;
                                } else if (adapter == 2) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 0;
                                }
                            } else {
                                if (adapter == 1) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 2;
                                } else if (adapter == 3) {
                                    serialized.FindProperty("_" + adNetworkName + "Adapter").intValue = 0;
                                }
                            }
                        }

                        if (AdiscopeAdapterSettings.ADMOB == adNetworkName) {
                            if (networkInfoSettings.ContainsKey(SERVICE_JSON_KEY_ADMOB) && networkInfoSettings[SERVICE_JSON_KEY_ADMOB] != null) {
                                string admobKey = networkInfoSettings[SERVICE_JSON_KEY_ADMOB].ToString();
                                if (admobKey != null && admobKey.Length > 0) {
                                    if (isAndroid) {
                                        serialized.FindProperty("_admobAppKey_aos").stringValue = admobKey;
                                    } else {
                                        serialized.FindProperty("_admobAppKey_ios").stringValue = admobKey;
                                    }
                                }
                            }
                        }
                        if (AdiscopeAdapterSettings.ADMANAGER == adNetworkName) {
                            string admobKey = serialized.FindProperty("_admobAppKey_aos").stringValue;
                            if (!isAndroid) {
                                admobKey = serialized.FindProperty("_admobAppKey_ios").stringValue;
                            }
                            if ((admobKey == null || admobKey.Length < 1) && networkInfoSettings != null) {
                                if (networkInfoSettings.ContainsKey(SERVICE_JSON_KEY_ADMOB) && networkInfoSettings[SERVICE_JSON_KEY_ADMOB] != null) {
                                    string admanagerKey = networkInfoSettings[SERVICE_JSON_KEY_ADMOB].ToString();
                                    if (admanagerKey != null && admanagerKey.Length > 0) {
                                        if (isAndroid) {
                                            serialized.FindProperty("_admobAppKey_aos").stringValue = admanagerKey;
                                        } else {
                                            serialized.FindProperty("_admobAppKey_ios").stringValue = admanagerKey;
                                        }
                                    }
                                }
                            }
                        }
                        if (AdiscopeAdapterSettings.MAX == adNetworkName) {
                            if(adiscopeInfoSettings != null && adiscopeInfoSettings.ContainsKey(SERVICE_JSON_KEY_ADMOB))
                            {
                                string admobKey = adiscopeInfoSettings[SERVICE_JSON_KEY_ADMOB].ToString();
                                if (admobKey != null && admobKey.Length > 0){
                                    if (isAndroid) {
                                        serialized.FindProperty("_admobAppKey_aos").stringValue = admobKey;
                                    } else {
                                        serialized.FindProperty("_admobAppKey_ios").stringValue = admobKey;
                                    }
                                }
                            }
                            if(networkInfoSettings.ContainsKey(SERVICE_JSON_KEY_AD_REVIEW) && networkInfoSettings[SERVICE_JSON_KEY_AD_REVIEW] != null)
                            {
                                string adReviewKey = networkInfoSettings[SERVICE_JSON_KEY_AD_REVIEW].ToString();
                                if (adReviewKey != null && adReviewKey.Length > 0) {
                                    if (isAndroid) {
                                        serialized.FindProperty("_applovinAdReviewKey").stringValue = adReviewKey;
                                    }
                                }
                            }
                        }
                    }
                }
                serialized.ApplyModifiedProperties();
            }
        }

        private static void UpdateLauncherGradleManifestPlaceholders(string mediaId, string mediaSecret, string subDomain)
        {
            string fullPath = Path.GetFullPath(PATH_LAUNCHER_TEMPLATE);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("[Adiscope] launcherTemplate.gradle not found: " + fullPath);
                return;
            }

            string newBlock =
                "manifestPlaceholders = [\n" +
                "            adiscope_media_id    : \"" + mediaId + "\",\n" +
                "            adiscope_media_secret: \"" + mediaSecret + "\",\n" +
                "            adiscope_sub_domain  : \"" + subDomain + "\"\n" +
                "        ]";

            string content = File.ReadAllText(fullPath);

            // 기존 manifestPlaceholders 블록이 있으면 교체, 없으면 defaultConfig 안에 삽입
            const string pattern = @"manifestPlaceholders\s*=\s*\[[^\]]*\]";
            if (Regex.IsMatch(content, pattern, RegexOptions.Singleline))
            {
                content = Regex.Replace(content, pattern, newBlock, RegexOptions.Singleline);
            }
            else
            {
                // versionName 행 바로 뒤에 삽입
                content = Regex.Replace(
                    content,
                    @"(versionName\s+'[^']*')",
                    "$1\n        " + newBlock);
            }

            File.WriteAllText(fullPath, content);
            Debug.Log("[Adiscope] launcherTemplate.gradle manifestPlaceholders updated: mediaId=" + mediaId);
        }

        private static void CreateAdiscopeFrameworksDirectory()
        {
            if (!Directory.Exists(Application.dataPath + PATH_ADISCOPE_EDITOR))
            {
                Directory.CreateDirectory(Application.dataPath + PATH_ADISCOPE_EDITOR);
            }
        }
    }

    static class AdiscopeAdapterSettings {
        public const string ADEVENT    = "adevent";
        public const string LUCKYEVENT = "luckyevent";
        public const string ADMOB      = "admob";
        public const string ADMANAGER  = "admanager";
        public const string MAX        = "max";
        private const string CHARTBOOST = "chartboost";
        private const string PANGLE     = "pangle";
        private const string VUNGLE     = "vungle";
        private const string TNKPUB     = "tnkpub";

        public static bool GetIsSetting(string network, bool isAndroid) {
            if (isAndroid) {
                switch (network) {
                    case ADEVENT:
                    case LUCKYEVENT:
                    case ADMOB:
                    case CHARTBOOST:
                    case MAX:
                    case PANGLE:
                    case VUNGLE:
                    case TNKPUB:
                    return true;
                    default: return false;
                }
            } else {
                switch (network) {
                    case ADEVENT:
                    case LUCKYEVENT:
                    case ADMANAGER:
                    case ADMOB:
                    case VUNGLE:
                    case CHARTBOOST:
                    case MAX:
                    case PANGLE:
                    case TNKPUB:
                    return true;
                    default: return false;
                }
            }
        }
    }
}