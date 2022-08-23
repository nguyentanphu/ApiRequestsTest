namespace Api1;

public class Consignment
{
    public string ContactAddress { get; set; }
    public string WarehouseAddress { get; set; }
    public PackageDimension[] PackageDimensions { get; set; }
}

public class PackageDimension
{
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Height { get; set; }
}