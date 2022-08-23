using System.Xml.Serialization;

namespace ApiRequestsTest.Domain;

[XmlRoot(ElementName = "PackageInput")]
public class Api3Model
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public Api3ModelDimension[] Packages { get; set; }
}

[XmlRoot(ElementName = "Package")]
public class Api3ModelDimension
{
    public double Width { get; set; }
    public double Length { get; set; }

    public double Height { get; set; }
}