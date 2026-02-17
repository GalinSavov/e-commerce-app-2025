using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers
{
    public class CouponsController(ICouponService couponService) : BaseApiController
    {
        [HttpGet("{code}")]
        public async Task<ActionResult<AppCoupon>> ValidateCoupon(string code)
        {
            var coupon = await couponService.GetCouponFromPromoCode(code);
            return coupon != null ? Ok(coupon) : BadRequest("Could not find coupon code!");
        }
    }
}
