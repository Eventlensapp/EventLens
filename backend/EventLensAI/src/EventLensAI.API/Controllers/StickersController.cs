using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Photos.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/stickers")]
public sealed class StickersController(IStickerService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<StickerDto>>>>List([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<StickerDto>>.Ok(await service.ListAsync(organizationId,ct)));
}
