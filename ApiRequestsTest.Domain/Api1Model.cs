namespace ApiRequestsTest.Domain;

public class Api1Model
{
    public string ContactAddress { get; set; }
    public string WarehouseAddress { get; set; }
    public Api1ModelDimension[] PackageDimensions { get; set; }
}

public class Api1ModelDimension
{
    public double Width { get; set; }
    public double Length { get; set; }
    public double Height { get; set; }
}