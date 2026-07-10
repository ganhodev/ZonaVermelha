using Microsoft.AspNetCore.Mvc;
using ZonaVermelha.API.Services;
using ZonaVermelha.Communication.Requests;

namespace ZonaVermelha.API.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController(UsuarioService usuarioService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CriarUsuarioAsync([FromBody] RequestUsuarioJson requestUsuarioJson)
    {
        var response = await usuarioService.CriarUsuarioAsync(requestUsuarioJson);
        return Ok(response);
    }
}
