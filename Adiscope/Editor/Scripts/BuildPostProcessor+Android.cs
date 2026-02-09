using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using Adiscope.Editor;
using System.Linq;

namespace Adiscope
{
    class BuildPostProcessorForAndroid
    {
        #region CONST VARIABLES
        private const string PATH_ADISCOPE_FILES        = "/Adiscope/AdiscopeAppSettingsFiles";
        private const string PATH_ADISCOPE_EDITOR       = "/Adiscope/AdiscopeAppSettingsFiles/Editor";
        private const string PATH_ADISCOPE_LIB          = "/Adiscope/AdiscopeAppSettingsFiles/Plugins/Adiscope.androidlib";
        private const string PATH_ADISCOPE_MANIFEST     = "/Adiscope/AdiscopeAppSettingsFiles/Plugins/Adiscope.androidlib/src/main";
        private const string DISPLAY_PROGRESS_DIALOG_TITLE = "Adiscope Install";
        private const string PROJECT_PROPERTIES_CONTENT = "android.library=true";
        #endregion

        public static bool CreateAdiscopeAndroidFiles(bool isProgress)
        {
            bool isFrameworks = CopyAdiscopeFrameworks(
                new List<AdiscopeFrameworkAndroidType>()
                {
                    AdiscopeFrameworkAndroidType.Admob,
                    AdiscopeFrameworkAndroidType.ChartBoost,
                    AdiscopeFrameworkAndroidType.Ironsource,
                    AdiscopeFrameworkAndroidType.UnityAds,
                    AdiscopeFrameworkAndroidType.MAX,
                    AdiscopeFrameworkAndroidType.AppLovin,
                    AdiscopeFrameworkAndroidType.FAN,
                    AdiscopeFrameworkAndroidType.MobVista,
                    AdiscopeFrameworkAndroidType.Pangle,
                    AdiscopeFrameworkAndroidType.Vungle,
                    AdiscopeFrameworkAndroidType.Inmobi,
                    AdiscopeFrameworkAndroidType.Smaato,
                    AdiscopeFrameworkAndroidType.Tapjoy,
                    AdiscopeFrameworkAndroidType.Tnkpub
                }
            , isProgress);
            bool isUpdateManifest = UpdateAndroidManifest(isProgress);
            bool isUpdateProperties = UpdateProperties(isProgress);
            bool isUpdateBuildGradle = UpdateBuildGradle(isProgress);
            bool isUpdateLauncherGradle = UpdateLauncherGradle(isProgress);

            return isFrameworks
                && isUpdateManifest
                && isUpdateProperties
                && isUpdateBuildGradle
                && isUpdateLauncherGradle;
        }

        private static bool UpdateLauncherGradle(bool isProgress)
        {
            if (isProgress)
            {
                if (
                    EditorUtility.DisplayCancelableProgressBar(
                        DISPLAY_PROGRESS_DIALOG_TITLE,
                        "Update build.gradle",
                        0.85f
                    )
                )
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }
            }

            string launcherTemplatePath = Path.Combine(
                Application.dataPath,
                "Plugins/Android/launcherTemplate.gradle"
            );

            if (!File.Exists(launcherTemplatePath))
            {
                // todo launcherTemplate 파일 생성
                Debug.Log("create launcherTemplate.gradle");

                // 디렉토리가 없으면 생성
                string directory = Path.GetDirectoryName(launcherTemplatePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string templateContent = LauncherTemplateProvider.GetDefaultLauncherTemplate();
                File.WriteAllText(launcherTemplatePath, templateContent);
                AssetDatabase.Refresh();
            }

            var content = File.ReadAllText(launcherTemplatePath);
            bool modified = false;

            int minSdk = (int)PlayerSettings.Android.minSdkVersion;

            if (minSdk >= 26)
            {
                var lines = File.ReadAllLines(launcherTemplatePath).ToList();
                string[] removeKeywords =
                {
                    "coreLibraryDesugaring ",
                    "coreLibraryDesugaringEnabled"
                };

                int beforeCount = lines.Count;

                lines = lines
                    .Where(line => !removeKeywords.Any(keyword => line.Contains(keyword)))
                    .ToList();

                int afterCount = lines.Count;

                if (afterCount != beforeCount)
                {
                    File.WriteAllLines(launcherTemplatePath, lines);
                    modified = true;
                }
            }
            else
            {
                if (!content.Contains("coreLibraryDesugaring "))
                {
                    content = content.Replace(
                        "dependencies {",
                        "dependencies {\n    coreLibraryDesugaring \"com.android.tools:desugar_jdk_libs:2.0.4\""
                    );
                    modified = true;
                }

                if (!content.Contains("coreLibraryDesugaringEnabled"))
                {
                    int targetSdk = (int)PlayerSettings.Android.targetSdkVersion;
                    string jdkVersion =
                        targetSdk >= 35 ? "JavaVersion.VERSION_17" : "JavaVersion.VERSION_11";
                    content = content.Replace(
                        $"targetCompatibility {jdkVersion}",
                        $"targetCompatibility {jdkVersion}\n        coreLibraryDesugaringEnabled true"
                    );
                    modified = true;
                }

                if (modified)
                {
                    File.WriteAllText(launcherTemplatePath, content);
                }
            }

            if (modified)
            {
                Debug.Log("launcherTemplate.gradle modified");
                AssetDatabase.Refresh();
            }

            return true;
        }

