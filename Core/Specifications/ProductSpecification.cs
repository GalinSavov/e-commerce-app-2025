using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification: BaseSpecification<Product>
{
    public ProductSpecification(ProductSpecParams productParams) : base(x=>
    (!productParams.Brands.Any() || productParams.Brands.Contains(x.Brand))
    && (!productParams.Types.Any() || productParams.Types.Contains(x.Type))
    )
    {
        switch (productParams.Sort)
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
