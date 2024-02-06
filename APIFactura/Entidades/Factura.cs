using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFactura.Entidades
{
    public class Factura
    {
        // Identificador único de la factura
        public int Id { get; set; }
        public string Emisor { get; set; }
        public string Receptor { get; set; }
        public DateTime FechaEmision { get; set; }
        // Lista de productos asociados a la factura
        public virtual List<Producto> Productos { get; set; }
        // Propiedad de solo lectura para el monto total de la factura
        [NotMapped]
        public double MontoTotal => Math.Round(CalcularMontoTotal(), 2);

        // Método privado para calcular el monto total de la factura
        private double CalcularMontoTotal()
        {
            return Productos?.Sum(p => p.PrecioUnitario * p.Cantidad) ?? 0;
        }
    }
}