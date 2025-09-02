using System;
using Core.Entities;

namespace Core.Specifications;

public class TypeListSpecification : BaseSpecification<Product,string>
{
    public TypeListSpecification()
    {
        SetSelect(x => x.Type);
        SetIsDistinct();
    }
}
