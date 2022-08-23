namespace Api1;

public class Consignment
{
    public string Consignor { get; set; }
    public string Consignee { get; set; }
    public Carton[] Cartons { get; set; }
}

public class Carton
{
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Height { get; set; }
}