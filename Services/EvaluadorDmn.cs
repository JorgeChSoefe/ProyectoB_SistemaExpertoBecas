using System.Globalization;
using System.Xml.Linq;
using BecasExpertas.Models;
namespace BecasExpertas.Services;
public static class EvaluadorDmn
{
    public static decimal CalcularIngresoAjustado(Estudiante e)
    {
        int miembros = Math.Max(1, e.MiembrosNucleoFamiliar);
        decimal ingresoNeto = Math.Max(0, e.IngresoFamiliarMensualBruto - e.GastosMedicosExcepcionales);
        decimal factorVivienda = e.TipoVivienda switch
        {
            TipoVivienda.Precario => 0.70m, TipoVivienda.Prestada => 0.80m,
            TipoVivienda.Alquilada => 0.85m, TipoVivienda.PropiaHipoteca => 0.90m, _ => 1m
        };
        decimal factorIds = e.IDS switch { 1 => 0.80m, 2 => 0.90m, _ => 1m };
        decimal factorDiscapacidad = e.TieneDiscapacidad ? 0.85m : 1m;
        return Math.Round((ingresoNeto / miembros) * factorVivienda * factorIds * factorDiscapacidad, 2);
    }
    public static int? ObtenerCategoria(decimal ingresoAjustado)
    {
        string ruta = Path.Combine(AppContext.BaseDirectory, "Knowledge", "BecasSocioeconomicas.dmn");
        var doc = XDocument.Load(ruta);
        foreach (var regla in doc.Root!.Elements("rule"))
        {
            decimal min = decimal.Parse(regla.Attribute("min")!.Value, CultureInfo.InvariantCulture);
            decimal max = decimal.Parse(regla.Attribute("max")!.Value, CultureInfo.InvariantCulture);
            if (ingresoAjustado >= min && ingresoAjustado <= max)
                return int.Parse(regla.Attribute("categoria")!.Value, CultureInfo.InvariantCulture);
        }
        return null;
    }
}
