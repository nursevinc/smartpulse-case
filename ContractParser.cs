using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPulseCase1
{
    public static class ContractParser
    {
        public static DateTime ParseContractToDateTime(string contract)
        {
            int yil = 2000 + int.Parse(contract.Substring(2, 2));
            int ay = int.Parse(contract.Substring(4, 2));
            int gun = int.Parse(contract.Substring(6, 2));
            int saat = int.Parse(contract.Substring(8, 2));
            return new DateTime(yil, ay, gun, saat, 0, 0);
        }
    }
}

