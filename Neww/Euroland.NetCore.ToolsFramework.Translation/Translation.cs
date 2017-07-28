using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// Represents a row of translation
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// Gets or sets the ID of translation
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}"/> 
        /// of Language and translation phrase
        /// </summary>
        public IDictionary<string, string> TranslationMap { get; set; }
    }
}
