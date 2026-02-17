using System;
using Core.Entities;
using Core.Interfaces;
using Stripe;

namespace Infrastructure.Services;

public class CouponService : ICouponService
{
    public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
    {
        PromotionCodeService promotionCodeService = new ();
        PromotionCodeListOptions options = new()
        {
            Code = code,
            Active = true,
            Limit = 1,
            Expand = ["data.promotion.coupon"]
        };
        StripeList<PromotionCode> promoCodes = await promotionCodeService.ListAsync(options);
        if(promoCodes.Data.Count == 0) return null;
        var promoCode = promoCodes.Data.First();
        Coupon coupon = promoCode.Promotion.Coupon;
        if(coupon == null) return null;
        return new AppCoupon
        {
            Name = coupon.Name,
            AmountOff = coupon.AmountOff,
            PercentOff = coupon.PercentOff,
            PromotionCode = code,
            CouponId = coupon.Id
        };
    }
}