        private static bool UpdateBuildGradle(bool isProgress)
        {
            if (isProgress)
            {
                if (
                    EditorUtility.DisplayCancelableProgressBar(
                        DISPLAY_PROGRESS_DIALOG_TITLE,
                        "Update build.gradle",
                        0.7f
                    )
                )
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }
            }

            int targetSdk = (int) PlayerSettings.Android.targetSdkVersion;
            string jdkVersion = targetSdk >= 35 ? "JavaVersion.VERSION_17" : "JavaVersion.VERSION_11";
            string gradleFilePath = Application.dataPath + PATH_ADISCOPE_LIB;
            gradleFilePath += "/build.gradle";

            string gradleContent = $@"
plugins {{
    id 'com.android.library'
}}

android {{
    namespace 'com.nps.adiscope'
    compileSdkVersion {targetSdk}

    defaultConfig {{
        minSdkVersion 23
        targetSdkVersion {targetSdk}
    }}

    compileOptions {{
        sourceCompatibility {jdkVersion}
        targetCompatibility {jdkVersion}
    }}
}}
";

            try {
                File.WriteAllText(gradleFilePath, gradleContent);
            } catch (Exception e) {
                Debug.LogError("failed to write file: " + gradleFilePath);
                Debug.LogError("" + e);
                return false;
            }

            return true;
        }

        /*** properties 파일 생성 start ***/
        private static bool UpdateProperties(bool isProgress) {
            if (isProgress) {
                if (EditorUtility.DisplayCancelableProgressBar(
                        DISPLAY_PROGRESS_DIALOG_TITLE,
                        "Update project.properties",
                        0.6f
                    )){
                    EditorUtility.ClearProgressBar();
                    return false;
                }
            }

            string propertiesPath = Application.dataPath + PATH_ADISCOPE_LIB;
            propertiesPath += "/project.properties";                         // 파일명 지정

            try {
                File.WriteAllText(propertiesPath, PROJECT_PROPERTIES_CONTENT);
            } catch (Exception e) {
                Debug.LogError("failed to write file: " + propertiesPath);
                Debug.LogError("" + e);
                return false;
            }

            return true;
        }
        /*** properties 파일 생성 end ***/

        /*** AndroidManifest 파일 생성 start ***/
        private static bool UpdateAndroidManifest(bool isProgress)
        {
            if (isProgress) {
                if (EditorUtility.DisplayCancelableProgressBar(
                        DISPLAY_PROGRESS_DIALOG_TITLE,
                        "Update AndroidManifest.xml",
                        0.5f
                    )){
                    EditorUtility.ClearProgressBar();
                    return false;
                }
            }

            ManifestHandler manifestHandler = GetManifestHandler();     // Manifest에 필수 항목 추가
            string manifestPath = CreateAdiscopeManifestDirectory();    // 폴더 생성
            manifestPath += "/AndroidManifest.xml";                     // 파일명 지정

            string legacyManifestPath = Application.dataPath + PATH_ADISCOPE_LIB + "/AndroidManifest.xml";
            if (File.Exists(legacyManifestPath))
            {
                File.Delete(legacyManifestPath);
            }

            return manifestHandler.WriteXmlFile(manifestPath);
        }

