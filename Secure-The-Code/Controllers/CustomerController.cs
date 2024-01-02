using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Secure_The_Code.Data;
using Secure_The_Code.Models;
using System.Data;

namespace Secure_The_Code.Controllers
{
    public class CustomerController : Controller
    {

        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        private readonly IDataProtector _dataProtector;
		public CustomerController(ApplicationDbContext contex, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
		{
			_context = contex;
			_dataProtector = dataProtectionProvider.CreateProtector("SecureMyCode");
			_configuration = configuration;
		}



		#region Using Ado
		public IActionResult Index(string searchValue)
		{
			List<CustomerViewModel> viewModel = [];

            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand command;
            if (!string.IsNullOrEmpty(searchValue))
            {
                //This can be exposed to Sql Inject
                //command = new($"SELECT * FROM Customers WHERE LastName LIKE {"%" + searchValue + "%"}",connection);


                command = new("SearchCustomers", connection);
                command.Parameters.AddWithValue("@SearchValue", searchValue);
                command.CommandType = CommandType.StoredProcedure;
            } 
            else
            {
                command = new SqlCommand("SELECT * FROM Customers", connection);
				command.CommandType = CommandType.Text;
			}

         

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                var customer = new CustomerViewModel
                {
                    Key = _dataProtector.Protect(reader.GetValue(nameof(Customer.Id)).ToString()!),
                    FirstName = reader.GetValue(nameof(Customer.FirstName)).ToString()!,
                    LastName = reader.GetValue(nameof(Customer.LastName)).ToString()!,
                    Gender = reader.GetValue(nameof(Customer.Gender)).ToString()!,
                    Email = reader.GetValue(nameof(Customer.Email)).ToString()!,
                    Address = reader.GetValue(nameof(Customer.Address)).ToString()!,
                };

                viewModel.Add(customer);
			}

            return View(viewModel);
			
		}
		#endregion
		//public IActionResult Index(string searchValue)
  //      {
  //          List<Customer> customers = [];

  //          if (!string.IsNullOrEmpty(searchValue))
  //          {
  //                  //customers = _context.Customers
		//			 //.FromSqlRaw($"SELECT * FROM Customers WHERE LastName LIKE {"%" + searchValue + "%"}")
		//			 //.ToList();

		//		//customers = _context.Customers
		//		// .FromSqlInterpolated($"SELECT * FROM Customers WHERE LastName LIKE {"%" + searchValue + "%"}")
		//		// .ToList();

		//		customers = _context.Customers.Where(c => c.LastName.Contains(searchValue)).ToList();
  //          }

  //          else
  //          {
  //              customers = _context.Customers.FromSqlRaw("SELECT * FROM Customers").ToList();
  //          }
  //          var viewModel = customers.Select(c => new CustomerViewModel
  //          {
  //              Key = _dataProtector.Protect(c.Id.ToString()),
  //              FirstName = c.FirstName,
  //              LastName = c.LastName,
  //              Gender = c.Gender,
  //              Email = c.Email,
  //              Address = c.Address
  //          });
  //          return View(viewModel);
  //      }


        public IActionResult Details(string id)
        {
            var customerId = int.Parse(_dataProtector.Unprotect(id));

            var customer = _context.Customers.Find(customerId);


            if (customer is null)
                return NotFound();

            var viewModel = new CustomerViewModel
            {
                Key = customerId.ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Gender = customer.Gender,
                Email = customer.Email,
                Address = customer.Address
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CustomerViewModel model)
        {

            //Validate on ServerSide
            if (!ModelState.IsValid)
                return View(model);

            var customer = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Email = model.Email,
                Address = model.Address
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
