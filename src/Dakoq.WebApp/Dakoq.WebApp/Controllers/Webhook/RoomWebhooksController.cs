using Dakoq.Domain.Repository;
using Dakoq.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dakoq.WebApp.Controllers.Webhook
{
    [Route("api/webhooks/{id}")]
    [ApiController]
    public class RoomWebhooksController(
        TraqJwtValidator jwtValidator,
        IRepositoryFactory repositoryFactory
        ) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> IndexAsync(
            [FromRoute(Name = "id")] Guid id,
            [FromHeader(Name = "DAKOQ-Signature")] string signature)
        {
            var ct = HttpContext.RequestAborted;
            await using var repo = await repositoryFactory.CreateAsync(ct);
            if (!await repo.VerifySecretAsync(id, signature, ct))
            {
                return NotFound();
            }

            var jwt = await new StreamReader(HttpContext.Request.Body, Encoding.UTF8).ReadToEndAsync(ct);
            var user = await jwtValidator.ValidateAsync(jwt, ct);
            if (user is null)
            {
                return BadRequest();
            }

            var roomId = (await repo.GetRoomWebhookAsync(id, ct)).RoomId;
            await repo.JoinToOrLeaveFromRoomAsync(roomId, user.UserId, ct);
            return NoContent();
        }
    }
}
