namespace ApiRequestsTest.Domain;

public class Api2Model
{
    public string Consignor { get; set; }
    public string Consignee { get; set; }
    public Api2ModelDimension[] Cartons { get; set; }
}

public class Api2ModelDimension
{
    public double Width { get; set; }
    public double Length { get; set; }
    public double Height { get; set; }
}