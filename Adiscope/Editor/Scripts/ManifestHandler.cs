/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Adiscope.Editor
{
    /// <summary>
    /// Generates AndroidManifest.xml file based on the Ad network setting
    /// used by Unity Editor Menu Extention for AdiscopeSdk
    /// Created by Hyunchang Kim (martin.kim@neowiz.com)
    /// </summary>
    public class ManifestHandler
    {
        private const string XML_VERSION = "1.0";
        private const string UTF8 = "utf-8";
        private const string N_SPACE = "http://schemas.android.com/apk/res/android";
        private const string N_SPACE_ALIAS = "android";

        private const string XML_TAG_MANIFEST = "manifest";
        private const string XML_TAG_USES_SDK = "uses-sdk";
        private const string XML_TAG_APPLICATION = "application";
        private const string XML_TAG_QUERIES = "queries";
        private const string XML_TAG_INTENT = "intent";
        private const string XML_TAG_ACTION = "action";
        private const string XML_TAG_USES_PERMISSION = "uses-permission";
        private const string XML_TAG_META_DATA = "meta-data";
        private const string XML_TAG_ACTIVITY = "activity";
        private const string XML_TAG_RECEIVER = "receiver";
        private const string XML_TAG_SERVICE = "service";
        private const string XML_TAG_PROVIDER = "provider";
        private const string XML_TAG_USES_LIBRARY = "uses-library";
        private const string XML_TAG_INTENT_FILTER = "intent-filter";
        private const string XML_TAG_DATA = "data";

        private const string XML_ATT_PACKAGE = "package";
        private const string XML_ATT_MIN_SDK_VERSION = "minSdkVersion";
        private const string XML_ATT_ANDROID_NAME = "name";
        private const string XML_ATT_ANDROID_VALUE = "value";
        private const string XML_ATT_PATH_PREFIX = "pathPrefix";
        private const string XML_ATT_SCHEME = "scheme";
        private const string XML_ATT_HOST = "host";

        private const string XML_INDENT_STR = "    ";

        private const int androidMinSdkVersion = 15;

        private XDocument docs;
        private XNamespace ns;
        private XElement rootManifest;
        private XElement usesSdk;
        private XElement application;
        private XElement queries;
        private XElement intents;

        private Dictionary<string, XElement> metaDatas;
        private Dictionary<string, XElement> actions;
        private Dictionary<string, XElement> usesPermissions;
        private Dictionary<string, XElement> activities;
        private Dictionary<string, XElement> receivers;
        private Dictionary<string, XElement> services;
        private Dictionary<string, XElement> providers;
        private Dictionary<string, XElement> usesLibrarys;

        public ManifestHandler(string packageName)
        {
            this.docs = new XDocument(new XDeclaration(XML_VERSION, UTF8, null));

            this.ns = N_SPACE;

            this.rootManifest = new XElement(
                XML_TAG_MANIFEST,
                new XAttribute(XNamespace.Xmlns + N_SPACE_ALIAS, ns),
                new XAttribute(XML_ATT_PACKAGE, packageName));

            this.usesSdk = new XElement(
                XML_TAG_USES_SDK,
                new XAttribute(ns + XML_ATT_MIN_SDK_VERSION, androidMinSdkVersion));

            this.application = new XElement(XML_TAG_APPLICATION);
            this.queries = new XElement(XML_TAG_QUERIES);
            this.intents = new XElement(XML_TAG_INTENT);

            this.actions = new Dictionary<string, XElement>();

            this.metaDatas = new Dictionary<string, XElement>();

            this.usesPermissions = new Dictionary<string, XElement>();

            this.activities = new Dictionary<string, XElement>();

            this.receivers = new Dictionary<string, XElement>();
            this.services = new Dictionary<string, XElement>();
            this.providers = new Dictionary<string, XElement>();
            this.usesLibrarys = new Dictionary<string, XElement>();
        }

        public bool AddAction(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.actions, XML_TAG_ACTION);
        }

        public bool AddPermission(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.usesPermissions, XML_TAG_USES_PERMISSION);
        }

        public bool AddMetaData(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.metaDatas, XML_TAG_META_DATA);
        }

        public bool AddActivity(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.activities, XML_TAG_ACTIVITY);
        }

        public bool AddReceiver(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.receivers, XML_TAG_RECEIVER);
        }
        public bool AddService(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.services, XML_TAG_SERVICE);
        }
        public bool AddProvider(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.providers, XML_TAG_PROVIDER);
        }
        public bool AddUsesLibrary(string xmlStr)
        {
            return AddXmlElement(xmlStr, this.usesLibrarys, XML_TAG_USES_LIBRARY);
        }

        private bool AddXmlElement(string xmlStr, Dictionary<string, XElement> storage,
            string tagName)
        {
            string attributeName;
            XElement elementToAdd;

            if (!ParseXmlElement(xmlStr, tagName, out attributeName, out elementToAdd))
            {
                Logger.e("adding xml element failed - parse error");
                return false;
            }

            try
            {
                if (!storage.ContainsKey(attributeName))
                {
                    storage.Add(attributeName, elementToAdd);
                }
                else
                {
                    // admob 도 있고 admanager 도 있는경우 먼저 어떤 값이 있을수 가 있다 이케이스에 먼저 세팅 된 값을 제거 해준뒤 추가 해주어야 한다.
                    if (tagName == "meta-data" && attributeName == "com.google.android.gms.ads.APPLICATION_ID")
                    {
                        storage.Remove(attributeName);
                        storage.Add(attributeName, elementToAdd);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.e("parsing {0} failed: {1}", tagName, xmlStr);
                Logger.e(e);

                return false;
            }

            return true;
        }

        private bool ParseXmlElement(string xmlStr, string xmlTagName,
            out string attrName, out XElement element)
        {
            KeyValuePair<string, string> attribute;

            bool result = ParseXmlElement(xmlStr, xmlTagName, false, out attribute, out element);

            attrName = attribute.Key;

            return result;
        }

        private bool ParseXmlElement(string xmlStr, string xmlTagName,
            out string attrName, out string attrValue, out XElement element)
        {
            KeyValuePair<string, string> attribute;

            bool result = ParseXmlElement(xmlStr, xmlTagName, true, out attribute, out element);

            attrName = attribute.Key;
            attrValue = attribute.Value;

            return result;
        }

        private bool ParseXmlElement(string xmlStr, string xmlTagName, bool needValue,
            out KeyValuePair<string, string> attribute, out XElement element)
        {
            try
            {
                XElement parsedElement = XElement.Parse(
                    "<" + XML_TAG_MANIFEST + " xmlns:" + N_SPACE_ALIAS + "=\"" + N_SPACE + "\">" +
                    xmlStr +
                    "</" + XML_TAG_MANIFEST + ">");
                
                XElement tagElement = parsedElement.Element(xmlTagName);

                string parsedName = tagElement.Attribute(this.ns + XML_ATT_ANDROID_NAME).Value;
                
                string parsedValue = null;
                if (needValue)
                {
                    parsedValue = tagElement.Attribute(this.ns + XML_ATT_ANDROID_VALUE).Value;
                }
                
                attribute = new KeyValuePair<string, string>(parsedName, parsedValue);
                element = tagElement;
                
                return true;
            }
            catch (Exception e)
            {
                Logger.e(e);

                attribute = new KeyValuePair<string, string>();
                element = null;

                return false;
            }
        }

        public bool WriteXmlFile(string targetFileName)
        {
            if (targetFileName == null || targetFileName.Length == 0)
            {
                Logger.e("invalide target file path");
                return false;
            }

            MakeXmlDocument();

            try
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = false;
                xws.IndentChars = XML_INDENT_STR;

                string parentDirectory = Path.GetDirectoryName(targetFileName);
                Directory.CreateDirectory(parentDirectory);

                XmlWriter xw = XmlWriter.Create(targetFileName, xws);

                this.docs.WriteTo(xw);
                xw.Flush();
                xw.Close();
            }
            catch (Exception e)
            {
                Logger.e("failed to write XML file: {0}", targetFileName);
                Logger.e(e);
                return false;
            }

            return true;
        }

        private void MakeXmlDocument()
        {
            this.docs.Add(this.rootManifest);

            foreach (KeyValuePair<string, XElement> kv in this.actions)
            {
                this.intents.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.usesPermissions)
            {
                this.rootManifest.Add(kv.Value);
            }

            // intent에 필수 속성인 action이나 data가 포함되어 있지 않을 시 intent 속성을 매니페스트에 작성하지 않음
            if((intents.ToString()).Contains(XML_TAG_ACTION) || (intents.ToString()).Contains(XML_TAG_DATA)) {
                this.queries.Add(this.intents);
                this.rootManifest.Add(this.queries);
            }

            this.rootManifest.Add(this.application);

            foreach (KeyValuePair<string, XElement> kv in this.metaDatas)
            {
                this.application.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.activities)
            {
                this.application.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.receivers)
            {
                this.application.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.services)
            {
                this.application.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.providers)
            {
                this.application.Add(kv.Value);
            }

            foreach (KeyValuePair<string, XElement> kv in this.usesLibrarys)
            {
                this.application.Add(kv.Value);
            }
        }
    }
}