        private static ManifestHandler GetManifestHandler()
        {
            ManifestHandler handler = new ManifestHandler("com.nps.adiscope");
            handler.AddMetaData(
                "<meta-data android:name=\"adiscope_media_id\" android:value=\"" + GetMediaId_AOS() + "\" />"
            );
            handler.AddMetaData(
                "<meta-data android:name=\"adiscope_media_secret\" android:value=\"" + GetMediaSecret_AOS() + "\" />"
            );
            handler.AddMetaData(
                "<meta-data android:name=\"adiscope_sub_domain\" android:value=\"" + GetSubDomain() + "\" />"
            );
            
            AdiscopeFrameworkAndroidType admobFramework = AdiscopeFrameworkAndroidType.Admob;
            AdiscopeFrameworkAndroidType maxFramework = AdiscopeFrameworkAndroidType.MAX;
            if(admobFramework.GetAdapterEnable() || maxFramework.GetAdapterEnable()){
                handler.AddMetaData(
                    "<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"" + GetGoogleAdsApplicationId() + "\" />"
                );
            }
            
            handler.AddMetaData(
                "<meta-data android:name=\"adiscope_unity_sdk_version\" android:value=\"" + GetUnityVersion() + "\" />"
            );
            handler.AddMetaData(
                "<meta-data android:name=\"unity_runtime_version\" android:value=\"" + Application.unityVersion + "\" />"
            );

            // 상세 이동을 위해서 추가
            handler.AddActivity(
                    "<activity android:name=\"com.nps.adiscope.core.offerwall.adv.act.AdvancedOfferwallActivity\" android:configChanges=\"orientation|screenSize|keyboardHidden\" " +
                    "android:exported=\"true\" android:theme=\"@android:style/Theme.NoTitleBar\" android:screenOrientation=\"portrait\" android:hardwareAccelerated=\"true\" >" +
                        "<intent-filter>" +
                            "<action android:name=\"android.intent.action.VIEW\"/>" +
                            "<category android:name=\"android.intent.category.DEFAULT\"/>" +
                            "<category android:name=\"android.intent.category.BROWSABLE\"/>" +
                            "<data android:host=\"*.adiscope.com\" android:pathPrefix=\"/" + GetMediaId_AOS() + "\" android:scheme=\"adiscope" + GetSubDomain() + "\"/>" +
                        "</intent-filter>" +
                    "</activity>"
            );

            return handler;
        }

        private static string CreateAdiscopeManifestDirectory()
        {
            string manifestPath = Application.dataPath + PATH_ADISCOPE_MANIFEST;
            if (!Directory.Exists(manifestPath))
            {
                Directory.CreateDirectory(manifestPath);
            }
            return manifestPath;
        }

        private static string GetMediaId_AOS() {
            var serialized = GetSettingsRegisterSerializedObject();
            return serialized.FindProperty("_mediaID_aos").stringValue;
        }

        private static string GetMediaSecret_AOS() {
            var serialized = GetSettingsRegisterSerializedObject();
            return serialized.FindProperty("_mediaSecret_aos").stringValue;
        }

        private static string GetSubDomain() {
            var serialized = GetSettingsRegisterSerializedObject();
            return serialized.FindProperty("_subDomain").stringValue;
        }

        private static string GetGoogleAdsApplicationId() {
            var serialized = GetSettingsRegisterSerializedObject();
            return serialized.FindProperty("_admobAppKey_aos").stringValue;
        }

        private static SerializedObject GetSettingsRegisterSerializedObject() {
            var settings = FrameworkSettingsRegister.Load();
            return new SerializedObject(settings);
        }

        private static string GetUnityVersion() {
            string filePath = "Packages/com.tnk.adiscope/package.json";
            string json = File.ReadAllText(filePath);
            ParsingPackageJson.PackageJson pj = JsonUtility.FromJson<ParsingPackageJson.PackageJson>(json);
            return pj.version;
        }
        /*** AndroidManifest 파일 생성 end ***/

        /*** edm4u를 설정 하기 위해 adapter 파일 생성 start ***/
        private static bool CopyAdiscopeFrameworks(List<AdiscopeFrameworkAndroidType> usingFrameworks, bool isProgress)
        {
            DeleteAdiscopeFrameworks(usingFrameworks); // 사용 안 하는 adapter edm4u 파일 삭제
            CreateAdiscopeFrameworksDirectory(); // edm4u 파일을 copy 할 폴더 생성
            return FileDownloadEdm4uAdapter(usingFrameworks, isProgress); // edm4u 파일 다운로드
        }

