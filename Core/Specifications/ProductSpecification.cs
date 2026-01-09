using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification: BaseSpecification<Product>
{
    public ProductSpecification(ProductSpecParams productParams) : base(x=>
    (string.IsNullOrEmpty(productParams.Search) || x.Name.Contains(productParams.Search, StringComparison.CurrentCultureIgnoreCase)) &&
    (!productParams.Brands.Any() || productParams.Brands.Contains(x.Brand))
    && (!productParams.Types.Any() || productParams.Types.Contains(x.Type))
    )
    {
        SetApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);
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
