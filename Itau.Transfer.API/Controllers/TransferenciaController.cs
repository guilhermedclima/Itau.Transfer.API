using Itau.Transfer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaController(ITransferenciaService transferenciaService) : ControllerBase
    {
      
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RealizaTransferencia([FromBody] Transferencia transferencia, CancellationToken ct)
        {
            await transferenciaService.TransferenciaAsync(transferencia, ct);
            return Ok();
        }
    }
}