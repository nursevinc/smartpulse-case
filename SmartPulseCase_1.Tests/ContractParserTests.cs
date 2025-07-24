using System;
using Xunit;
using SmartPulseCase_1;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SmartPulseCase1;

namespace SmartPulseCase_1.Tests
{
    public class ContractParserTests
    {
        [Theory]
        [InlineData("PH24112706", 2024, 11, 27, 6)]
        [InlineData("PH25021518", 2025, 2, 15, 18)]
        [InlineData("PH22010100", 2022, 1, 1, 0)]
        public void ParseContractToDateTime_DogruTarih(string contract, int yil, int ay, int gun, int saat)
        {
            // Beklenen deðer
            var beklenen = new DateTime(yil, ay, gun, saat, 0, 0);

            // Gerçek deðer
            var sonuc = ContractParser.ParseContractToDateTime(contract);

            // Karþýlaþtýr
            Assert.Equal(beklenen, sonuc);
        }
    }
}
