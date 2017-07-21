using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using System.IO;

namespace Euroland.NetCore.ToolsFramework.Localization
{
    /// <summary>
    /// An implementation of <see cref="IFileInfo"/> to provide reading resources embeded inside assembly
    /// </summary>
    internal sealed class AssemblyStreamFileInfo : IFileInfo
    {
        private readonly System.Reflection.Assembly _assembly;
        private readonly string _resourceName;

        /// <summary>
        /// Create a <see cref="AssemblyStreamFileInfo"/>
        /// </summary>
        /// <param name="assembly">Assembly where resource is embeded</param>
        /// <param name="resourceName">The fully qualified name of resource, including namespace</param>
        public AssemblyStreamFileInfo(System.Reflection.Assembly assembly, string resourceName)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            if(string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            _assembly = assembly;
            _resourceName = resourceName;
        }
        public bool Exists => _assembly.GetManifestResourceNames().Any(name => string.Equals(name, _resourceName, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Always returns 0
        /// </summary>
        public long Length => 0;

        public string PhysicalPath => _assembly.Location;

        public string Name => System.IO.Path.GetFileName(_resourceName);

        public DateTimeOffset LastModified => DateTime.MinValue;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            return _assembly.GetManifestResourceStream(_resourceName);
        }
    }
}
