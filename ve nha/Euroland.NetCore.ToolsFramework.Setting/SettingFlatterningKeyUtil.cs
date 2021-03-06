﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Setting
{
    public class SettingFlatterningKeyUtil
    {
        public static readonly string NameDelimiter = ":";

        /// <summary>
        /// Gets setting name from the given path
        /// </summary>
        /// <param name="path">The path to the this setting</param>
        /// <returns>Name of the setting</returns>
        public static string GetSettingName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var lastDelimiterIndex = path.LastIndexOf(NameDelimiter);

            return lastDelimiterIndex == -1 ? path : path.Substring(lastDelimiterIndex + 1);
        }

        /// <summary>
        /// Gets setting name nearest the path delimiter from specified path
        /// </summary>
        /// <param name="path">The setting path</param>
        /// <param name="parentPath">The prefix parent to look up setting name</param>
        /// <returns></returns>
        public static string GetClosestSettingName(string path, string parentPath)
        {
            if (!string.IsNullOrEmpty(parentPath))
            {
                parentPath = parentPath + NameDelimiter;
            }
            else
                parentPath = string.Empty;

            var parentPathLength = parentPath.Length;
            var delimiterIndexFromParent = path.IndexOf(NameDelimiter, parentPathLength);

            return delimiterIndexFromParent == -1 
                ? path.Substring(parentPathLength) 
                : path.Substring(parentPathLength, delimiterIndexFromParent - parentPathLength);
        }

        /// <summary>
        /// Gets parent name from the given path
        /// </summary>
        /// <param name="path">The path to the this setting</param>
        /// <returns>The path to the parent</returns>
        public static string GetSettingParentName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            var lastDelimiterIndex = path.LastIndexOf(NameDelimiter);
            return lastDelimiterIndex == -1 ? path : path.Substring(0, lastDelimiterIndex);
        }

        /// <summary>
        /// Combines the path segments into single path
        /// </summary>
        /// <param name="name">The name of current setting</param>
        /// <param name="path">The parent's path of current setting</param>
        public static string Join(string path, string name)
        {
            return string.IsNullOrEmpty(path) 
                ? name
                : string.Join(NameDelimiter, path, name);
        }
    }
}
