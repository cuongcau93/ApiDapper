using Euroland.NetCore.ToolsFramework.Data;
using Euroland.NetCore.ToolsFramework.WebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.WebSample.Data
{
    public interface IMailQueueRepository4
    {
        List<MailQueue> GetAllMail();
    }

    public class MailQueueRepository4 : RepositoryBase, IMailQueueRepository4
    {
        public MailQueueRepository4(IDatabaseContext dbContext) : base(dbContext)
        {

        }

        public List<MailQueue> GetAllMail()
        {
            return this.DbContext.Exec<MailQueue>("spMailQueueSelectAll").ToList();
        }
    }
}
