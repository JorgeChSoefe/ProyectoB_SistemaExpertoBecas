using NRules.Fluent.Dsl;
using NRules.RuleModel;
using BecasExpertas.Models;

namespace BecasExpertas.Rules;

[Priority(500)]
public class ValidarDatosFueraDeRango : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;

        When()
            .Match(() => estudiante, e =>
                string.IsNullOrWhiteSpace(e.Nombre) ||
                string.IsNullOrWhiteSpace(e.Cedula) ||
                e.PromedioPonderado < 0 ||
                e.PromedioPonderado > 10 ||
                e.CreditosAprobados < 0 ||
                e.CreditosAprobados > 300 ||
                e.IngresoFamiliarMensualBruto < 0 ||
                e.IngresoFamiliarMensualBruto > 10_000_000 ||
                e.MiembrosNucleoFamiliar < 1 ||
                e.MiembrosNucleoFamiliar > 15 ||
                e.IDS < 1 ||
                e.IDS > 5 ||
                e.GastosMedicosExcepcionales < 0 ||
                e.GastosMedicosExcepcionales > 5_000_000 ||
                e.MateriasReprobadas < 0 ||
                e.MateriasReprobadas > 20);

        Then()
            .Do(ctx => Ejecutar(ctx));
    }

    private static void Ejecutar(IContext ctx)
    {
        ctx.Insert(new ResultadoValidacion
        {
            EsValido = false,
            Mensaje = "Datos vacíos o fuera de rango."
        });
        ctx.Halt();
    }
}

[Priority(490)]
public class ValidarIngresoCero : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;

        When()
            .Match(() => estudiante, e => e.IngresoFamiliarMensualBruto == 0);

        Then()
            .Do(ctx => Ejecutar(ctx));
    }

    private static void Ejecutar(IContext ctx)
    {
        ctx.Insert(new ResultadoValidacion
        {
            EsValido = false,
            Mensaje = "Ingreso familiar en cero: requiere verificación documental."
        });
        ctx.Halt();
    }
}

[Priority(480)]
public class ValidarMateriasReprobadas : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;
        ConfiguracionSistema configuracion = default!;

        When()
            .Match(() => estudiante)
            .Match(() => configuracion)
            .Having(() => estudiante.MateriasReprobadas > configuracion.MaxMateriasReprobadas);

        Then()
            .Do(ctx => Ejecutar(ctx, estudiante, configuracion));
    }

    private static void Ejecutar(
        IContext ctx,
        Estudiante estudiante,
        ConfiguracionSistema configuracion)
    {
        int brecha = estudiante.MateriasReprobadas - configuracion.MaxMateriasReprobadas;

        ctx.Insert(new DecisionPrograma
        {
            Programa = ProgramaElegible.Ninguno,
            Aprobado = false,
            ExplicacionUsuario = $"No califica: supera en {brecha} el máximo permitido de materias reprobadas.",
            ExplicacionAuditoria = $"NRules ValidarMateriasReprobadas: materias={estudiante.MateriasReprobadas} > máximo={configuracion.MaxMateriasReprobadas}; versión={configuracion.VersionCriterios}.",
            RequisitosPendientes =
            {
                $"Aprobar al menos {brecha} materia(s)."
            },
            Brechas =
            {
                new Brecha
                {
                    Requisito = "Materias reprobadas",
                    ValorActual = estudiante.MateriasReprobadas.ToString(),
                    ValorRequerido = configuracion.MaxMateriasReprobadas.ToString(),
                    Diferencia = brecha.ToString(),
                    AccionSugerida = $"Aprobar al menos {brecha} materia(s)."
                }
            }
        });
        ctx.Halt();
    }
}
