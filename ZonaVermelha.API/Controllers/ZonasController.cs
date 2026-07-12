using Microsoft.AspNetCore.Mvc;
using ZonaVermelha.API.Services;

namespace ZonaVermelha.API.Controllers;


    [ApiController]
    [Route("api/zonas")]
    public class ZonasController(ZonaService zonaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BuscarZonasProximasAsync([FromQuery] double latitude,[FromQuery] double longitude)
    {
        var response = await zonaService.BuscarZonasProximasAsync(latitude, longitude);
        return Ok (response);
    }
}