        private static void DeleteAdiscopeFrameworks(List<AdiscopeFrameworkAndroidType> usingFrameworks)
        {
            string path = Application.dataPath + PATH_ADISCOPE_EDITOR;
            if (Directory.Exists(path))
            {
                foreach (string filename in Directory.GetFiles(path, "*.xml"))
                {
                    foreach (AdiscopeFrameworkAndroidType type in usingFrameworks) {
                        if (filename.Contains(type.GetFileName())) {
                            try
                            {
                                File.Delete(filename);
                            }
                            catch (IOException)
                            {
                                File.Delete(filename);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                File.Delete(filename);
                            }

                            string metaFilename = filename + ".meta";
                            if (File.Exists(metaFilename)) {
                                try
                                {
                                    File.Delete(metaFilename);
                                }
                                catch (IOException)
                                {
                                    File.Delete(metaFilename);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    File.Delete(metaFilename);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CreateAdiscopeFrameworksDirectory()
        {
            if (!Directory.Exists(Application.dataPath + PATH_ADISCOPE_EDITOR))
            {
                Directory.CreateDirectory(Application.dataPath + PATH_ADISCOPE_EDITOR);
            }
        }

        private static bool FileDownloadEdm4uAdapter(List<AdiscopeFrameworkAndroidType> usingFrameworks, bool isProgress)
        {
            float progress = 0.4f / usingFrameworks.Count;
            float totalProgress = 0.1f + progress;
            foreach (AdiscopeFrameworkAndroidType type in usingFrameworks)
            {
                Debug.Log("Create : " + type.GetFileName() + " / enable : " + type.GetAdapterEnable());
                if (!type.GetAdapterEnable())
                {
                    continue;
                }

                if (isProgress) {
                    if (EditorUtility.DisplayCancelableProgressBar(
                            DISPLAY_PROGRESS_DIALOG_TITLE,
                            "Download Adapter Files",
                            totalProgress))
                    {
                        EditorUtility.ClearProgressBar();
                        return false;
                    }

                    totalProgress += progress;
                }

                if(!DownloadAdapterFile(type.GetFilePath(), type.GetFileName())){
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Failed to install", "파일 생성 실패", "닫기");
                    return false;
                }
            }

            return true;
        }

        private static bool DownloadAdapterFile(string file_path, string file_name)
        {
            string uriString = file_path;
            uriString += file_name;
            try
            {
                Debug.Log("DownloadAdapterFile : " + uriString);
                (new WebClient()).DownloadFile(
                    new Uri(uriString),
                    Path.Combine(Application.dataPath + PATH_ADISCOPE_EDITOR, file_name)
                );
            }
            catch (Exception exception) { 
                Debug.LogError("failed to download adapter file: " + exception.Message);
                EditorUtility.ClearProgressBar();
                return false;
            }

            return true;
        }
        /*** edm4u를 설정 하기 위해 adapter 파일 생성 end ***/
    }

    // Adapter 제거 시 Dependencies 제를 위해 유지 해야 함
    public enum AdiscopeFrameworkAndroidType
    {
        Admob,
        ChartBoost,
        Ironsource,
        UnityAds,
        MAX,
        AppLovin,
        FAN,
        MobVista,
        Pangle,
        Vungle,
        Inmobi,
        Smaato,
        Tapjoy,
        Tnkpub
    }

    static class AdiscopeFrameworkAndroidTypeExtension
    {
        private const string ADMOB_FILE_NAME        = "AdmobDependencies.xml";
        private const string CHARTBOOST_FILE_NAME   = "ChartboostDependencies.xml";
        private const string IRONSOURCE_FILE_NAME   = "IronsourceDependencies.xml";
        private const string UNITYADS_FILE_NAME     = "UnityadsDependencies.xml";
        private const string MAX_FILE_NAME          = "MaxDependencies.xml";
        private const string APPLOVIN_FILE_NAME     = "ApplovinDependencies.xml";
        private const string FAN_FILE_NAME          = "FanDependencies.xml";
        private const string MOBVISTA_FILE_NAME     = "MobvistaDependencies.xml";
        private const string PANGLE_FILE_NAME       = "PangleDependencies.xml";
        private const string VUNGLE_FILE_NAME       = "VungleDependencies.xml";
        private const string INMOBI_FILE_NAME       = "InmobiDependencies.xml";
        private const string SMAATO_FILE_NAME       = "SmaatoDependencies.xml";
        private const string TAPJOY_FILE_NAME       = "TapjoyDependencies.xml";
        private const string TNKPUB_FILE_NAME       = "TnkpubDependencies.xml";


        // private const string ADISCOPE_FILE_PATH = "https://github.com/adiscope/Adiscope-Android-Sample/releases/download/";
        private const string ADISCOPE_FILE_PATH = "https://github.com/adiscope/Adiscope-Unity-UPM-Beta/releases/download/";
        private const string ADMOB_FILE_PATH        = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string CHARTBOOST_FILE_PATH   = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string MAX_FILE_PATH          = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string PANGLE_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string VUNGLE_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string TNKPUB_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";

        public static string GetFileName(this AdiscopeFrameworkAndroidType type)
        {
            switch (type)
            {
                case AdiscopeFrameworkAndroidType.Admob:        return ADMOB_FILE_NAME;
                case AdiscopeFrameworkAndroidType.ChartBoost:   return CHARTBOOST_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Ironsource:   return IRONSOURCE_FILE_NAME;
                case AdiscopeFrameworkAndroidType.UnityAds:     return UNITYADS_FILE_NAME;
                case AdiscopeFrameworkAndroidType.MAX:          return MAX_FILE_NAME;
                case AdiscopeFrameworkAndroidType.AppLovin:     return APPLOVIN_FILE_NAME;
                case AdiscopeFrameworkAndroidType.FAN:          return FAN_FILE_NAME;
                case AdiscopeFrameworkAndroidType.MobVista:     return MOBVISTA_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Pangle:       return PANGLE_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Vungle:       return VUNGLE_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Inmobi:       return INMOBI_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Smaato:       return SMAATO_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Tapjoy:       return TAPJOY_FILE_NAME;
                case AdiscopeFrameworkAndroidType.Tnkpub:       return TNKPUB_FILE_NAME;
                default:                                        return null;
            }
        }

        public static string GetFilePath(this AdiscopeFrameworkAndroidType type)
        {
            switch (type)
            {
                case AdiscopeFrameworkAndroidType.Admob:        return ADMOB_FILE_PATH;
                case AdiscopeFrameworkAndroidType.ChartBoost:   return CHARTBOOST_FILE_PATH;
                case AdiscopeFrameworkAndroidType.MAX:          return MAX_FILE_PATH;
                case AdiscopeFrameworkAndroidType.Pangle:       return PANGLE_FILE_PATH;
                case AdiscopeFrameworkAndroidType.Vungle:       return VUNGLE_FILE_PATH;
                case AdiscopeFrameworkAndroidType.Tnkpub:       return TNKPUB_FILE_PATH;
                default:                                        return null;
            }
        }

        public static bool GetAdapterEnable(this AdiscopeFrameworkAndroidType type)
        {
            var settings = FrameworkSettingsRegister.Load();
            var serialized = new SerializedObject(settings);

            switch (type)
            {
                case AdiscopeFrameworkAndroidType.Admob:        return (serialized.FindProperty("_admobAdapter").intValue == 1 || serialized.FindProperty("_admobAdapter").intValue == 2);
                case AdiscopeFrameworkAndroidType.ChartBoost:   return (serialized.FindProperty("_chartboostAdapter").intValue == 1 || serialized.FindProperty("_chartboostAdapter").intValue == 2);
                case AdiscopeFrameworkAndroidType.MAX:          return (serialized.FindProperty("_maxAdapter").intValue == 1 || serialized.FindProperty("_maxAdapter").intValue == 2);
                case AdiscopeFrameworkAndroidType.Pangle:       return (serialized.FindProperty("_pangleAdapter").intValue == 1 || serialized.FindProperty("_pangleAdapter").intValue == 2);
                case AdiscopeFrameworkAndroidType.Vungle:       return (serialized.FindProperty("_vungleAdapter").intValue == 1 || serialized.FindProperty("_vungleAdapter").intValue == 2);
                case AdiscopeFrameworkAndroidType.Tnkpub:       return (serialized.FindProperty("_tnkpubAdapter").intValue == 1 || serialized.FindProperty("_tnkpubAdapter").intValue == 2);;
                default:                                        return false;
            }
        }
    }
}
