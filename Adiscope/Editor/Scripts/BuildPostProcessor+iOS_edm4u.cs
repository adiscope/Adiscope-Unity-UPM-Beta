using System;
using System.IO;
using System.Net;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Adiscope
{
    class BuildPostProcessorForIosEdm4u {
        private const string PATH_ADISCOPE_EDITOR_IOS   = "/Adiscope/AdiscopeAppSettingsFilesiOS/Editor";
        private const string DISPLAY_PROGRESS_DIALOG_TITLE = "Adiscope Install";

        public static bool CreateAdiscopeIosFiles(bool isProgress) {
            return CopyAdiscopeFrameworks(new List<AdiscopeFrameworkType>() {
                AdiscopeFrameworkType.Core,
                AdiscopeFrameworkType.AdEvent,
                AdiscopeFrameworkType.Admanager,
                AdiscopeFrameworkType.Admob,
                AdiscopeFrameworkType.Vungle,
                AdiscopeFrameworkType.ChartBoost,
                AdiscopeFrameworkType.FAN,
                AdiscopeFrameworkType.MobVista,
                AdiscopeFrameworkType.UnityAds,
                AdiscopeFrameworkType.Ironsource,
                AdiscopeFrameworkType.AppLovin,
                AdiscopeFrameworkType.Max,
                AdiscopeFrameworkType.Pangle,
                AdiscopeFrameworkType.Tnkpub
            }, isProgress);
        }
        /*** edm4u를 설정 하기 위해 adapter 파일 생성 start ***/
        private static bool CopyAdiscopeFrameworks(List<AdiscopeFrameworkType> usingFrameworks, bool isProgress) {
            DeleteAdiscopeFrameworks(usingFrameworks); // 사용 안 하는 adapter edm4u 파일 삭제
            CreateAdiscopeFrameworksDirectory(); // edm4u 파일을 copy 할 폴더 생성
            return FileDownloadEdm4uAdapter(usingFrameworks, isProgress); // edm4u 파일 다운로드
        }

        private static void DeleteAdiscopeFrameworks(List<AdiscopeFrameworkType> usingFrameworks)
        {
            string path = Application.dataPath + PATH_ADISCOPE_EDITOR_IOS;
            if (Directory.Exists(path))
            {
                foreach (string filename in Directory.GetFiles(path, "*.xml"))
                {
                    foreach (AdiscopeFrameworkType type in usingFrameworks) {
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

        private static void CreateAdiscopeFrameworksDirectory() {
            if (!Directory.Exists(Application.dataPath + PATH_ADISCOPE_EDITOR_IOS)) {
                Directory.CreateDirectory(Application.dataPath + PATH_ADISCOPE_EDITOR_IOS);
            }
        }

        private static bool FileDownloadEdm4uAdapter(List<AdiscopeFrameworkType> usingFrameworks, bool isProgress) {
            float progress = 0.3f / usingFrameworks.Count;
            float totalProgress = 0.6f + progress;
            foreach (AdiscopeFrameworkType type in usingFrameworks) {
                if (!type.GetAdapterEnable()) {
                    continue;
                }

                if (isProgress) {
                    if (EditorUtility.DisplayCancelableProgressBar(
                            DISPLAY_PROGRESS_DIALOG_TITLE,
                            "Download Adapter Files",
                            totalProgress)) {
                        EditorUtility.ClearProgressBar();
                        return false;
                    }

                    totalProgress += progress;
                }

                if (!DownloadAdapterFile(type.GetFilePath(), type.GetFileName())) {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Failed to install", "파일 생성 실패", "닫기");
                    return false;
                }
            }

            return true;
        }

        private static bool DownloadAdapterFile(string file_path, string file_name) {
            string uriString = file_path;
            uriString += file_name;
            Debug.Log("File URL : " + uriString);
            try
            {
                (new WebClient()).DownloadFile(
                    new Uri(uriString),
                    Path.Combine(Application.dataPath + PATH_ADISCOPE_EDITOR_IOS, file_name)
                );
            }
            catch (Exception exception)
            {
                Debug.LogError("failed to download adapter file: " + exception.Message);
                EditorUtility.ClearProgressBar();
                return false;
            }

            return true;
        }
        /*** edm4u를 설정 하기 위해 adapter 파일 생성 end ***/
    }

    // Adapter 제거 시 Dependencies 제를 위해 유지 해야 함
    public enum AdiscopeFrameworkType {
        Core,
        AdEvent,
        Admanager,
        Admob,
        Vungle,
        ChartBoost,
        FAN,
        MobVista,
        Ironsource,
        UnityAds,
        AppLovin,
        Max,
        Pangle,
        Tnkpub
    }

    static class AdiscopeFrameworkTypeExtension {

        private const string CORE_FILE_NAME         = "AdiscopeIosDependencies.xml";
        private const string ADEVENT_FILE_NAME      = "AdEventIosDependencies.xml";
        private const string ADMANAGER_FILE_NAME    = "AdmanagerIosDependencies.xml";
        private const string ADMOB_FILE_NAME        = "AdmobIosDependencies.xml";
        private const string VUNGLE_FILE_NAME       = "VungleIosDependencies.xml";
        private const string CHARTBOOST_FILE_NAME   = "ChartboostIosDependencies.xml";
        private const string FAN_FILE_NAME          = "FanIosDependencies.xml";
        private const string MOBVISTA_FILE_NAME     = "MobvistaIosDependencies.xml";
        private const string IRONSOURCE_FILE_NAME   = "IronsourceIosDependencies.xml";
        private const string UNITYADS_FILE_NAME     = "UnityadsIosDependencies.xml";
        private const string APPLOVIN_FILE_NAME     = "ApplovinIosDependencies.xml";
        private const string MAX_FILE_NAME          = "MaxIosDependencies.xml";
        private const string PANGLE_FILE_NAME       = "PangleIosDependencies.xml";
        private const string TNKPUB_FILE_NAME       = "TnkpubIosDependencies.xml";

        // private const string ADISCOPE_FILE_PATH     = "https://github.com/adiscope/Adiscope-iOS-Sample/releases/download/";
        private const string ADISCOPE_FILE_PATH     = "https://github.com/adiscope/Adiscope-Unity-UPM-Beta/releases/download/";
        private const string CORE_FILE_PATH         = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string ADMANAGER_FILE_PATH    = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string ADMOB_FILE_PATH        = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string VUNGLE_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string CHARTBOOST_FILE_PATH   = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string MAX_FILE_PATH          = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string PANGLE_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";
        private const string TNKPUB_FILE_PATH       = ADISCOPE_FILE_PATH + "5.2.0/";

        public static string GetFileName(this AdiscopeFrameworkType type) {
            switch (type) {
                case AdiscopeFrameworkType.Core:            return CORE_FILE_NAME;
                case AdiscopeFrameworkType.AdEvent:         return ADEVENT_FILE_NAME;
                case AdiscopeFrameworkType.Admanager:       return ADMANAGER_FILE_NAME;
                case AdiscopeFrameworkType.Admob:           return ADMOB_FILE_NAME;
                case AdiscopeFrameworkType.Vungle:          return VUNGLE_FILE_NAME;
                case AdiscopeFrameworkType.ChartBoost:      return CHARTBOOST_FILE_NAME;
                case AdiscopeFrameworkType.FAN:             return FAN_FILE_NAME;
                case AdiscopeFrameworkType.MobVista:        return MOBVISTA_FILE_NAME;
                case AdiscopeFrameworkType.Ironsource:      return IRONSOURCE_FILE_NAME;
                case AdiscopeFrameworkType.UnityAds:        return UNITYADS_FILE_NAME;
                case AdiscopeFrameworkType.AppLovin:        return APPLOVIN_FILE_NAME;
                case AdiscopeFrameworkType.Max:             return MAX_FILE_NAME;
                case AdiscopeFrameworkType.Pangle:          return PANGLE_FILE_NAME;
                case AdiscopeFrameworkType.Tnkpub:          return TNKPUB_FILE_NAME;
                default:                                    return null;
            }
        }

        public static string GetFilePath(this AdiscopeFrameworkType type) {
            switch (type)
            {
                case AdiscopeFrameworkType.Core:            return CORE_FILE_PATH;
                case AdiscopeFrameworkType.Admanager:       return ADMANAGER_FILE_PATH;
                case AdiscopeFrameworkType.Admob:           return ADMOB_FILE_PATH;
                case AdiscopeFrameworkType.Vungle:          return VUNGLE_FILE_PATH;
                case AdiscopeFrameworkType.ChartBoost:      return CHARTBOOST_FILE_PATH;
                case AdiscopeFrameworkType.Max:             return MAX_FILE_PATH;
                case AdiscopeFrameworkType.Pangle:          return PANGLE_FILE_PATH;
                case AdiscopeFrameworkType.Tnkpub:          return TNKPUB_FILE_PATH;
                default:                                    return null;
            }
        }

        public static bool GetAdapterEnable(this AdiscopeFrameworkType type) {
            var settings = FrameworkSettingsRegister.Load();
            var serialized = new SerializedObject(settings);

            switch (type) {
                case AdiscopeFrameworkType.Core:            return true;
                case AdiscopeFrameworkType.Admanager:       return (serialized.FindProperty("_admanagerAdapter").intValue == 1 || serialized.FindProperty("_admanagerAdapter").intValue == 3);
                case AdiscopeFrameworkType.Admob:           return (serialized.FindProperty("_admobAdapter").intValue == 1 || serialized.FindProperty("_admobAdapter").intValue == 3);
                case AdiscopeFrameworkType.Vungle:          return (serialized.FindProperty("_vungleAdapter").intValue == 1 || serialized.FindProperty("_vungleAdapter").intValue == 3);
                case AdiscopeFrameworkType.ChartBoost:      return (serialized.FindProperty("_chartboostAdapter").intValue == 1 || serialized.FindProperty("_chartboostAdapter").intValue == 3);
                case AdiscopeFrameworkType.Max:             return (serialized.FindProperty("_maxAdapter").intValue == 1 || serialized.FindProperty("_maxAdapter").intValue == 3);
                case AdiscopeFrameworkType.Pangle:          return (serialized.FindProperty("_pangleAdapter").intValue == 1 || serialized.FindProperty("_pangleAdapter").intValue == 3);
                case AdiscopeFrameworkType.Tnkpub:          return (serialized.FindProperty("_tnkpubAdapter").intValue == 1 || serialized.FindProperty("_tnkpubAdapter").intValue == 3);
                default: return false;
            }
        }
    }
}
