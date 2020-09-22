using Accountant.Library;
using Accountant.Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Accountant.Test
{
    public class Test
    {
        private OfxManager ofxManager;
        private readonly string[] filePaths;

        public Test()
        {
            filePaths = new string[]
            {
                Path.Combine(Environment.CurrentDirectory, @"OFX\extrato1.ofx"),
                Path.Combine(Environment.CurrentDirectory, @"OFX\extrato2.ofx"),
            };

            ofxManager = new OfxManager();
        }

        [Fact]
        public void ImportFile1()
        {
            var ofx = ImportOfx(filePaths[0]);

            Assert.Equal(string.Empty, ofx.ErrorMessage);
        }

        [Fact]
        public void ImportFile2()
        {
            var ofx = ImportOfx(filePaths[1]);

            Assert.Equal(string.Empty, ofx.ErrorMessage);
        }

        [Fact]
        public void JoinImports()
        {
            var joinedOfx = JoinOfxs();

            var totalValue = joinedOfx.Transactions.Sum(i => i.TransactionValue);

            Assert.Equal(4025.34M, totalValue);
        }

        [Fact]
        public void SaveAndGetImportation()
        {
            var joinedOfx = JoinOfxs();

            var result = ofxManager.SaveOfxFile(joinedOfx);

            Assert.Equal(string.Empty, result);

            var ofx = ofxManager.GetOfxFile();

            Assert.True(ofx != null);
        }

        [Fact]
        public void FileNotFound()
        {
            var ofxFilePath = Path.Combine(Environment.CurrentDirectory, @"OFX\extrato999.ofx");
            var ofx = ImportOfx(ofxFilePath);

            Assert.Equal("OFX file not found", ofx.ErrorMessage);
        }

        [Fact]
        public void InvalidFile()
        {
            var ofxFilePath = Path.Combine(Environment.CurrentDirectory, @"OFX\extrato3.ofx");
            var ofx = ImportOfx(ofxFilePath);

            Assert.Equal("Invalid File", ofx.ErrorMessage);
        }

        [Fact]
        public void InvalidJoin()
        {
            var ofx = ofxManager.JoinOfxs(null);
            Assert.True(ofx == null);

            ofx = ofxManager.JoinOfxs(new List<Ofx>());
            Assert.True(ofx == null);
        }

        private Ofx ImportOfx(string filePath)
        {
            var ofxReader = new OfxReader(filePath);

            return ofxReader.Parse();
        }

        private Ofx JoinOfxs()
        {
            var ofxs = new List<Ofx>();

            foreach (var path in filePaths)
            {
                var ofx = ImportOfx(path);
                ofxs.Add(ofx);
            }

            return ofxManager.JoinOfxs(ofxs);
        }
    }
}
