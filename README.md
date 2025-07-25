--SmartPulse Case Çalışması

Bu proje, SmartPulse tarafından verilen back-end geliştirme case çalışması kapsamında C# dili ile hazırlanmıştır. EPİAŞ Şeffaflık Platformu üzerinden işlem verileri alınarak işlenmiş, gruplama ve veri analiz işlemleri gerçekleştirilmiştir.

---

Amaç:

EPİAŞ API’sinden alınan işlem geçmişi verilerine dayanarak:

- ContractName’e göre işlemleri gruplamak
- Her bir contract için:
  -Toplam İşlem Miktarı
  -Toplam İşlem Tutarı
  -Ağırlıklı Ortalama Fiyat
- PHYYAAGGSS formatındaki contractName üzerinden tarih-parsleme işlemi yapmak
- Elde edilen sonuçları tarihe göre sıralayarak konsola tablo şeklinde yazdırmak
- Hesaplanan verileri csv dosyası olarak dışa aktarmak

---

 Kullanılan Teknolojiler

- C# (.NET8)
- HttpClient ile REST API bağlantısı
- `System.Text.JSON ile JSON parse
- LINQ ile gruplama ve hesaplama
- xUnit ile birim testi
- Basit bir TGT cacheleme mekanizması (2 saat süreli)

---

 Özellikler

- EPİAŞ login işlemi sonrası alınan TGT bilgisi cache dosyasına yazılır ve 2 saat süreyle kullanılır.
- API’den gelen JSON verisi C# modellerine deserialize edilir.
- Veriler işlenip, gruplandıktan sonra tablo halinde konsola yazdırılır.
-  Contract parser fonksiyonu birim test ile doğrulanmıştır.
- Sonuçlar csv dosyası olarak dışa aktarılır.
- Hatalar try-catch ile yönetilir ve kullanıcı bilgilendirilir.
---

 Test

ContractParser fonksiyonu, xUnit ile test edilmiştir.  
Test projesi: SmartPulseCase_1.Tests
---
 Projeyi Çalıştırma
 
Projeyi Visual Studio ile açın

Program.cs içindeki GetTgtAsync() metoduna kendi kullanıcı bilgilerinizi girin
(EPİAŞ kayıt: https://kayit.epias.com.tr)

Uygulamayı başlatın
