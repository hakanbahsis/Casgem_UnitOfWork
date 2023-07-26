using Business.Abstract;
using DataAccess.Concrete.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerProcessService _customerProcessService;
        private readonly Context _context;

        public DefaultController(ICustomerService customerService, Context context, ICustomerProcessService customerProcessService)
        {
            _customerService = customerService;
            _context = context;
            _customerProcessService = customerProcessService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<SelectListItem> sender = (from x in _context.Customers.ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.CustomerName,
                                             Value = x.CustomerId.ToString()
                                         }).ToList();
            ViewBag.Sender = sender;

            List<SelectListItem> receiver = (from x in _context.Customers.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.CustomerName,
                                               Value = x.CustomerId.ToString()
                                           }).ToList();
            ViewBag.Receiver = receiver;
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomerProcess customerProcess)
        {
            var valueSender = _customerService.TGetByID(customerProcess.SenderId);
            var valueReceiver = _customerService.TGetByID(customerProcess.ReveiverId);

            valueReceiver.CustomerBalance += customerProcess.Amount;
            valueSender.CustomerBalance -= customerProcess.Amount;

            List<Customer> modifiedCustomer= new List<Customer>()
            {
                valueSender, valueReceiver
            };

            _customerService.TMultiUpdate(modifiedCustomer);
            _customerProcessService.TInsert(customerProcess);

            return RedirectToAction("CustomerProcessList");
        }
    }
}
