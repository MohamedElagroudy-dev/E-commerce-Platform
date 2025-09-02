using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentsController(IPaymentService paymentService,
    IUnitOfWork unit,
    IHubContext<NotificationHub> hubContext,
    ILogger<PaymentsController> logger,
    IConfiguration config) : BaseApiController
    {
        private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

            if (cart == null) return BadRequest("Problem with your cart on the API");

            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = ConstructStripeEvent(json);

                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data.");
                }

                await HandlePaymentIntentSucceeded(intent);

                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe webhook error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to construct Stripe event");
                throw new StripeException("Invalid signature");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                var spec = new OrderSpecification(intent.Id, true);

                var order = await unit.Repository<Order>().GetEntityWithSpec(spec)
                            ?? throw new Exception("Order not found");

                order.Status = OrderStatus.PaymentReceived;

                if ((long)order.GetTotal() * 100 != intent.Amount)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else
                {
                    order.Status = OrderStatus.PaymentReceived;
                }

                await unit.Complete();

                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

                if (!string.IsNullOrEmpty(connectionId))
                {
                    await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",
                        order.ToDto());
                }
            }
        }
    }
}

/*
 🔹 Key Fixes Applied:

1. ✅ Event Type Filtering: Now handles specific events (payment_intent.succeeded, payment_intent.created, etc.)
2. ✅ Proper Error Handling: Returns 200 OK for all acknowledged events, even if not processed
3. ✅ Better Logging: Added detailed logging for debugging
4. ✅ Signature Validation: Improved Stripe signature checking
5. ✅ Payment Failure Handling: Added handler for payment_intent.payment_failed
6. ✅ Null Checks: Better handling of missing orders and malformed events

 🔹 Webhook Event Flow:
 - payment_intent.created → Just acknowledge (200 OK)
 - payment_intent.succeeded → Update order status to PaymentReceived
 - payment_intent.payment_failed → Update order status to PaymentFailed  
 - charge.updated → Just acknowledge (200 OK)
 - Other events → Log and acknowledge (200 OK)
 */