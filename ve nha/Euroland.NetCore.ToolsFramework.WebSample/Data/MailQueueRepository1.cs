using Euroland.NetCore.ToolsFramework.Data;
using Euroland.NetCore.ToolsFramework.WebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.WebSample.Data
{
    public interface IMailQueueRepository1
    {
        List<MailQueue> GetAllMail();
    }
    public class MailQueueRepository1 : RepositoryBase, IMailQueueRepository1
    {
        public MailQueueRepository1(IDatabaseContext dbContext) : base(dbContext)
        {

        }

        public List<MailQueue> GetAllMail()
        {
            return this.DbContext.Exec<MailQueue>("spMailQueueSelectAll").ToList();
        }
    }
}
