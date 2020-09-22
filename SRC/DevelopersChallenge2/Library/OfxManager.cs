using Accountant.Library.Commom;
using Accountant.Library.Model;
using Accountant.Library.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Accountant.Library
{
    public class OfxManager
    {
        private readonly RepositoryManager repositoryManager;

        public OfxManager()
        {
            repositoryManager = new RepositoryManager();
        }

        public Ofx JoinOfxs(List<Ofx> ofxs)
        {
            if (ofxs == null || ofxs.Count < 1)
            {
                return null;
            }

            var ofx = ofxs.FirstOrDefault();

            foreach (var secondOfx in ofxs)
            {
                MergeOfxs(ofx, secondOfx);
            }

            return ofx;
        }

        private void MergeOfxs(Ofx ofx1, Ofx ofx2)
        {
            var minDate = ofx1.Transactions.Min(x => x.Date);
            var maxDate = ofx1.Transactions.Max(x => x.Date);

            var newOfx2 = ofx2.Transactions.Where(x => x.Date < minDate || x.Date > maxDate).ToList();
            ofx1.Transactions.AddRange(newOfx2);
        }

        public string SaveOfxFile(Ofx ofx)
        {
            var jsonFile = JsonManager.ObjectToJson(ofx);

            return repositoryManager.CreateFile(jsonFile);
        }

        public Ofx GetOfxFile()
        {
            var jsonFile = repositoryManager.GetFile();

            return string.IsNullOrWhiteSpace(jsonFile) ? null : JsonManager.JsonToObject<Ofx>(jsonFile);
        }

    }
}
