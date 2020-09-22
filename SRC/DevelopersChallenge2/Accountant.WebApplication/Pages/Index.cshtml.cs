using Accountant.Library;
using Accountant.WebApplication.Commons;
using Accountant.WebApplication.Enums;
using Accountant.WebApplication.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accountant.WebApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OfxManager ofxManager;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(IWebHostEnvironment environment)
        {
            ofxManager = new OfxManager();
            _environment = environment;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public IFormFile[] Files { get; set; }
        public void OnPost()
        {
            if (Files == null || Files.Length < 1)
            {
                AddCookie(Cookies.UploadFile.ToString(), true);
                return;
            }

            var ofxs = new List<Library.Model.Ofx>();

            var oldOfx = ofxManager.GetOfxFile();
            if (oldOfx != null)
            {
                ofxs.Add(oldOfx);
            }

            foreach (var file in Files)
            {
                var readFile = file.OpenReadStream();
                var ofxReader = new OfxReader(readFile);
                var ofx = ofxReader.Parse();

                ofxs.Add(ofx);
            }

            var mergedOfx = ofxManager.JoinOfxs(ofxs);
            ofxManager.SaveOfxFile(mergedOfx);

            AddCookie(Cookies.UploadFile.ToString(), true);
        }

        public JsonResult OnPostTotals()
        {
            try
            {
                var totals = new Totals
                {
                    total = 0,
                    totalCredits = 0,
                    totalDebits = 0,
                    totalMovements = 0
                };

                var ofx = ofxManager.GetOfxFile();
                if (ofx == null || ofx.Transactions.Count < 1)
                {
                    return new JsonResult(totals);
                }

                totals.total = ofx.Transactions.Sum(x => x.TransactionValue).ToString("#0.00");
                totals.totalDebits = ofx.Transactions.Where(x => x.Type == Library.Model.OfxTransaction.TransactionType.DEBIT).Sum(x => x.TransactionValue).ToString("#0.00");
                totals.totalCredits = ofx.Transactions.Where(x => x.Type == Library.Model.OfxTransaction.TransactionType.CREDIT).Sum(x => x.TransactionValue).ToString("#0.00");
                totals.totalMovements = ofx.Transactions.Count().ToString();

                return new JsonResult(totals);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult OnPostLoadTable()
        {
            try
            {
                var ofxTable = new Table { Header = new List<string>(), Items = new List<List<object>>() };

                var ofx = ofxManager.GetOfxFile();
                if (ofx == null || ofx.Transactions.Count < 1)
                {
                    return new JsonResult(ofxTable);
                }

                ofxTable.Header = new List<string> { "Date", "TransactionType", "TransactionValue", "Description" };

                foreach (var transaction in ofx.Transactions.OrderBy(x => x.Date))
                {
                    var line = new List<object>
                    {
                        transaction.Date.ToString("dd/MM/yyyy HH:mm:ss") ,
                        transaction.Type.ToString(),
                        transaction.TransactionValue.ToString("#0.00"),
                        transaction.Description,
                    };

                    ofxTable.Items.Add(line);
                }

                return new JsonResult(ofxTable);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void OnPostCreateLoading()
        {
            try
            {
                AddCookie(Cookies.UploadFile.ToString(), false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JsonResult OnPostUploadFileLoading()
        {
            try
            {
                Thread.Sleep(1000);

                var downloaded = GetCookie<bool>(Cookies.UploadFile.ToString());

                if (downloaded)
                {
                    AddCookie(Cookies.UploadFile.ToString(), false);
                }

                return new JsonResult(downloaded);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AddCookie(string key, object value, bool onlyHttp = true, DateTime? expirationDate = null)
        {
            CookiesManager.AddCookie(Request, Response, key, value, onlyHttp, expirationDate);
        }

        private T GetCookie<T>(string key)
        {
            try
            {
                return CookiesManager.GetCookie<T>(Request, key);
            }
            catch
            {
                return default;
            }
        }
    }
}
