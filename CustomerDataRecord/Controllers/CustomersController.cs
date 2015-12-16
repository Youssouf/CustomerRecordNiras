using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CustomerDataRecord.Models;
using System.Xml;
using System.Web.UI.WebControls;
using System.Data.Entity.Core;

namespace CustomerDataRecord.Controllers
{   
    /// <summary>
    ///  The customer controller  inherit fronm base class Controller  whcih encapsulate the different user anction
    /// </summary>
    public class CustomersController : Controller
    {
        private CustomerDataEntities db = new CustomerDataEntities();
        
        /// <summary>
        ///   This method returns a list of customers     
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var cust = (from c in db.Customer
                       orderby c.FirstName, c.LastName ascending
                       select c).ToList();
            return View(cust);
           
        }        

        /// <summary>
        /// This Method provides the details of a specific Customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
           
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        /// <summary>
        /// Get function to create  a new customer 
        /// </summary>
        /// <returns></returns>
     
       [HttpGet]
        public ActionResult Create()
        {

            return View();// PartialView("_CreateCustomerPartial");
        }
      
        /// <summary>
        /// Post Method to create a customer
        /// </summary>
        /// <param name="customer">Customer object </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customer.Add(customer);
                db.SaveChanges();             
               return  RedirectToAction("Index", "Customers"); 
            }

            return View(customer);          
        }

       /// <summary>
       ///  Update the information of a customer based on the id
       /// </summary>
       /// <param name="id">  an integer id  for the specific customer.</param>
       /// <returns> Returns a customer to view</returns>
       /// 
        [HttpGet]
  
        public ActionResult Edit(int id)
        {
            Session["CustomerID"] = id;  // Important for handling concurrencyException

            Customer customer = GetCustomerById(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
           
            return View(customer); 
        }
        /// <summary>
        /// Post method for creating  a new customer object
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            customer.CustomerID = (int)Session["CustomerID"]; // To handle concurrencyException
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();  
                
               
                return RedirectToAction("Index");
            }
           // return PartialView("_EditCustomerPartial", customer);
            return PartialView(customer);
        }

        /// <summary>
        /// Deleting a customer based on the specific id 
        /// </summary>
        /// <param name="id"> an integer customer id</param>
        /// <returns>Partial view based on customer model.</returns>
        

        public ActionResult Delete(int? id )
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteCustomerPartial",customer);
        }

        /// <summary>
        /// Post method deleting  a specific customer based on an integer customer id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Delete  a specific customer and redirected to the customer list.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customer.Find(id);
                db.Customer.Remove(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            
            }
        /// <summary>
        /// Find a specific customer based on the an integer id.
        /// </summary>
        /// <param name="id"> An integer customer id</param>
        /// <returns>A partial view based on the selected customed  is returned</returns>

       public PartialViewResult CustomerDetails(int id)
        {
       //   var custDatails = db.Customer.Where(x => x.CustomerID == id).ToList();
           var custDetail = from cust in db.Customer
                            where cust.CustomerID == id
                            select cust;
           return PartialView("CustomerDetails", custDetail);          
        }
        /// <summary>
        /// Based on  an integer customer id, a detailed inforamtion  of each  order of a specific customer is returned.
        /// </summary>
        /// <param name="id">An integer customer id</param>
        /// <returns>A partial view is returned</returns>
        
       public PartialViewResult GetCustomerOrderList(int id)
        {
           
            var queryOrder = (from ord in db.Customer
                              join ord1 in db.Orders on ord.CustomerID equals ord1.CustomerID
                              where ord.CustomerID == id
                              select ord1).ToList();

           if (queryOrder.Count != 0)
           {
               return PartialView("_CustomerOrderList", queryOrder);
           }
           else
           {
               return PartialView("_NoOrderFound");
           }

        }  
        /// <summary>
        /// Search  a customer based on country or a specific string
        /// </summary>
        /// <param name="stringKey">A string for the name of a customer</param>
        /// <param name="countryKey">string for the country</param>
        /// <returns> return a list of cusomer to the view.</returns>   

        public ActionResult FindCustomerByNameOrByCountry(string stringKey, string countryKey)
        {
            var listOfCountry = new List<string>();
           
            var searchCtry = from country in db.Customer
                             orderby country.Countrry
                             select country.Countrry;
            listOfCountry.AddRange(searchCtry.Distinct());

            ViewBag.countryKey = new SelectList(listOfCountry);

            var customSet = from cut in db.Customer
                            select cut;

            if (!string.IsNullOrEmpty(stringKey))
            {
                customSet = customSet.Where(x =>x.FirstName.Contains(stringKey));
                
                
            }

            if (!string.IsNullOrEmpty(countryKey))
            {
                customSet = customSet.Where(p => p.Countrry == countryKey);
            }
            return View(customSet);
                    
        }
       /// <summary>
       ///  Find all Companies for a given customer id 
       /// </summary>
       /// <param name="id"> An integer for customer id </param>
       /// <returns> Partial view of company list</returns>       

        public PartialViewResult FindCompanyForCustomerPartial(int? id)
        {
            var companyList = (from cp in db.Company
                               join custComp in db.Customer_Company on cp.CompanyID equals custComp.CompanyID
                               join cut in db.Customer on custComp.CustomerID equals cut.CustomerID
                               where cut.CustomerID == id
                               select cp).ToList();

            if (companyList.Count != 0)
            {
                return PartialView("FindCompanyForCustomer", companyList);

            }
            else
            {

                return PartialView("_NoCompanyFound"); 
                
            }

        }

        /// <summary>
        ///  Find  a customer by unsing its id.
        /// </summary>
        /// <param name="id">Based on a customer id a customer object is returned</param>
        /// <returns> customer object</returns>
        public  Customer GetCustomerById(int id)
        {
            var queryCust = from cust in db.Customer.Where(cust => cust.CustomerID == id)                    
                            select cust;
            Customer newCustomer = queryCust.FirstOrDefault();

            return newCustomer;
        }
        /// <summary>
        /// This method returns a list of customer
        /// </summary>
        /// <returns></returns>
        public List<Customer> GetCustomerById()
        {
           var queryCust = from cust in db.Customer
                            select cust;

            var result = queryCust.GroupBy(test => test.CustomerID)
                   .Select(grp => grp.FirstOrDefault())
                   .ToList();

            return result;
        }
        /// <summary>
        ///  Searchs a company by name
        /// </summary>
        /// <param name="compName"> parameter name for specific customer</param>
        /// <returns></returns>
       public List<Company> GetCompanyNameByName(string compName)
        {
            var querycompany = db.Company.Where( cpn => cpn.CompanyName == compName).ToList();               

            return querycompany;
        }

        /// <summary>
        /// Implement IDisposable interface in order  to clean up  the unmanaged resources.
        /// </summary>
        /// <param name="disposing"> An integer parameter </param>
     
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            
            base.Dispose(disposing); 
        }
       
    }
}
