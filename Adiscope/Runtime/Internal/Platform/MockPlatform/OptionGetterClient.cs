/*
 * Created by mjgu (mjgu@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)

using System;
using System.IO;
using UnityEngine;

using Adiscope.Internal.Interface;

namespace Adiscope.Internal.Platform.MockPlatform
{
	internal class OptionGetterClient : IOptionGetterClient
	{
		public OptionGetterClient ()
		{
		}

		#region APIs 


		public string GetSDKVersion() { return ""; }

		public string GetUnitySDKVersion() { 
            string filePath = "Packages/com.tnk.adiscope/package.json";
            string json = File.ReadAllText(filePath);
            ParsingPackageJson.PackageJson pj = JsonUtility.FromJson<ParsingPackageJson.PackageJson>(json);
			return pj.version; 
		}

		public string GetNetworkVersions() { return ""; }

		public string GetInitializeFailLog() { return ""; }

		#endregion
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
        }
    }
}

#endif