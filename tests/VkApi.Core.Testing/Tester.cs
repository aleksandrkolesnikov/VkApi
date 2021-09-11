using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Xunit;


namespace VkApi.Core.Testing
{
    public class Tester
    {
        public Tester()
        {
            static (string, string) GetCredentials()
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load("../../../settings.xml");

                var credentials = xmlDoc.DocumentElement.SelectSingleNode("/settings/credentials");
                var login = credentials.Attributes["login"].Value;
                var password = credentials.Attributes["password"].Value;

                return (login, password);
            }

            (var login, var password) = GetCredentials();
            sut = Client.Create(login, password).Result;
        }

        [Fact(DisplayName = "Get All Documents")]
        public async Task TestGetAllDocs()
        {
            var docs = await sut.GetDocumentsAsync();

            Assert.NotNull(docs);
        }

        [Fact(DisplayName = "Upload Document")]
        public async Task TestUploadDoc()
        {
            var name = "simple-text.txt";
            var data = Encoding.UTF8.GetBytes("Hello World!. This is simple text!");
            using var md5 = MD5.Create();
            var expectedHash = md5.ComputeHash(data);

            var doc = await sut.UploadDocumentAsync(name, data);

            Assert.Equal(name, doc.Title);
            Assert.Equal(expectedHash, doc.Hash);
        }

        [Fact(DisplayName = "Remove Document")]
        public async Task TestRemoveDoc()
        {
            var name = "simple-text.txt";
            var data = Encoding.UTF8.GetBytes("Hello World!. This is simple text!");
            var doc = await sut.UploadDocumentAsync(name, data);

            await sut.RemoveDocumentAsync(doc);

            Assert.True(true);
        }

        [Fact(DisplayName = "Make Multiple Requests")]
        public void TestMultipleRequests()
        {
            var tasks = new Task<IEnumerable<Document>>[100];
            for (int i = 0; i < tasks.Length; ++i)
            {
                var docs = sut.GetDocumentsAsync();
                tasks[i] = docs;
            }

            Task.WaitAll(tasks);
        }

        private readonly Client sut;
    }
}
