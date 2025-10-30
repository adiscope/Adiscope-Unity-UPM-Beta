namespace Adiscope
{
    public static class LauncherTemplateProvider
    {
        public static string GetDefaultLauncherTemplate()
        {
#if UNITY_6000_0_OR_NEWER
            return @"
apply plugin: 'com.android.application'
apply from: 'setupSymbols.gradle'
apply from: '../shared/keepUnitySymbols.gradle'

dependencies {
    implementation project(':unityLibrary')
    }

android {
    namespace ""**NAMESPACE**""
    ndkPath ""**NDKPATH**""
    ndkVersion ""**NDKVERSION**""

    compileSdk **APIVERSION**
    buildToolsVersion = ""**BUILDTOOLS**""

    compileOptions {
        sourceCompatibility JavaVersion.VERSION_17
        targetCompatibility JavaVersion.VERSION_17
    }

    defaultConfig {
        minSdk **MINSDK**
        targetSdk **TARGETSDK**
        applicationId '**APPLICATIONID**'
        ndk {
            abiFilters **ABIFILTERS**
            debugSymbolLevel **DEBUGSYMBOLLEVEL**
        }
        versionCode **VERSIONCODE**
        versionName '**VERSIONNAME**'
    }

    androidResources {
        noCompress = **BUILTIN_NOCOMPRESS** + unityStreamingAssets.tokenize(', ')
        ignoreAssetsPattern = ""!.svn:!.git:!.ds_store:!*.scc:!CVS:!thumbs.db:!picasa.ini:!*~""
    }**SIGN**

    lint {
        abortOnError false
    }

    buildTypes {
        debug {
            minifyEnabled **MINIFY_DEBUG**
            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**
            jniDebuggable true
        }
        release {
            minifyEnabled **MINIFY_RELEASE**
            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**
        }
    }**PACKAGING****PLAY_ASSET_PACKS****SPLITS**
**BUILT_APK_LOCATION**
    bundle {
        language {
            enableSplit = false
        }
        density {
            enableSplit = false
        }
        abi {
            enableSplit = true
        }
        texture {
            enableSplit = true
        }
    }

    **GOOGLE_PLAY_DEPENDENCIES**
}**SPLITS_VERSION_CODE****LAUNCHER_SOURCE_BUILD_SETUP**
";
#else
            // Unity 2019 ~ 2022 (22.x 포함)
            return @"
apply plugin: 'com.android.application'

dependencies {
    implementation project(':unityLibrary')
    }

android {
    namespace ""**NAMESPACE**""
    ndkPath ""**NDKPATH**""

    compileSdkVersion **APIVERSION**
    buildToolsVersion '**BUILDTOOLS**'

    compileOptions {
        sourceCompatibility JavaVersion.VERSION_11
        targetCompatibility JavaVersion.VERSION_11
    }

    defaultConfig {
        minSdkVersion **MINSDKVERSION**
        targetSdkVersion **TARGETSDKVERSION**
        applicationId '**APPLICATIONID**'
        ndk {
            abiFilters **ABIFILTERS**
        }
        versionCode **VERSIONCODE**
        versionName '**VERSIONNAME**'
    }

    aaptOptions {
        noCompress = **BUILTIN_NOCOMPRESS** + unityStreamingAssets.tokenize(', ')
        ignoreAssetsPattern = ""!.svn:!.git:!.ds_store:!*.scc:!CVS:!thumbs.db:!picasa.ini:!*~""
    }**SIGN**

    lintOptions {
        abortOnError false
    }

    buildTypes {
        debug {
            minifyEnabled **MINIFY_DEBUG**
            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**
            jniDebuggable true
        }
        release {
            minifyEnabled **MINIFY_RELEASE**
            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**
        }
    }**PACKAGING_OPTIONS****PLAY_ASSET_PACKS****SPLITS**
**BUILT_APK_LOCATION**
    bundle {
        language {
            enableSplit = false
        }
        density {
            enableSplit = false
        }
        abi {
            enableSplit = true
        }
    }
}**SPLITS_VERSION_CODE****LAUNCHER_SOURCE_BUILD_SETUP**
";
#endif
        }
    }
}
