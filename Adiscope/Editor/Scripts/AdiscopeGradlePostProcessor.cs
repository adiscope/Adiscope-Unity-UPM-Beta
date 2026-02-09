using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Text.RegularExpressions;

namespace Adiscope.PostProcessor
{
    /// <summary>
    /// Gradle 빌드 전에 템플릿 파일을 수정하는 PreProcessor
    /// IPreprocessBuildWithReport를 사용하여 템플릿 파일을 직접 수정
    /// </summary>
    public class AdiscopeGradlePostProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return;

            Debug.Log("[AdiscopeGradlePostProcessor] Preprocessing Android build templates");

            // AppLovin Ad Review 설정 확인
            var settings = Adiscope.FrameworkSettingsRegister.Load();
            var serialized = new SerializedObject(settings);
            bool applovinAdReview = serialized.FindProperty("_applovinAdReview").boolValue;
            string applovinAdReviewKey = serialized.FindProperty("_applovinAdReviewKey").stringValue;
            bool buildAppBundle = EditorUserBuildSettings.buildAppBundle;

            Debug.Log($"[AdiscopeGradlePostProcessor] AppLovin Ad Review: {applovinAdReview}, Build AAB: {buildAppBundle}");
            if(applovinAdReview && string.IsNullOrEmpty(applovinAdReviewKey)) Debug.Log($"[AdiscopeGradlePostProcessor] AppLovin Ad Review key is empty.");

