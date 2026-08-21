namespace BecasExpertas.Models;
public class Estudiante
{
    public string Nombre { get; set; } = "";
    public string Cedula { get; set; } = "";
    public decimal PromedioPonderado { get; set; }
    public int CreditosAprobados { get; set; }
    public decimal IngresoFamiliarMensualBruto { get; set; }
    public int MiembrosNucleoFamiliar { get; set; }
    public TipoVivienda TipoVivienda { get; set; }
    public int IDS { get; set; }
    public decimal GastosMedicosExcepcionales { get; set; }
    public bool TieneDiscapacidad { get; set; }
    public TipoColegio TipoColegioProcedencia { get; set; }
    public int MateriasReprobadas { get; set; }
}
