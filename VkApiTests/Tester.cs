using VkApi;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace VkApiTests
{
    public class Tester
    {
        [Fact(DisplayName = "Get All Documents")]
        public async Task GetDocumentsTest()
        {
            var docs = await VkClient.Get.GetDocuments();

            Assert.NotEmpty(docs);
        }

        [Fact(DisplayName = "Upload Document")]
        public async Task UploadDocumentTest()
        {
            var path = "../../../TestData/Program.fs";
            var name = ":MyTestFolder:TestData:MyCustomFileName.txt";
            using var stream = File.OpenRead(path);
            var doc = await VkClient.Get.UploadDocument(stream, name);

            Assert.Equal(name, doc.Title);
        }

        [Fact(DisplayName = "Upload Large Document")]
        public async Task UploadLargeDocumentTest()
        {
            var path = "../../../TestData/LargeFile.7z";
            var name = ":MyTestFolder:TestData:MyLargeFile.7z";
            using var stream = File.OpenRead(path);
            var doc = await VkClient.Get.UploadDocument(stream, name);

            Assert.Equal(name, doc.Title);
        }

        [Fact(DisplayName = "Remove Document")]
        public async Task RemoveDocumentTest()
        {
            /*var client = VkClient.Get;
            var doc = await client.UploadDocument("Program.fs");
            await client.RemoveDocument(doc);
            var docs = await client.GetDocuments();

            Assert.DoesNotContain(doc, docs);*/
        }

        [Fact(DisplayName = "Make Many Requests And Failed")]
        public void MakeManyRequestTest()
        {
            void TestCode()
            {
                var tasks = new Task<IEnumerable<Document>>[30];
                var client = VkClient.Get;
                for (int i = 0; i < tasks.Length; ++i)
                {
                    var docs = client.GetDocuments();
                    tasks[i] = docs;
                }

                Task.WaitAll(tasks);
            }

            var exception = Assert.Throws<AggregateException>(TestCode);
            Assert.Equal(typeof(VkException), exception.InnerException.GetType());
        }
    }
}
