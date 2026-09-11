/*
 * Created by mjgu (mjgu@neowiz.com)
 */
using System;

namespace Adiscope.Model
{
    /// <summary>
    /// success information for showing ad
    /// </summary>
    public class InitResult : EventArgs
    {
        /// <summary>
        /// Result of Initialize Result
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// constructor for show success result
        /// </summary>
        /// <param name="isSuccess">result of initialize</param>
        public InitResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public override string ToString()
        {
            return
                "InitResult{" +
                "IsSuccess=\"" + this.IsSuccess + "\"" +
                "}";
        }
    }
}
