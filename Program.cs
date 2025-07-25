using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartPulseCase1;

namespace SmartPulseCase
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("SmartPulse Case'e hoş geldiniz!");

                Console.Write("EPİAŞ Kullanıcı Adı: ");
                string username = Console.ReadLine();

                Console.Write("EPİAŞ Şifre: ");
                string password = Console.ReadLine();

                string tgt = await GetTgtAsync(username, password);

                string jsonData = await GetTransactionDataAsync(tgt);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Root result = JsonSerializer.Deserialize<Root>(jsonData, options);
                List<Transaction> transactions = result.items;

                Console.WriteLine("Toplam işlem sayısı: " + transactions.Count);

                List<ContractSummary> grouped = transactions
               .GroupBy(t => t.contractName)
               .Select(g =>
               {
                  double totalQuantity = g.Sum(x => x.quantity / 10.0);
                  double totalValue = g.Sum(x => (x.price * x.quantity) / 10.0);
                  double averagePrice = totalQuantity == 0 ? 0 : totalValue / totalQuantity;

                DateTime dateTime;
                try
                {
                 dateTime = ContractParser.ParseContractToDateTime(g.Key);
                }
                catch
                {
               Console.Error.WriteLine($"Geçersiz contractName: {g.Key}");
               dateTime = DateTime.MinValue;
                }

                return new ContractSummary
                 {
                   Contract = g.Key,
                   DateTime = dateTime,
                   TotalQuantity = totalQuantity,
                   TotalValue = totalValue,
                   AvgPrice = averagePrice
                 };
                })
                .OrderBy(x => x.DateTime)
                .ToList();


                // konsola yazdır
                Console.WriteLine("Contract     | Tarih              | Miktar   | Tutar     | Ortalama");
                foreach (var item in grouped)
                {
                    Console.WriteLine($"{item.Contract,-12} | {item.DateTime:dd.MM.yyyy HH:mm} | {item.TotalQuantity,8:F2} | {item.TotalValue,9:F2} | {item.AvgPrice,9:F2}");
                }

                // csv olarak aktar
                ExportToCsv(grouped, "rapor.csv");
                Console.WriteLine("CSV olarak dışa aktarıldı: rapor.csv");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Bir hata oluştu: " + ex.Message);
            }
        }

        static void ExportToCsv(List<ContractSummary> data, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Contract,Tarih,Miktar,Tutar,Ortalama");

            foreach (var item in data)
            {
                sb.AppendLine($"{item.Contract},{item.DateTime:dd.MM.yyyy HH:mm},{item.TotalQuantity:F2},{item.TotalValue:F2},{item.AvgPrice:F2}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }


        static async Task<string> GetTgtAsync(string username, string password)
        {
            string cacheFile = "tgt_cache.txt";

            // 1. CACHE VAR MI?
            if (File.Exists(cacheFile))
            {
                var lines = File.ReadAllLines(cacheFile);
                if (lines.Length == 3 && lines[2] == username &&
                    DateTime.TryParse(lines[0], out DateTime cachedTime) &&
                    DateTime.Now - cachedTime < TimeSpan.FromHours(2))
                {
                    Console.WriteLine("TGT cache'ten alındı.");
                    return lines[1];
                }
                else
                {
                    // Farklı kullanıcıysa veya süresi geçtiyse eski cache’i sil
                    File.Delete(cacheFile);
                    Console.WriteLine("Cache geçersiz, silindi.");
                }
            }

            // 2. YENİDEN LOGIN
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("username", username),
        new KeyValuePair<string, string>("password", password)
    });

            var response = await client.PostAsync("https://giris.epias.com.tr/cas/v1/tickets", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("TGT alınamadı. Hatalı kullanıcı adı veya şifre.");
                return null;
            }

            var html = await response.Content.ReadAsStringAsync();

            var tgtLine = html.Split('\n')[0];
            var start = tgtLine.IndexOf("/cas/v1/tickets/") + "/cas/v1/tickets/".Length;
            var end = tgtLine.IndexOf("\"", start);
            var tgt = tgtLine.Substring(start, end - start);

            // 3. CACHE’E YAZ (Tarih, TGT, Kullanıcı adı)
            File.WriteAllLines(cacheFile, new[]
            {
        DateTime.Now.ToString("s"),
        tgt,
        username
    });

            Console.WriteLine("Yeni TGT alındı ve cache’lendi.");
            return tgt;
        }


        static async Task<string> GetTransactionDataAsync(string tgt)
        {
            var client = new HttpClient();
            //tgt ekle
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("TGT", tgt);
            //istek
            var body = new
            {
                startDate = "2025-01-27T00:00:00+03:00",
                endDate = "2025-01-28T00:00:00+03:00"
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //api'ye istek gönderme
            var response = await client.PostAsync("https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Veri alınamadı! HTTP Hata: " + response.StatusCode);
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
