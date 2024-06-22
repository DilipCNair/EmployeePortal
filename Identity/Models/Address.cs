namespace Identity.Models;

public class Address
{
    public Guid Id { get; set; }

    public string? HouseNo { get;set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PinCode { get; set; }

    public string? Landmark { get; set; }

}
