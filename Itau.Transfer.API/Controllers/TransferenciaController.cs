using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Itau.Transfer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaController(ITransferenciaService transferenciaService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RealizaTransferencia([FromBody] TransferenciaDto transferencia, CancellationToken ct)
        {
            await transferenciaService.TransferenciaAsync(transferencia, ct);
            return Ok();
        }
    }
}