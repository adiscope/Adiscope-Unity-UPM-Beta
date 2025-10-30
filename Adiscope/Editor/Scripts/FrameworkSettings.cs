using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adiscope
{
    public class FrameworkSettings : ScriptableObject
    {
        [SerializeField]
        string _mediaID_aos;

        [SerializeField]
        string _mediaSecret_aos;

        [SerializeField]
        string _mediaID_ios;

        [SerializeField]
        string _mediaSecret_ios;

        [SerializeField]
        string _subDomain;

        [SerializeField]
        string _trackingDesc;

        [SerializeField]
        int _adeventAdapter;

        [SerializeField]
        int _admobAdapter;

        [SerializeField]
        string _admobAppKey_aos;

        [SerializeField]
        string _admobAppKey_ios;

        [SerializeField]
        int _maxAdapter;

        [SerializeField]
        int _admanagerAdapter;

        [SerializeField]
        int _chartboostAdapter;

        [SerializeField]
        int _pangleAdapter;

        [SerializeField]
        int _vungleAdapter;

        public static string MediaID_AOS { get { 
            var serialized = new SerializedObject(FrameworkSettingsRegister.Load());
            return serialized.FindProperty("_mediaID_aos").stringValue; 
        } }

        public static string MediaID_iOS { get { 
            var serialized = new SerializedObject(FrameworkSettingsRegister.Load());
            return serialized.FindProperty("_mediaID_ios").stringValue; 
        } }
        public static string SubDomain { get { 
            var serialized = new SerializedObject(FrameworkSettingsRegister.Load());
            return serialized.FindProperty("_subDomain").stringValue; 
        } }
    }

    public class ParsingPackageJson : MonoBehaviour
    {
        [Serializable]
        public class PackageJson
        {
            public string name;
            public string displayName;
            public string version;
            public string description;
            public string unity;
            public string[] keywords;
            public Dictionary<string, object> author;
            public Dictionary<string, object> dependencies;
        }
    }
}