using APIFactura.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIFactura.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Controlador que maneja las operaciones relacionadas con las facturas
    public class FacturasController : ControllerBase
    {
        private readonly APIDbContext _context;

        // Constructor que recibe una instancia de DbContext
        public FacturasController(APIDbContext context)
        {
            _context = context;
        }

        // Acción para obtener todas las facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            // Retorna todas las facturas con sus productos cargados
            return await _context.Facturas
                .Include(f => f.Productos)
                .ToListAsync();
        }

        // Acción para obtener una factura por su Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            // Retorna una factura específica con sus productos cargados
            var factura = await _context.Facturas
                .Include(f => f.Productos)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
            {
                // Retorna un error 404 si la factura no existe
                return NotFound();
            }

            return factura;
        }

        // Acción para crear una nueva factura
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            // Verifica si el Id de la factura ya existe
            if (_context.Facturas.Any(f => f.Id == factura.Id))
            {
                // Retorna un error indicando que la factura ya existe
                return BadRequest("El Id de la factura ya existe.");
            }
            // Validar que la cantidad y el precioUnitario no sean iguales a cero para cada producto
            if (factura.Productos.Any(p => p.Cantidad == 0 || p.PrecioUnitario == 0))
            {
                // Retorna un error 400 indicando que la cantidad o el precioUnitario no son válidos
                return BadRequest("La cantidad y el precio unitario deben ser mayores a cero para todos los productos.");
            }
            // Agrega la nueva factura al contexto
            _context.Facturas.Add(factura);

            // Asocia productos a la factura antes de guardarla
            foreach (var producto in factura.Productos)
            {
                producto.Id = factura.Id;
                _context.Productos.Add(producto);
            }

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Recalcula el monto total después de guardar la factura
            factura = await _context.Facturas
                .Include(f => f.Productos)
                .FirstOrDefaultAsync(f => f.Id == factura.Id);

            // Retorna la nueva factura creada
            return CreatedAtAction("GetFactura", new { id = factura.Id }, factura);
        }

        #region PUT
        //Ingresa un nuevo  producto a una factura existente
        [HttpPut("{id:int}")]   
        public async Task<IActionResult> PutFactura(Factura factura, int id)
        {
            //si la factura es diferente retorna un mensaje de error.
            if (factura.Id != id)
                return BadRequest("Factura no encontrada");

            // identifica si existe un registro con esa Id (validacion)
            var exist = await _context.Facturas.AnyAsync(x => x.Id == id);
            if (!exist) return NotFound("No encontrado");

            _context.Facturas.Update(factura);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Modifica un producto existente
        [HttpPut("ActualizarProducto/{idFactura}/{idProducto}")]
        public async Task<IActionResult> PutProductoFactura(int idFactura, int idProducto, [FromBody] Producto producto)
        {
            // Verificar si la factura existe en la base de datos
            var facturaExiste = await _context.Facturas.AnyAsync(f => f.Id == idFactura);

            // Si la factura no existe, retornar un error 404
            if (!facturaExiste) return NotFound("Factura no encontrada");

            // Verificar si el producto existe en la base de datos
            var productoExiste = await _context.Productos.AnyAsync(p => p.Id == idProducto);

            // Si el producto no existe, retornar un error indicando que no es posible actualizar
            if (!productoExiste) return NotFound("Producto no encontrado. No es posible actualizar.");

            // Asignar el Id del producto proporcionado y actualizar sus propiedades
            producto.Id = idProducto;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }
        #endregion

        [HttpDelete("eliminar/{idfactura}/{idproducto}")]
        public async Task<IActionResult> DeleteFactura(int idfactura, int idproducto)
        {
            // Verificar si la factura existe en la base de datos
            var facturaExiste = await _context.Facturas.AnyAsync(f => f.Id == idfactura);

            // Si la factura no existe, retornar un error 404
            if (!facturaExiste) return NotFound("Factura no encontrada");

            // Verificar si el producto existe en la base de datos
            var productoExiste = await _context.Productos.AnyAsync(p => p.Id == idproducto);

            // Si el producto no existe, retornar un error indicando que no es posible eliminar
            if (!productoExiste) return NotFound("Producto no encontrado. No es posible eliminar.");

            // Crear una instancia de Producto con el Id proporcionado y eliminarlo de la base de datos
            var producto = new Producto() { Id = idproducto };
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            // Crear una instancia de Factura con el Id proporcionado y eliminarla de la base de datos
            var factura = new Factura() { Id = idfactura };
            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            // Retornar un Ok indicando que la eliminación fue exitosa
            return Ok();
        }

        // Método privado para verificar si una factura existe por su Id
        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }

    }
}
