namespace API.DTOs;

public class OrderItemDTO
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string PictureURL { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

}