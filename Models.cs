using System.Collections.Generic;

public class Transaction
{
    public long id { get; set; }
    public string date { get; set; }
    public string hour { get; set; }
    public string contractName { get; set; }
    public double price { get; set; }
    public double quantity { get; set; }
}

public class Root
{
    public List<Transaction> items { get; set; }
}
public class ContractSummary
{
    public string Contract { get; set; }
    public DateTime DateTime { get; set; }
    public double TotalQuantity { get; set; }
    public double TotalValue { get; set; }
    public double AvgPrice { get; set; }
}
