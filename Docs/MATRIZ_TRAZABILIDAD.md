# Matriz de trazabilidad

| Requisito | Implementación | Pruebas |
|---|---|---|
| Categorías 1-5 | DMN `BecasSocioeconomicas.dmn` + regla integradora | N2-N6, B2-B4 |
| Beca excelencia | `BecaExcelencia` | N1, B1 |
| Admisión condicional | `AdmisionCondicional` | N7 |
| Ninguno y brechas | `CrearNinguno` | N8 |
| Rangos de entrada | `ValidarDatosFueraDeRango` | A2 |
| Ingreso cero | `ValidarIngresoCero` | A1 |
| Materias reprobadas | `ValidarMateriasReprobadas` | A3 |
| Dos niveles de explicación | `DecisionPrograma` | Todos |
