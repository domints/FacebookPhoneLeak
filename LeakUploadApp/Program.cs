using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LeakUploadApp.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;

namespace LeakUploadApp
{
    class Program
    {
        const string API_URL = "http://localhost:5000/api/";
        static HttpClient client;
        static byte[] salt;
        static async Task Main(string[] args)
        {
            if(args.Length < 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("You need to provide file name!");
                return;
            }

            Console.Write("Please provide API Key: ");
            var apiKey = Console.ReadLine().Trim();

            client = new HttpClient();
            client.BaseAddress = new Uri(API_URL);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

            var collections = await GetAsync<List<Collection>>("admin/collections").ConfigureAwait(false);
            var collectionName = Path.GetFileNameWithoutExtension(args[0]);
            Console.Write("Please provide collection name: ");
            var inputCollectionName = Console.ReadLine().Trim();
            collectionName = string.IsNullOrWhiteSpace(inputCollectionName) ? collectionName : inputCollectionName;
            var selectedCollection = collections.Find(c => string.Equals(c.Name, collectionName.Trim(), StringComparison.InvariantCultureIgnoreCase));
            var collectionId = -1;
            if(selectedCollection == null)
            {
                collectionId = await PostAsync<int>("admin/collection", collectionName.Trim()).ConfigureAwait(false);
            }
            else
            {
                collectionId = selectedCollection.Id;
            }

            Console.WriteLine($"Going with collection Id:{collectionId}");

            long lastLineNo = 0; 
            long currentLineNo = 0;

            Console.WriteLine("Which line to start from? [0]: ");
            var lineToStart = Console.ReadLine();
            if (long.TryParse(lineToStart, out var iLineToStart))
                lastLineNo = iLineToStart;

            salt = Encoding.ASCII.GetBytes(await client.GetStringAsync("search/publicSalt").ConfigureAwait(false));
            List<InsertEntry> currentBatch = new();
            async Task upload()
            {
                await PostAsync($"admin/collection/{collectionId}/entries", currentBatch).ConfigureAwait(false);
                currentBatch.Clear();
                var msg = $"{lastLineNo} - {currentLineNo - 1}";
                await File.WriteAllTextAsync("status.txt", msg).ConfigureAwait(false);
                Console.WriteLine(msg);
                lastLineNo = currentLineNo;
            }

            StreamReader facebook = new(args[0]);
            string line;
            while ((line = facebook.ReadLine()) != null)
            {
                if(currentLineNo < iLineToStart)
                {
                    currentLineNo++;
                    continue;
                }

                currentBatch.Add(PrepareEntry(line));
                currentLineNo++;
                if(currentBatch.Count >= 1000)
                    await upload().ConfigureAwait(false);
            }

            if(currentBatch.Count > 0)
                await upload().ConfigureAwait(false);
        }

        private static InsertEntry PrepareEntry(string line)
        {
            var row = line.Split(":");
            return new InsertEntry
            {
                PhoneHash = GenerateBase64Hash(row[0]),
                IdHash = GenerateBase64Hash(row[1]),
                HasName = !string.IsNullOrWhiteSpace(row[2]) || !string.IsNullOrWhiteSpace(row[3]),
                HasGender = !string.IsNullOrWhiteSpace(row[4]),
                HasLivingPlace = !string.IsNullOrWhiteSpace(row[5]),
                HasComingPlace = !string.IsNullOrWhiteSpace(row[6]),
                HasRelationshipStatus = !string.IsNullOrWhiteSpace(row[7]),
                HasWorkplace = !string.IsNullOrWhiteSpace(row[8]),
                HasEmail = !string.IsNullOrWhiteSpace(row[10]),
                HasBirthdate = !string.IsNullOrWhiteSpace(row[11]) && row[11] != "1/1/0001 12-00-00 AM"
            };
        }

        private static string GenerateBase64Hash(string data)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                data,
                salt,
                KeyDerivationPrf.HMACSHA512,
                100,
                64));
        }

        protected static async Task<T> PostAsync<T>(string url, object body)
        {
            var bodyJson = JsonConvert.SerializeObject(body);
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync(url, content).ConfigureAwait(false);
            var responseString = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try {
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(responseString);
                throw;
            }
        }

        protected static async Task PostAsync(string url, object body)
        {
            var jsonBody = JsonConvert.SerializeObject(body);
            Console.WriteLine($"Pushing {jsonBody.Length} bytes JSON...");
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync(url, content).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();
        }

        protected static async Task<T> GetAsync<T>(string url)
        {
            var httpResponse = await client.GetAsync(url).ConfigureAwait(false);
            var responseString = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try {
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(responseString);
                throw;
            }
        }

        protected static async Task<T> PutAsync<T>(string url, object body)
        {
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var httpResponse = await client.PutAsync(url, content).ConfigureAwait(false);
            var responseString = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try {
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(responseString);
                throw;
            }
        }

        protected static async Task<T> DeleteAsync<T>(string url)
        {
            var httpResponse = await client.DeleteAsync(url).ConfigureAwait(false);
            var responseString = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try {
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(responseString);
                throw;
            }
        }
    }
}
