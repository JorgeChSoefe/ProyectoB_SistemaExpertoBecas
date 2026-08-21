# Plan de pruebas

## Objetivo
Verificar decisiones, límites, explicaciones, manejo de conflictos y entradas adversarias. Cada ejecución de `Program.cs` imprime entrada, salida esperada, salida real y estado.

## Distribución
- 8 casos normales: N1 a N8.
- 4 casos borde: B1 a B4.
- 5 casos adversarios: A1 a A5.
- Total: 17 casos, por encima del mínimo de 15.

## Casos normales
| ID | Entrada principal | Salida esperada |
|---|---|---|
| N1 | Promedio 9,80; 120 créditos; 0 reprobadas; ingreso fuera de DMN | Beca de excelencia |
| N2 | Ingreso ajustado dentro del primer intervalo DMN | Socioeconómica categoría 1 |
| N3 | Ingreso ajustado en segundo intervalo | Socioeconómica categoría 2 |
| N4 | Ingreso ajustado en tercer intervalo | Socioeconómica categoría 3 |
| N5 | Ingreso ajustado en cuarto intervalo | Socioeconómica categoría 4 |
| N6 | Ingreso ajustado en quinto intervalo | Socioeconómica categoría 5 |
| N7 | Promedio 6,80 | Admisión condicional |
| N8 | Sin coincidencia de programa | Ninguno + brecha + acción |

## Casos borde
| ID | Límite evaluado | Salida esperada |
|---|---|---|
| B1 | Promedio exactamente 9,50 y 24 créditos | Beca de excelencia |
| B2 | Ingreso ajustado exactamente 100.000 | Categoría 1 |
| B3 | IDS exactamente 5 | Evaluación válida, categoría 2 |
| B4 | Materias reprobadas exactamente 3 | Evaluación válida, categoría 2 |

## Casos adversarios
| ID | Condición intencional | Salida esperada |
|---|---|---|
| A1 | Ingreso declarado igual a cero | Validación documental |
| A2 | Promedio igual a 15, fuera de rango | Rechazo de datos |
| A3 | Coinciden excelencia y socioeconómica | Solo excelencia por exclusividad |
| A4 | Promedio 6,20 y discapacidad | Excepción para admisión condicional |
| A5 | Ocho materias reprobadas | Ninguno + brecha + acción |

## Criterio de éxito
Un caso es correcto cuando la salida esperada aparece en la salida real. En A3, además, debe permanecer una única beca tras resolver el conflicto.
