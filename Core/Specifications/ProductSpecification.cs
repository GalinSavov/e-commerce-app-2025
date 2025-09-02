using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification: BaseSpecification<Product>
{
    public ProductSpecification(string? brand, string? type, string? sort) : base(x=>
    (string.IsNullOrWhiteSpace(brand) || x.Brand == brand) && (string.IsNullOrWhiteSpace(type) || x.Type == type)
    )
    {
        switch (sort)
        {
            case "priceDesc":
                SetOrderByDescending(x => x.Price);
                break;
            case "priceAsc":
                SetOrderBy(x => x.Price);
                break;
            default:
                SetOrderBy(x => x.Name);
                break;
        }
    }
}
