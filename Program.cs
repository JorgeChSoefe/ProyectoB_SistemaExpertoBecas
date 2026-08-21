using NRules;
using NRules.Fluent;
using BecasExpertas.Models;
using BecasExpertas.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("PROYECTO B - SISTEMA EXPERTO DE BECAS O ADMISIÓN");
Console.WriteLine(new string('=', 72));

var repositorio = new RuleRepository();
repositorio.Load(x => x.From(typeof(Program).Assembly));
var fabrica = repositorio.Compile();
var configuracion = new ConfiguracionSistema();

Estudiante Crear(string nombre, decimal promedio, int creditos, decimal ingreso, int familia,
    TipoVivienda vivienda, int ids, decimal gastosMedicos, bool discapacidad,
    TipoColegio colegio, int reprobadas) => new()
{
    Nombre = nombre,
    Cedula = Guid.NewGuid().ToString("N")[..12],
    PromedioPonderado = promedio,
    CreditosAprobados = creditos,
    IngresoFamiliarMensualBruto = ingreso,
    MiembrosNucleoFamiliar = familia,
    TipoVivienda = vivienda,
    IDS = ids,
    GastosMedicosExcepcionales = gastosMedicos,
    TieneDiscapacidad = discapacidad,
    TipoColegioProcedencia = colegio,
    MateriasReprobadas = reprobadas
};

var casos = new List<CasoPrueba>
{
    // 8 normales
    new("N1", TipoCasoPrueba.Normal, Crear("Excelencia sin conflicto",9.80m,120,2_500_000m,4,TipoVivienda.PropiaPagada,5,0,false,TipoColegio.Publico,0), new(){"BecaExcelencia"}),
    new("N2", TipoCasoPrueba.Normal, Crear("Socioeconómica 1",8.00m,60,300_000m,5,TipoVivienda.Precario,1,0,false,TipoColegio.Publico,1), new(){"BecaSocioeconomica:1"}),
    new("N3", TipoCasoPrueba.Normal, Crear("Socioeconómica 2",8.00m,60,600_000m,4,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Publico,1), new(){"BecaSocioeconomica:2"}),
    new("N4", TipoCasoPrueba.Normal, Crear("Socioeconómica 3",8.00m,60,800_000m,4,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Subvencionado,1), new(){"BecaSocioeconomica:3"}),
    new("N5", TipoCasoPrueba.Normal, Crear("Socioeconómica 4",8.00m,60,1_000_000m,4,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Privado,1), new(){"BecaSocioeconomica:4"}),
    new("N6", TipoCasoPrueba.Normal, Crear("Socioeconómica 5",8.00m,60,1_400_000m,4,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Privado,1), new(){"BecaSocioeconomica:5"}),
    new("N7", TipoCasoPrueba.Normal, Crear("Admisión condicional",6.80m,20,2_000_000m,4,TipoVivienda.PropiaPagada,5,0,false,TipoColegio.Privado,2), new(){"AdmisionCondicional"}),
    new("N8", TipoCasoPrueba.Normal, Crear("Ningún programa",8.00m,80,2_500_000m,4,TipoVivienda.PropiaPagada,5,0,false,TipoColegio.Privado,1), new(){"Ninguno"}),

    // 4 borde
    new("B1", TipoCasoPrueba.Borde, Crear("Promedio exacto excelencia",9.50m,24,2_500_000m,4,TipoVivienda.PropiaPagada,5,0,false,TipoColegio.Publico,0), new(){"BecaExcelencia"}),
    new("B2", TipoCasoPrueba.Borde, Crear("Ingreso exacto categoría 1",8.00m,40,500_000m,5,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Publico,1), new(){"BecaSocioeconomica:1"}),
    new("B3", TipoCasoPrueba.Borde, Crear("IDS máximo válido",8.00m,40,600_000m,4,TipoVivienda.PropiaPagada,5,0,false,TipoColegio.Publico,1), new(){"BecaSocioeconomica:2"}),
    new("B4", TipoCasoPrueba.Borde, Crear("Máximo reprobadas permitido",8.00m,40,600_000m,4,TipoVivienda.PropiaPagada,3,0,false,TipoColegio.Publico,3), new(){"BecaSocioeconomica:2"}),

    // 5 adversarios, superando el mínimo de 3
    new("A1", TipoCasoPrueba.Adversario, Crear("Ingreso cero",8.00m,40,0,4,TipoVivienda.Precario,1,0,false,TipoColegio.Publico,1), new(){"Validacion"}),
    new("A2", TipoCasoPrueba.Adversario, Crear("Promedio fuera de rango",15.00m,40,500_000m,4,TipoVivienda.Alquilada,2,0,false,TipoColegio.Publico,1), new(){"Validacion"}),
    new("A3", TipoCasoPrueba.Adversario, Crear("Conflicto de becas",9.80m,100,300_000m,5,TipoVivienda.Precario,1,0,false,TipoColegio.Publico,0), new(){"BecaExcelencia"}),
    new("A4", TipoCasoPrueba.Adversario, Crear("Excepción discapacidad",6.20m,20,2_000_000m,4,TipoVivienda.PropiaPagada,5,0,true,TipoColegio.Publico,2), new(){"AdmisionCondicional"}),
    new("A5", TipoCasoPrueba.Adversario, Crear("Materias excesivas",8.00m,40,500_000m,4,TipoVivienda.Alquilada,2,0,false,TipoColegio.Publico,8), new(){"Ninguno"})
};

