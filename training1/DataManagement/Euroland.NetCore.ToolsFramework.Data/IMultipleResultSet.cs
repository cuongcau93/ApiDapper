using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Data
{
    public interface IMultipleResultSet : IDisposable
    {
        /// <summary>
        /// Get an object from multiple set
        /// </summary>
        /// <typeparam name="TResult">Type of object to map returned result to</typeparam>
        /// <returns>Returns an object from multiple set with properties matching columns. Return <c>Null</c> if no matching</returns>
        TResult GetSingle<TResult>() where TResult : class;

        /// <summary>
        /// Gets the set of record from multiple record-set
        /// </summary>
        /// <typeparam name="TResult">Type of object to map returned result to</typeparam>
        /// <returns>The set of record</returns>
        IEnumerable<TResult> Get<TResult>() where TResult : class;
    }
}
