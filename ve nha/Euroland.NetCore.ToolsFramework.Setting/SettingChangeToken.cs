using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// An implementaion of <see cref="IChangeToken"/> to propagate 
    /// notifications that a setting change has occurred
    /// </summary>
    public class SettingChangeToken : IChangeToken
    {
        private CancellationTokenSource _calcelTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets a value tha indicates if a change has occurred
        /// </summary>
        public bool HasChanged => _calcelTokenSource.IsCancellationRequested;

        public bool ActiveChangeCallbacks => true;

        /// <summary>
        ///  Registers for a callback that will be invoked when the entry has changed. 
        /// </summary>
        /// <param name="callback">The <see cref="Action{object}"/> to invoke</param>
        /// <param name="state">The state to be passed into the callback</param>
        /// <returns>An <see cref="IDisposable"/> that is used to unregister the callback</returns>
        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return _calcelTokenSource.Token.Register(callback, state);
        }

        /// <summary>
        /// Trigger the change token when a reload occurs
        /// </summary>
        public void OnChange()
        {
            _calcelTokenSource.Cancel();
        }
    }
}
