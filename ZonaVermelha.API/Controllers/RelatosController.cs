using Microsoft.AspNetCore.Mvc;
using ZonaVermelha.API.Services;
using ZonaVermelha.Communication.Requests;

namespace ZonaVermelha.API.Controllers;

[ApiController]
[Route("api/relatos")]
public class RelatosController(RelatoService relatoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CriarRelato([FromBody] RequestRelatoJson requestRelatoJson)
    {
        var response = await relatoService.CriarRelatoAsync(requestRelatoJson);
        return Ok(response);
    }
}