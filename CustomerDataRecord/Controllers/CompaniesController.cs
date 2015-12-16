using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CustomerDataRecord.Models;
using System.Text;

namespace CustomerDataRecord.Controllers
{   
    /// <summary>
    /// A Companies controller class, which encapsulates the methods called users actions.
    /// </summary>
    public class CompaniesController : Controller
    {
        private CustomerDataEntities db = new CustomerDataEntities();


       
        /// <summary>
        ///  Retunrs a list of Company to the wiew
        /// </summary>
        /// <returns> company list to view </returns>
        public ActionResult Index()
        {
            var compList = (from comp in db.Company
                            select comp).ToList();
            return View(compList);
        }

       
        /// <summary>
        ///  Get the detail of each company based on the company id
        /// </summary>
        /// <param name="id"> An integer company id</param>
        /// <returns> A Company object to list</returns>
        [HttpGet]
        public ActionResult Details(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = FindCompanyById(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        

        /// <summary>
        /// Create an insert a company  object to list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a new company object an inserts it to the list.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Company.Add(company);
                db.SaveChanges();
                return  RedirectToAction("Index");
            }

            return View(company);// PartialView("_CreateCompanyPartial", company);
        }

        // GET: Companies/Edit/5
        /// <summary>
        /// Update a  caompany info based on the a specific id
        /// </summary>
        /// <param name="id"> An integer id</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);// PartialView("_EditCompanyPartial", company);
        }
        /// <summary>
        /// Update a company info and saves it to the db.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
               return RedirectToAction("Index", "Companies");
            }
            return View(company);// PartialView("_EditCompanyPartial", company);
        }

        // GET: Companies/Delete/5
        /// <summary>
        /// Delete a Company from the list based on a specific id
        /// </summary>
        /// <param name="id"> An integer id</param>
        /// <returns> partial view of company model</returns>
        
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete",company);
        }

        
        // POST: Companies/Delete/5
        /// <summary>
        /// Post method for deleting  a specific company
        /// </summary>
        /// <param name="id"> An integer id company id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Company.Find(id);
            db.Company.Remove(company);
            db.SaveChanges();

            return RedirectToAction("Index");
            
        }
        /// <summary>
        ///  Search a list of customers for a company.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public PartialViewResult FindCustomerForCompany(int id)
        {
            var customerList = (from cust in db.Customer
                                join custComp in db.Customer_Company on cust.CustomerID equals custComp.CustomerID
                                join cp in db.Company on custComp.CompanyID equals cp.CompanyID
                                where cp.CompanyID == id
                                select cust).ToList();
            
            if (customerList.Count == 0)
            {
                return PartialView("_NoCustomerFound");// View("NoCompany");
            }            
            return PartialView("_CustomerList", customerList);

        }
        /// <summary>
        ///  find the employers by employee id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       public PartialViewResult GetCompanyEmployees(int id)
        {
            var queryEmp = (from comp in db.Company
                            join emp in db.Employees on comp.CompanyID equals emp.CompanyID
                            where comp.CompanyID == id
                            select emp).ToList();

            if (queryEmp.Count == 0)
            {
                return PartialView("_NoEmployeeFound");
            }
            else
            {
                return PartialView("_CompanyEmployees", queryEmp);
            }
            
        }
        /// <summary>
        /// Search a specific comapny based on an integer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns> A company object is returned</returns>

        public Company FindCompanyById(int ? id)
        {
            var queryComp = from cp in db.Company
                            where cp.CompanyID == id
                            select cp;
            Company company = queryComp.FirstOrDefault();

            return company;

        }
        /// <summary>
        /// Query  the company and Print company name and address 
        /// </summary>
        /// <returns></returns>

        public string FullName() 
        {
            var companyQuery = (from comp1 in db.Company
                                select comp1).ToList();

            StringBuilder builder = new StringBuilder();
            foreach (var item in companyQuery)
            {
                 builder.Append(item.CompanyName + " " + item.CompanyAddress);
            }

            return builder.ToString();           
            
        }
        /// <summary>
        /// Implements an IDisposable interface in order to clean up the unmanaged resources
        /// </summary>
        /// <param name="disposing"> a boolean parameter </param>

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
