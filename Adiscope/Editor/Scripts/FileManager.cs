/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Adiscope.Editor
{
    /// <summary>
    /// Manager class to handle file operation
    /// </summary>
    public class FileManager
    {
        public FileManager() { }

        /// <summary>
        /// read json file as dictionary
        /// </summary>
        /// <param name="filePath">json file path</param>
        /// <returns>json data map</returns>
        public Dictionary<string, object> ReadJsonFile(string filePath)
        {
            // check file path
            if (filePath == null || filePath.Length == 0)
            {
                Logger.e("invalid file path");
                return null;
            }

            // Read json string from jsonFilePath
            string jsonString = string.Empty;
            try
            {
                jsonString = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Logger.e("can't read file: {0}", filePath);
                Logger.e(e);
                return null;
            }

            if (jsonString == null || jsonString.Length == 0)
            {
                Logger.e("content not exists");
                return null;
            }

            // Parse the contents
            Dictionary<string, object> settings = null;
            try
            {
                settings = MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
            }
            catch (Exception e)
            {
                Logger.e("parsing json string error: {0}", jsonString);
                Logger.e(e);
                return null;
            }

            if (settings == null)
            {
                Logger.e("parsing failed, returned null");
                return null;
            }

            return settings;
        }
    }
}
