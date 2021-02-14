using VkApi;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;


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

        [Fact(DisplayName = "Upload Document From Stream")]
        public async Task UploadFromMemoryStreamTest()
        {
            byte[] buffer = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            using var stream = new MemoryStream(buffer, writable: false);
            var actualName = "Stream.dat";
            var actualHash = ComputeMD5Hash(stream);
            stream.Position = 0;
            var doc = await VkClient.Get.UploadDocument(stream, actualName);

            Assert.Equal(actualName, doc.Title);
            Assert.Equal((ulong)buffer.Length, doc.Size);
            Assert.Equal(actualHash, doc.Hash);
        }

        [Fact(DisplayName = "Upload Document From Buffer")]
        public async Task UploadFromBufferTest()
        {
            byte[] buffer = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var actualName = "Buffer.bin";
            var actualHash = ComputeMD5Hash(buffer);
            var doc = await VkClient.Get.UploadDocument(buffer, actualName);

            Assert.Equal(actualName, doc.Title);
            Assert.Equal((ulong)buffer.Length, doc.Size);
            Assert.Equal(actualHash, doc.Hash);
        }

        [Fact(DisplayName = "Upload Document From File")]
        public async Task UploadFromFileTest()
        {
            var path = "../../../TestData/CSR057069_0_3376112712.xlsx";
            var actualName = "Table.xlsx";
            byte[] actualHash = null;
            using (var stream = File.OpenRead(path))
            {
                actualHash = ComputeMD5Hash(stream);
            }

            using var fileStream = File.OpenRead(path);
            var doc = await VkClient.Get.UploadDocument(fileStream, actualName);

            Assert.Equal(actualName, doc.Title);
            Assert.Equal((ulong)fileStream.Length, doc.Size);
            Assert.Equal(actualHash, doc.Hash);
        }

        [Fact(DisplayName = "Upload Document From Large File")]
        public async Task UploadFromLargeFileTest()
        {
            var path = "../../../TestData/LargeFile.7z";
            var actualName = "MyLargeFile.7z";
            byte[] actualHash = null;
            using (var stream = File.OpenRead(path))
            {
                actualHash = ComputeMD5Hash(stream);
            }

            using var fileStream = File.OpenRead(path);
            var doc = await VkClient.Get.UploadDocument(fileStream, actualName);

            Assert.Equal(actualName, doc.Title);
            Assert.Equal((ulong)fileStream.Length, doc.Size);
            Assert.Equal(actualHash, doc.Hash);
        }

        [Fact(DisplayName = "Remove Document")]
        public async Task RemoveDocumentTest()
        {
            var path = "../../../TestData/Program.fs";
            var name = "FileForDelete.txt";
            using var stream = File.OpenRead(path);
            var client = VkClient.Get;
            var doc = await client.UploadDocument(stream, name);
            await client.RemoveDocument(doc);
            var docs = await client.GetDocuments();

            Assert.DoesNotContain(doc, docs);
        }

        [Fact(DisplayName = "Failed With Too Many Requests Per Second")]
        public void MakeManyRequestTest()
        {
            static void TestCode()
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
            Assert.Equal(typeof(TooManyRequestsPerSecondException), exception.InnerException.GetType());
        }

        private static byte[] ComputeMD5Hash(Stream stream)
        {
            using var md5 = MD5.Create();

            return md5.ComputeHash(stream);
        }

        private static byte[] ComputeMD5Hash(byte[] buffer)
        {
            using var md5 = MD5.Create();

            return md5.ComputeHash(buffer);
        }
    }
}
