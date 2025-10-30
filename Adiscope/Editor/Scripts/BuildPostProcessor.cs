#if UNITY_IOS || UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

#if UNITY_ANDROID
using UnityEditor.Android;
using System.Text.RegularExpressions;
#endif

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

public class BuildPostProcessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        switch (target)
        {
#if UNITY_IOS
            case BuildTarget.iOS:
                Adiscope.PostProcessor.BuildPostProcessorForIos.OnPostProcessBuild(path);
                break;
#endif
            case BuildTarget.Android:
                break;
            default:
                break;
        }
    }

    [PostProcessBuild(999)]
    public static void OnPostProcessLastBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.Android)
        {
            string launcherTemplatePath = Path.Combine(
                Application.dataPath,
                "Plugins/Android/launcherTemplate.gradle"
            );
            if (!File.Exists(launcherTemplatePath))
            {
                Debug.Log($"not found launcher build.gradle, {launcherTemplatePath}");
                return;
            }

            var content = File.ReadAllText(launcherTemplatePath);
            bool modified = false;

            // minSdk 값 파싱
            int minSdk = (int)PlayerSettings.Android.minSdkVersion;

            if (minSdk >= 26)
                return;

            Debug.Log($"[BuildPostProcessor] minSdk: {minSdk}");

            // dependencies 블록 수정
            if (!content.Contains("coreLibraryDesugaring "))
            {
                content = content.Replace(
                    "dependencies {",
                    "dependencies {\n    coreLibraryDesugaring \"com.android.tools:desugar_jdk_libs:2.0.4\""
                );
                modified = true;
            }

            // compileOptions 블록 수정
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

            File.WriteAllText(launcherTemplatePath, content);

            if (modified)
            {
                File.WriteAllText(launcherTemplatePath, content);
                Debug.Log("[BuildPostProcessor] Existing launcherTemplate.gradle modified");
                AssetDatabase.Refresh();
            }
            return;
        }

        if (target != BuildTarget.iOS)
            return;
#if UNITY_IOS
        // DT_TOOLCHAIN_DIR 치환
        var xcconfigFiles = Directory.GetFiles(path, "*.xcconfig", SearchOption.AllDirectories);
        foreach (var file in xcconfigFiles)
        {
            var content = File.ReadAllText(file);
            if (content.Contains("DT_TOOLCHAIN_DIR"))
            {
                content = content.Replace("DT_TOOLCHAIN_DIR", "TOOLCHAIN_DIR");
                File.WriteAllText(file, content);
            }
        }
#endif
    }
}
#endif
