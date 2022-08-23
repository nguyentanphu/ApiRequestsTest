namespace Api1;

public class PackageInput
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public Package[] Packages { get; set; }
}

public class Package
{
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Height { get; set; }
}