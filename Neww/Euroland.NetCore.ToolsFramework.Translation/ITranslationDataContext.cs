using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public interface ITranslationDataContext
    {
        IEnumerable<Translation> LoadTranslations(string storedProcedure, object parameters);

        T ExecSingle<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        T ExecSingle<T>(string storedProcedure, dynamic parameterObject = null);
    }
}
