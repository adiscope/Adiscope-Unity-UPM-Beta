/*
 * Created by Sooyeon Jo (sooyeon@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS
{
	/// <summary>
	/// iOS client for ad event
	/// this class will call iOS native plugin's method
	/// </summary>		
	public class AdEventClient : IAdEventClient
	{
		public event EventHandler<ShowResult> OnOpened;
		public event EventHandler<ShowResult> OnClosed;
		public event EventHandler<ShowFailure> OnFailedToShow;

		public event EventHandler<ShowResult> OnOpenedBackground;
		public event EventHandler<ShowResult> OnClosedBackground;
		public event EventHandler<ShowFailure> OnFailedToShowBackground;

		public static AdEventClient Instance;

		public AdEventClient ()
		{
			Instance = this;
		}

		#region AD APIs 

		public bool Show(string unitId)
		{
			return false;
		}

		#endregion

		#region Callbacks

		private delegate void onAdEventOpenedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onAdEventOpenedCallback))] 
		public static void onAdEventOpened(string unitId)
		{
			Debug.Log("onAdEventOpened() unitId = " + unitId);
			Instance.AdEventOpenedProc (unitId);
		}

		private void AdEventOpenedProc(string unitId)
		{
			if (Instance.OnOpened != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnOpened(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnOpenedBackground != null)
			{
				Instance.OnOpenedBackground(Instance, new ShowResult(unitId));
			}
		}

		private delegate void onAdEventClosedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onAdEventClosedCallback))] 
		public static void onAdEventClosed(string unitId)
		{
			Debug.Log("onAdEventClosed() unitId = " + unitId);
			Instance.AdEventClosedProc (unitId);
		}

		private void AdEventClosedProc(string unitId)
		{
			if (Instance.OnClosed != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnClosed(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnClosedBackground != null)
			{
				Instance.OnClosedBackground(Instance, new ShowResult(unitId));
			}
		}
			
		private delegate void onAdEventFailedToShowCallback(string unitId, int code, string description);

		[MonoPInvokeCallback(typeof(onAdEventFailedToShowCallback))]
		public static void onAdEventFailedToShow(string unitId, int code, string description)
		{
			Debug.Log("onAdEventFailedToShow() unitId = " + unitId + " code = " + code + " description = " + description);
			Instance.AdEventFailedToShowProc (unitId, code, description);
		}

		private void AdEventFailedToShowProc(string unitId, int code, string description)
		{
			if (Instance.OnFailedToShow != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnFailedToShow(
						Instance, new ShowFailure(unitId, new AdiscopeError(code, description)));
				});
			}

			if (Instance.OnFailedToShowBackground != null)
			{
				Instance.OnFailedToShowBackground (
					Instance, new ShowFailure(unitId, new AdiscopeError (code, description)));
			}
		}
        #endregion
    }
}

#endif