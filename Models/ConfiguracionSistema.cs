namespace BecasExpertas.Models;
public class ConfiguracionSistema
{
    public decimal PromedioMinimoExcelencia { get; set; } = 9.50m;
    public int CreditosMinimosExcelencia { get; set; } = 24;
    public decimal PromedioMinimoGeneral { get; set; } = 7.00m;
    public decimal PromedioMinimoCondicional { get; set; } = 6.50m;
    public decimal PromedioMinimoExcepcionDiscapacidad { get; set; } = 6.00m;
    public int MaxMateriasReprobadas { get; set; } = 3;
    public string VersionCriterios { get; set; } = "2026.1";
}
