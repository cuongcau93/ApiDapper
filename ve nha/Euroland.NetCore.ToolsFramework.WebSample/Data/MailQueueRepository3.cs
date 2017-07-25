using Euroland.NetCore.ToolsFramework.Data;
using Euroland.NetCore.ToolsFramework.WebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.WebSample.Data
{
    public interface IMailQueueRepository3
    {
        List<MailQueue> GetAllMail();
    }

    public class MailQueueRepository3 : RepositoryBase, IMailQueueRepository3
    {
        public MailQueueRepository3(string connectionString) : base(connectionString)
        {

        }

        public List<MailQueue> GetAllMail()
        {
            return this.DbContext.Exec<MailQueue>("spMailQueueSelectAll").ToList();
        }
    }
}
