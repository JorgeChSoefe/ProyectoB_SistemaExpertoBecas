using NRules.Fluent.Dsl;
using NRules.RuleModel;
using BecasExpertas.Models;

namespace BecasExpertas.Rules;

/// <summary>
/// Excepción académica demostrativa para condición de discapacidad.
/// El umbral debe reemplazarse por la cláusula del reglamento oficial seleccionado.
/// </summary>
[Priority(350)]
public class ExcepcionDiscapacidad : Rule
{
    public override void Define()
    {
        Estudiante estudiante = default!;
        ConfiguracionSistema configuracion = default!;

        When()
            .Match(() => estudiante, e => e.TieneDiscapacidad)
            .Match(() => configuracion)
            .Having(() => estudiante.PromedioPonderado >= configuracion.PromedioMinimoExcepcionDiscapacidad
                       && estudiante.PromedioPonderado < configuracion.PromedioMinimoCondicional
                       && estudiante.MateriasReprobadas <= configuracion.MaxMateriasReprobadas);

        Then().Do(ctx => ctx.Insert(new DecisionPrograma
        {
            Programa = ProgramaElegible.AdmisionCondicional,
            Aprobado = true,
            ExplicacionUsuario = "Califica para admisión condicional mediante la cláusula de excepción por discapacidad.",
            ExplicacionAuditoria = $"NRules ExcepcionDiscapacidad: discapacidad=true; promedio={estudiante.PromedioPonderado:F2} >= mínimo_excepción={configuracion.PromedioMinimoExcepcionDiscapacidad:F2}; versión={configuracion.VersionCriterios}.",
            RequisitosCumplidos =
            {
                "Condición de discapacidad registrada.",
                $"Promedio igual o superior al mínimo excepcional {configuracion.PromedioMinimoExcepcionDiscapacidad:F2}."
            },
            RequisitosPendientes =
            {
                "Presentar respaldo documental y cumplir el plan de acompañamiento académico."
            }
        }));
    }
}

/// <summary>
/// Resuelve el conflicto cuando coinciden beca de excelencia y socioeconómica.
/// Para la demostración conserva excelencia y retira la socioeconómica.
/// </summary>
[Priority(450)]
public class ResolverExclusividadBecas : Rule
{
    public override void Define()
    {
        DecisionPrograma excelencia = default!;
        DecisionPrograma socioeconomica = default!;

        When()
            .Match(() => excelencia, d => d.Programa == ProgramaElegible.BecaExcelencia)
            .Match(() => socioeconomica, d => d.Programa == ProgramaElegible.BecaSocioeconomica);

        Then().Do(ctx => Ejecutar(ctx, excelencia, socioeconomica));
    }

    private static void Ejecutar(IContext ctx, DecisionPrograma excelencia, DecisionPrograma socioeconomica)
    {
        ctx.Retract(socioeconomica);
        excelencia.ExplicacionUsuario += " La beca de excelencia prevalece por la regla de exclusividad.";
        excelencia.ExplicacionAuditoria += " Conflicto detectado: excelencia + socioeconómica. Se retiró la decisión socioeconómica.";
        excelencia.RequisitosCumplidos.Add("Conflicto de programas resuelto mediante regla de exclusividad.");
        ctx.Update(excelencia);
    }
}
