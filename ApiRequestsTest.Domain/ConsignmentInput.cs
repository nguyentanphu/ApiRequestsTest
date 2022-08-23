namespace ApiRequestsTest.Domain;

public class ConsignmentInput
{
    public string SourceAddress { get; set; }
    public string DestinationAddress { get; set; }
    public CartonDimension[] Cartons { get; set; }
    
}

public class CartonDimension
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Length { get; set; }
}