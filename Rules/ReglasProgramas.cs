using NRules.Fluent.Dsl;
using NRules.RuleModel;
using BecasExpertas.Models;
using BecasExpertas.Services;

namespace BecasExpertas.Rules;

[Priority(300)]
public class BecaExcelencia : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;
        ConfiguracionSistema configuracion = default!;

        When()
            .Match(() => estudiante)
            .Match(() => configuracion)
            .Having(() =>
                estudiante.PromedioPonderado >= configuracion.PromedioMinimoExcelencia &&
                estudiante.CreditosAprobados >= configuracion.CreditosMinimosExcelencia &&
                estudiante.MateriasReprobadas == 0);

        Then()
            .Do(ctx => Ejecutar(ctx, estudiante, configuracion));
    }

    private static void Ejecutar(
        IContext ctx,
        Estudiante estudiante,
        ConfiguracionSistema configuracion)
    {
        ctx.Insert(new DecisionPrograma
        {
            Programa = ProgramaElegible.BecaExcelencia,
            Aprobado = true,
            ExplicacionUsuario = "Califica para beca de excelencia por rendimiento académico.",
            ExplicacionAuditoria = $"NRules BecaExcelencia: promedio={estudiante.PromedioPonderado:F2} >= {configuracion.PromedioMinimoExcelencia:F2}; créditos={estudiante.CreditosAprobados} >= {configuracion.CreditosMinimosExcelencia}; reprobadas=0; versión={configuracion.VersionCriterios}.",
            RequisitosCumplidos =
            {
                "Promedio mínimo de excelencia.",
                "Créditos mínimos aprobados.",
                "Sin materias reprobadas."
            }
        });
    }
}

[Priority(250)]
public class BecaSocioeconomicaPorDmn : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;

        When()
            .Match(() => estudiante);

        Then()
            .Do(ctx => Ejecutar(ctx, estudiante));
    }

    private static void Ejecutar(IContext ctx, Estudiante estudiante)
    {
        decimal ingresoAjustado = EvaluadorDmn.CalcularIngresoAjustado(estudiante);
        int? categoria = EvaluadorDmn.ObtenerCategoria(ingresoAjustado);

        if (categoria is null)
            return;

        ctx.Insert(new DecisionPrograma
        {
            Programa = ProgramaElegible.BecaSocioeconomica,
            CategoriaSocioeconomica = categoria,
            Aprobado = true,
            ExplicacionUsuario = $"Califica para beca socioeconómica categoría {categoria}.",
            ExplicacionAuditoria = $"DMN FIRST: ingreso ajustado={ingresoAjustado:N2} CRC; categoría={categoria}; IDS={estudiante.IDS}; vivienda={estudiante.TipoVivienda}; discapacidad={estudiante.TieneDiscapacidad}.",
            RequisitosCumplidos =
            {
                $"Ingreso ajustado por persona: {ingresoAjustado:N2} CRC.",
                $"Categoría DMN: {categoria}."
            }
        });
    }
}

[Priority(200)]
public class AdmisionCondicional : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;
        ConfiguracionSistema configuracion = default!;

        When()
            .Match(() => estudiante)
            .Match(() => configuracion)
            .Having(() =>
                estudiante.PromedioPonderado >= configuracion.PromedioMinimoCondicional &&
                estudiante.PromedioPonderado < configuracion.PromedioMinimoGeneral &&
                estudiante.MateriasReprobadas <= configuracion.MaxMateriasReprobadas);

        Then()
            .Do(ctx => Ejecutar(ctx, estudiante, configuracion));
    }

    private static void Ejecutar(
        IContext ctx,
        Estudiante estudiante,
        ConfiguracionSistema configuracion)
    {
        decimal brecha = configuracion.PromedioMinimoGeneral - estudiante.PromedioPonderado;

        ctx.Insert(new DecisionPrograma
        {
            Programa = ProgramaElegible.AdmisionCondicional,
            Aprobado = true,
            ExplicacionUsuario = $"Califica para admisión condicional. Debe mejorar {brecha:F2} puntos.",
            ExplicacionAuditoria = $"NRules AdmisionCondicional: {configuracion.PromedioMinimoCondicional:F2} <= promedio={estudiante.PromedioPonderado:F2} < {configuracion.PromedioMinimoGeneral:F2}; versión={configuracion.VersionCriterios}.",
            RequisitosPendientes =
            {
                $"Elevar el promedio en {brecha:F2} puntos."
            },
            Brechas =
            {
                new Brecha
                {
                    Requisito = "Promedio general",
                    ValorActual = $"{estudiante.PromedioPonderado:F2}",
                    ValorRequerido = $"{configuracion.PromedioMinimoGeneral:F2}",
                    Diferencia = $"{brecha:F2}",
                    AccionSugerida = "Participar en tutorías y mejorar el promedio."
                }
            }
        });
    }
}