int correctos = 0;
foreach (var caso in casos)
{
    var sesion = fabrica.CreateSession();
    sesion.Insert(configuracion);
    sesion.Insert(caso.Estudiante);
    sesion.Fire();

    var validacion = sesion.Query<ResultadoValidacion>().FirstOrDefault();
    var decisiones = sesion.Query<DecisionPrograma>().ToList();
    if (validacion is null && decisiones.Count == 0)
        decisiones.Add(CrearDecisionNinguno(caso.Estudiante, configuracion));

    var reales = validacion is not null
        ? new List<string> { "Validacion" }
        : decisiones.Select(Clave).ToList();

    bool correcto = caso.Esperados.All(reales.Contains) &&
                    (caso.Id != "A3" || reales.Count(x => x.StartsWith("Beca")) == 1);
    if (correcto) correctos++;

    Console.WriteLine($"\n[{caso.Id}] {caso.Tipo} - {caso.Estudiante.Nombre}");
    Console.WriteLine($"Entrada: promedio={caso.Estudiante.PromedioPonderado:F2}, créditos={caso.Estudiante.CreditosAprobados}, ingreso={caso.Estudiante.IngresoFamiliarMensualBruto:N0}, familia={caso.Estudiante.MiembrosNucleoFamiliar}, IDS={caso.Estudiante.IDS}, discapacidad={caso.Estudiante.TieneDiscapacidad}, reprobadas={caso.Estudiante.MateriasReprobadas}");
    Console.WriteLine($"Salida esperada: {string.Join(", ", caso.Esperados)}");
    Console.WriteLine($"Salida real: {string.Join(", ", reales)}");
    Console.WriteLine($"Estado: {(correcto ? "CORRECTO" : "REVISAR")}");

    if (validacion is not null) Console.WriteLine($"Usuario: {validacion.Mensaje}");
    foreach (var decision in decisiones)
    {
        Console.WriteLine($"Usuario: {decision.ExplicacionUsuario}");
        Console.WriteLine($"Auditoría: {decision.ExplicacionAuditoria}");
        foreach (var pendiente in decision.RequisitosPendientes) Console.WriteLine($"Acción: {pendiente}");
        foreach (var brecha in decision.Brechas) Console.WriteLine($"Brecha: {brecha.Requisito} | {brecha.Diferencia} | {brecha.AccionSugerida}");
    }
}
Console.WriteLine($"\nRESUMEN: {correctos}/{casos.Count} casos correctos");

static string Clave(DecisionPrograma d) => d.Programa == ProgramaElegible.BecaSocioeconomica
    ? $"BecaSocioeconomica:{d.CategoriaSocioeconomica}"
    : d.Programa.ToString();

static DecisionPrograma CrearDecisionNinguno(Estudiante e, ConfiguracionSistema c)
{
    decimal ingreso = EvaluadorDmn.CalcularIngresoAjustado(e);
    decimal exceso = Math.Max(0, ingreso - 400_000m);
    decimal brechaPromedio = Math.Max(0, c.PromedioMinimoExcelencia - e.PromedioPonderado);
    return new DecisionPrograma
    {
        Programa = ProgramaElegible.Ninguno,
        Aprobado = false,
        ExplicacionUsuario = "No califica para los programas evaluados. Consulte las acciones sugeridas.",
        ExplicacionAuditoria = $"Sin coincidencias NRules/DMN. Ingreso ajustado={ingreso:N2}; promedio={e.PromedioPonderado:F2}; versión={c.VersionCriterios}.",
        RequisitosPendientes =
        {
            exceso > 0
                ? $"Reducir la brecha de ingreso ajustado en {exceso:N2} CRC o actualizar la documentación."
                : $"Mejorar el promedio en {brechaPromedio:F2} puntos para optar por excelencia."
        },
        Brechas =
        {
            new Brecha
            {
                Requisito = exceso > 0 ? "Ingreso ajustado" : "Promedio",
                ValorActual = exceso > 0 ? $"{ingreso:N2}" : $"{e.PromedioPonderado:F2}",
                ValorRequerido = exceso > 0 ? "Máximo 400.000" : "9,50",
                Diferencia = exceso > 0 ? $"{exceso:N2}" : $"{brechaPromedio:F2}",
                AccionSugerida = "Revisar y actualizar el expediente en el siguiente periodo."
            }
        }
    };
}
