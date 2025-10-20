using System;
using Core.Entities;

namespace Core.Interfaces;

public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string id);
    Task<ShoppingCart?> SetCartAsync(ShoppingCart shoppingCart);
    Task<bool> DeleteCartAsync(string id);
}
