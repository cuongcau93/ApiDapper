using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting.Abstractions
{
    /// <summary>
    /// Represents setting key/values for an application
    /// </summary>
    public interface ISettingProvider
    {
        /// <summary>
        /// Tries to get setting value for the specified name
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="value">The setting value</param>
        /// <returns><c>True</c> if a value for the specified name was found, otherwise <c>False</c></returns>
        bool TryGet(string name, out string value);

        /// <summary>
        /// Sets a setting value for the specified name
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="value">The setting value</param>
        void Set(string name, string value);

        /// <summary>
        /// Loads setting values from the <see cref="ISettingProviderFactory"/>s represented by this <see cref="ISettingProvider"/>
        /// </summary>
        void Load();

        /// <summary>
        /// Gets value indicating that provider has already loaded
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Returns the list of names without path that this provider contains
        /// </summary>
        /// <param name="otherChildNames">The child names that other providers contain</param>
        /// <param name="parentPath">The path for the parent <see cref="ISetting"/></param>
        /// <returns>The list of keys for this provider.</returns>
        IEnumerable<string> GetChildKeys(IEnumerable<string> otherChildNames, string parentPath);

        /// <summary>
        /// Gets serializing setting data to string
        /// </summary>
        /// <returns>The serialization string</returns>
        string WriteAsString();

        /// <summary>
        /// Gets async serializing setting data to string
        /// </summary>
        /// <returns>The serialization string</returns>
        System.Threading.Tasks.Task<string> WriteAsStringAsync();

        /// <summary>
        /// Writes serialized setting data string into a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to write</param>
        void WriteAsString(System.IO.Stream stream);

        /// <summary>
        /// Writes serialized setting data string into a <see cref="System.IO.Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to write</param>
        /// <returns></returns>
        System.Threading.Tasks.Task WriteAsStringAsync(System.IO.Stream stream);
    }
}
