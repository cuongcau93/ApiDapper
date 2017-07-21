using Euroland.NetCore.ToolsFramework.Setting.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    /// <summary>
    /// An implementation of <see cref="ISettingItem"/>
    /// </summary>
    public class SettingItem : ISettingItem
    {
        private readonly string _path;
        private readonly SettingItemRoot _root;

        /// <summary>
        /// Create a new <see cref="SettingItem"/>
        /// </summary>
        /// <param name="root">The root setting item</param>
        /// <param name="path">The path to this setting item</param>
        public SettingItem(SettingItemRoot root, string path)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            _root = root;
            _path = path;
        }

        public string this[string name]
        {
            get
            {
                return _root[SettingFlatterningKeyUtil.Combine(_path, name)];
            }
            set
            {
                _root[SettingFlatterningKeyUtil.Combine(_path, name)] = value;
            }
        }

        public string Name => SettingFlatterningKeyUtil.GetSettingName(_path);

        public string Path => _path;

        public string Value
        {
            get
            {
                return _root[_path];
            }
            set
            {
                _root[_path] = value;
            }

        }

        public ISettingItem GetChild(string name)
        {
            return _root.GetChild(SettingFlatterningKeyUtil.Combine(_path, name));
        }

        public IEnumerable<ISettingItem> GetChildren()
        {
            return _root.GetImmediateChildren(_path);
        }
    }
}
