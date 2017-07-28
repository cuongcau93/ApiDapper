/*
 * Sample to new instance DbContext with connection string 
 * This is the second way
 * Injection Configuration to get connection string from outside
 */
using Euroland.NetCore.ToolsFramework.Data;
using Euroland.NetCore.ToolsFramework.WebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euroland.NetCore.ToolsFramework.WebSample.Data
{
    public interface IMailQueueRepository2
    {
        List<MailQueue> GetAllMail();
    }

    public class MailQueueRepository2 : IMailQueueRepository2
    {
        private readonly Microsoft.Extensions.Configuration.IConfigurationRoot _configuration;
        private readonly IDatabaseContext DbContext;
        public MailQueueRepository2(Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }


        public MailQueueRepository2()
        {
            var sharkconnectionString = _configuration["ConnectionStrings:DefaultConnection"];
            DbContext = new DapperDatabaseContext(sharkconnectionString);
        }

        public List<MailQueue> GetAllMail()
        {
            return this.DbContext.Exec<MailQueue>("spMailQueueSelectAll").ToList();
        }
    }
}
