using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Euroland.NetCore.ToolsFramework.WebSample.Data;
using Euroland.NetCore.ToolsFramework.WebSample.Models;

namespace Euroland.NetCore.ToolsFramework.WebSample.Controllers
{
    public class MailQueueController : Controller
    {
        private readonly IMailQueueRepository1 mailQueueRepository;
        public MailQueueController(IMailQueueRepository1 mailQueueRepository)
        {
            this.mailQueueRepository = mailQueueRepository;
        }

        public IActionResult Index()
        {
            List<MailQueue> mailQueues = mailQueueRepository.GetAllMail();
            //ViewBag.MailQueues = mailQueues;
            return View(mailQueues);
        }
    }
}