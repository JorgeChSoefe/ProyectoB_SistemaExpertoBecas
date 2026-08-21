namespace BecasExpertas.Models;
public class Brecha
{
    public string Requisito { get; set; } = "";
    public string ValorActual { get; set; } = "";
    public string ValorRequerido { get; set; } = "";
    public string Diferencia { get; set; } = "";
    public string AccionSugerida { get; set; } = "";
}
public class DecisionPrograma
{
    public ProgramaElegible Programa { get; set; }
    public int? CategoriaSocioeconomica { get; set; }
    public bool Aprobado { get; set; }
    public string ExplicacionUsuario { get; set; } = "";
    public string ExplicacionAuditoria { get; set; } = "";
    public List<string> RequisitosCumplidos { get; set; } = new();
    public List<string> RequisitosPendientes { get; set; } = new();
    public List<Brecha> Brechas { get; set; } = new();
}
public class ResultadoValidacion
{
    public bool EsValido { get; set; }
    public string Mensaje { get; set; } = "";
}
public record CasoPrueba(string Id, TipoCasoPrueba Tipo, Estudiante Estudiante,
    List<string> Esperados);
