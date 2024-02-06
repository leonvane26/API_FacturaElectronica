using APIFactura.Entidades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Producto
{
    // Oculta completamente el campo Id en todas las respuestas JSON
    [JsonIgnore]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}