            // 프로젝트 내 gradle 파일 수정
            ProcessLauncherTemplate(applovinAdReview, buildAppBundle, applovinAdReviewKey);
            ProcessBaseProjectTemplate(applovinAdReview, buildAppBundle, string.IsNullOrEmpty(applovinAdReviewKey));
            ProcessSettingsTemplate(applovinAdReview, buildAppBundle, string.IsNullOrEmpty(applovinAdReviewKey));
        }

        private void ProcessLauncherTemplate(bool applovinAdReview, bool buildAppBundle, string applovinAdReviewKey)
        {
            string templatePath = Path.Combine(Application.dataPath, "Plugins/Android/launcherTemplate.gradle");

            if (!File.Exists(templatePath))
            {
                Debug.LogWarning($"[AdiscopeGradlePostProcessor] launcherTemplate.gradle not found at {templatePath}");
                return;
            }

            string content = File.ReadAllText(templatePath);
            bool modified = false;

            // AppLovin Ad Review 추가 또는 제거
            if (applovinAdReview && buildAppBundle && !string.IsNullOrEmpty(applovinAdReviewKey))
            {
                // 추가
                bool needsPlugin = !content.Contains("apply plugin: 'applovin-quality-service'");
                bool needsBlock = !content.Contains("applovin {");

                if (needsPlugin || needsBlock)
                {
                    if (needsPlugin)
                    {
                        // apply plugin 추가
                        content = content.Replace(
                            "apply plugin: 'com.android.application'",
                            "apply plugin: 'com.android.application'\napply plugin: 'applovin-quality-service'"
                        );
                    }

                    if (needsBlock)
                    {
                        // applovin {} 블록 추가
                        string applovinBlock = $@"

applovin {{
  apiKey ""{applovinAdReviewKey}""
}}
";
                        // apply plugin: 'applovin-quality-service' 다음에 추가
                        content = content.Replace(
                            "apply plugin: 'applovin-quality-service'",
                            "apply plugin: 'applovin-quality-service'" + applovinBlock
                        );
                    }

                    modified = true;
                    Debug.Log("[AdiscopeGradlePostProcessor] AppLovin Ad Review added to launcherTemplate.gradle");
                }
            }
            else
            {
                // 제거
                if (content.Contains("applovin-quality-service") || content.Contains("applovin {"))
                {
                    // 패턴 1: apply plugin + 빈 줄 + applovin 블록
                    content = Regex.Replace(content,
                        @"\napply plugin: 'applovin-quality-service'\n\napplovin \{[^}]*\}\n",
                        "\n", RegexOptions.Singleline);

                    // 패턴 2: apply plugin 없이 빈 줄 + applovin 블록만 있는 경우
                    content = Regex.Replace(content,
                        @"\n\napplovin \{[^}]*\}\n",
                        "\n", RegexOptions.Singleline);

                    // 패턴 3: applovin 블록만 있는 경우 (앞뒤 빈 줄 처리)
                    content = Regex.Replace(content,
                        @"\napplovin \{[^}]*\}\n",
                        "\n", RegexOptions.Singleline);

                    // 패턴 4: apply plugin 라인만 남은 경우
                    content = Regex.Replace(content,
                        @"\napply plugin: 'applovin-quality-service'\n",
                        "\n");

                    // 패턴 5: 마지막 정리 - 남아있는 조각들
                    content = content.Replace("apply plugin: 'applovin-quality-service'\n", "");
                    content = content.Replace("apply plugin: 'applovin-quality-service'", "");

                    modified = true;
                    Debug.Log("[AdiscopeGradlePostProcessor] AppLovin Ad Review removed from launcherTemplate.gradle");
                }
            }

            if (modified)
            {
                File.WriteAllText(templatePath, content);
            }
        }

        private void ProcessBaseProjectTemplate(bool applovinAdReview, bool buildAppBundle, bool emptyKey)
        {
            string templatePath = Path.Combine(Application.dataPath, "Plugins/Android/baseProjectTemplate.gradle");

            if (!File.Exists(templatePath))
            {
                Debug.LogWarning($"[AdiscopeGradlePostProcessor] baseProjectTemplate.gradle not found at {templatePath}");
                return;
            }

            string content = File.ReadAllText(templatePath);
            bool modified = false;

            if (applovinAdReview && buildAppBundle && !emptyKey)
            {
                // AppLovin Ad Review 플러그인 추가
                if (!content.Contains("id 'com.applovin.quality'"))
                {
                    var agpVersionMatch = Regex.Match(content, @"id 'com\.android\.library' version '([^']+)' apply false");
                    string agpVersion = agpVersionMatch.Success ? agpVersionMatch.Groups[1].Value : "8.9.2";
                    string oldLine = $"id 'com.android.library' version '{agpVersion}' apply false";
                    string newLine = $"id 'com.android.library' version '{agpVersion}' apply false\n    id 'com.applovin.quality' version '+' apply false";

                    content = content.Replace(oldLine, newLine);
                    modified = true;
                    Debug.Log($"[AdiscopeGradlePostProcessor] AppLovin Ad Review added to baseProjectTemplate.gradle");
                }
            }
            else
            {
                // 제거
                if (content.Contains("id 'com.applovin.quality'"))
                {
                    // 플러그인 제거
                    content = Regex.Replace(content, @"    id 'com\.applovin\.quality' version '\+' apply false\n", "");
                    modified = true;
                    Debug.Log("[AdiscopeGradlePostProcessor] AppLovin Quality plugin removed from baseProjectTemplate.gradle");
                }
            }

            if (modified)
            {
                File.WriteAllText(templatePath, content);
            }
        }

        private void ProcessSettingsTemplate(bool applovinAdReview, bool buildAppBundle, bool emptyKey)
        {
            string templatePath = Path.Combine(Application.dataPath, "Plugins/Android/settingsTemplate.gradle");

            if (!File.Exists(templatePath))
            {
                Debug.LogWarning($"[AdiscopeGradlePostProcessor] settingsTemplate.gradle not found at {templatePath}");
                return;
            }

            string content = File.ReadAllText(templatePath);
            bool modified = false;

            if (applovinAdReview && buildAppBundle && !emptyKey)
            {
                // pluginManagement의 repositories에 AppLovin repository 추가
                if (!content.Contains("artifacts.applovin.com"))
                {
                    var pluginMgmtMatch = Regex.Match(content, @"(pluginManagement \{[\s\S]*?repositories \{[\s\S]*?)(mavenCentral\(\))([\s\S]*?\}[\s\S]*?\})");
                    if (pluginMgmtMatch.Success)
                    {
                        string before = pluginMgmtMatch.Groups[1].Value;
                        string mavenCentral = pluginMgmtMatch.Groups[2].Value;
                        string after = pluginMgmtMatch.Groups[3].Value;

                        string replacement = before + mavenCentral + "\n        maven { url 'https://artifacts.applovin.com/android' }" + after;
                        content = content.Replace(pluginMgmtMatch.Value, replacement);
                        modified = true;
                        Debug.Log("[AdiscopeGradlePostProcessor] AppLovin Ad Review repository added to settingsTemplate.gradle (pluginManagement)");
                    }
                }
            }
            else
            {
                // 제거
                if (content.Contains("artifacts.applovin.com"))
                {
                    content = Regex.Replace(content, @"\n        maven \{ url 'https://artifacts\.applovin\.com/android' \}", "");
                    modified = true;
                    Debug.Log("[AdiscopeGradlePostProcessor] AppLovin Ad Review repository removed from settingsTemplate.gradle");
                }
            }

            if (modified)
            {
                File.WriteAllText(templatePath, content);
            }
        }
    }
}
