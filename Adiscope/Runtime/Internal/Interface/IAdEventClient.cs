/*
 * Created by Sooyeon Jo (sooyeon@neowiz.com)
 */
using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for AdEvent client
    /// </summary>
    internal interface IAdEventClient
    {
        event EventHandler<ShowResult> OnOpened;
        event EventHandler<ShowResult> OnClosed;
        event EventHandler<ShowFailure> OnFailedToShow;

        event EventHandler<ShowResult> OnOpenedBackground;
        event EventHandler<ShowResult> OnClosedBackground;
        event EventHandler<ShowFailure> OnFailedToShowBackground;

        bool Show(string unitId);
        
    }
}
