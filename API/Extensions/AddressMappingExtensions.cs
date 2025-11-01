using System;
using API.DTOs;
using Core.Entities;

namespace API.Extensions;

public static class AddressMappingExtensions
{
    public static AddressDTO? ToDTO(this Address address)
    {
        if (address == null) return null;
        return new AddressDTO
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }
    public static Address ToEntity(this AddressDTO addressDTO)
    {
        return addressDTO == null
            ? throw new ArgumentNullException(nameof(addressDTO))
            : new Address
            {
                Line1 = addressDTO.Line1,
                Line2 = addressDTO.Line2,
                City = addressDTO.City,
                State = addressDTO.State,
                PostalCode = addressDTO.PostalCode,
                Country = addressDTO.Country
            };
    }
    public static void UpdateFromDTO(this Address address,AddressDTO addressDTO)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));
        if (addressDTO == null) throw new ArgumentNullException(nameof(addressDTO));
        
        address.Line1 = addressDTO.Line1;
        address.Line2 = addressDTO.Line2;
        address.City = addressDTO.City;
        address.State = addressDTO.State;
        address.PostalCode = addressDTO.PostalCode;
        address.Country = addressDTO.Country; 
    }
}
