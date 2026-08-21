# Matriz de trazabilidad completa

| Requisito del sistema | Implementación | Casos que lo verifican | Evidencia producida |
|---|---|---|---|
| Validación de 10 variables y rangos | `ValidarDatosFueraDeRango` | A2 | Mensaje de validación |
| Ingreso cero como dato sospechoso | `ValidarIngresoCero` | A1 | Solicitud de verificación documental |
| Categorías socioeconómicas 1-5 | DMN + `BecaSocioeconomicaPorDmn` | N2-N6, B2-B4 | Programa y categoría |
| Beca de excelencia | `BecaExcelencia` | N1, B1, A3 | Decisión y auditoría |
| Admisión condicional | `AdmisionCondicional` | N7 | Decisión, brecha y acción |
| Excepción por discapacidad | `ExcepcionDiscapacidad` | A4 | Decisión y cláusula aplicada |
| Exclusividad entre becas | `ResolverExclusividadBecas` | A3 | Una sola beca final y auditoría |
| Resultado ninguno | `CrearDecisionNinguno` | N8, A5 | Incumplimiento, brecha y acción |
| Explicación para usuario | `ExplicacionUsuario` | Todos los casos decisorios | Texto sin jerga técnica |
| Explicación para auditoría | `ExplicacionAuditoria` | Todos los casos decisorios | Regla, datos y versión |
| 8 normales, 4 borde y 3+ adversarios | `Program.cs` y `PLAN_PRUEBAS.md` | N1-N8, B1-B4, A1-A5 | Esperado, real y estado |
| Segunda representación integrada | `BecasSocioeconomicas.dmn` + `EvaluadorDmn` | N2-N6, B2-B4 | Evaluación DMN FIRST |
| Reproducibilidad | README + Dockerfile | Ejecución completa | Comandos local y Docker |
