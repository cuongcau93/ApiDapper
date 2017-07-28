using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public class Phrases
    {
        public int id { get; set; }

        public string EN { get; set; }

        public string DE { get; set; }

        public string ES { get; set; }

        public string FR { get; set; }

        public string IT { get; set; }

        public string NL { get; set; }

        public string FI { get; set; }

        public string SE { get; set; }

        public string RU { get; set; }

        public string PL { get; set; }

        public string CN { get; set; }

        public string KR { get; set; }

        public string JP { get; set; }

        public string IE { get; set; }

        public string NO { get; set; }

        public string DK { get; set; }

        public string FO { get; set; }

        public System.Nullable<int> foId { get; set; }

        public string ET { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public System.Nullable<byte> Status { get; set; }

        public System.Nullable<int> TempID { get; set; }

        public string location { get; set; }

        public DateTime inserted { get; set; }

        public bool JPtranslated { get; set; }

        public System.Nullable<bool> ARtranslated { get; set; }

        public System.Nullable<bool> DKtranslated { get; set; }

        public string AR { get; set; }

        public string VI { get; set; }

        public System.Nullable<bool> VItranslated { get; set; }

        public System.Nullable<bool> ETtranslated { get; set; }

        public string PT { get; set; }

        public string TW { get; set; }

        public string HE { get; set; }
    }

    public class TranslationInfo
    {
        public string PropertyName { get; set; }
        public Phrases Phrases { get; set; }
    }
}
