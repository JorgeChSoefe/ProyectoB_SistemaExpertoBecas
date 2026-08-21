# Proyecto B: Sistema Experto de Becas o Admisión

## 1. ¿Qué hace este sistema?

Este proyecto implementa un sistema experto para evaluar información académica y socioeconómica de una persona solicitante.

El sistema determina si la persona califica para una beca de excelencia, una beca socioeconómica en alguna de sus cinco categorías, admisión condicional o ninguno de los programas evaluados.

Cada resultado incluye una explicación dirigida al usuario y una explicación de auditoría que identifica las reglas, condiciones y valores utilizados para tomar la decisión. Cuando la persona no cumple una condición, el sistema también puede indicar la brecha detectada y una acción sugerida.

---

## 2. Requisitos

Para ejecutar el proyecto localmente se necesita:

- .NET SDK 8.
- Acceso a NuGet durante la restauración inicial de dependencias.
- Docker, únicamente para la ejecución mediante contenedor.

### 2.1 Verificar la versión de .NET

Ejecute el siguiente comando:

```powershell
dotnet --version
```

El resultado debe indicar una versión correspondiente a .NET SDK 8.

### 2.2 Verificar Docker

Si desea ejecutar el proyecto mediante Docker, compruebe la instalación con:

```powershell
docker --version
```

Docker no es necesario para la ejecución local con .NET.

---

## 3. Ejecución local

Abra una terminal en la carpeta raíz del proyecto y ejecute:

```powershell
dotnet run
```

Este es el comando principal para restaurar las dependencias necesarias, compilar el proyecto y ejecutar los casos definidos en `Program.cs`.

Al finalizar correctamente, la consola debe mostrar:

```text
RESUMEN: 17/17 casos correctos
```

### 3.1 Comandos de diagnóstico

Si necesita ejecutar por separado la restauración, limpieza y compilación, utilice:

```powershell
dotnet restore
dotnet clean
dotnet build
dotnet run
```

Para una ejecución normal solamente se necesita:

```powershell
dotnet run
```

---

## 4. Ejecución con Docker

Desde la carpeta raíz del proyecto, construya la imagen:

```powershell
docker build -t proyecto-b-becas .
```

Luego ejecute el contenedor:

```powershell
docker run --rm proyecto-b-becas
```

La ejecución mediante Docker debe finalizar mostrando:

```text
RESUMEN: 17/17 casos correctos
```

---

## 5. Decisiones que produce el sistema

| Decisión | Descripción |
|---|---|
| `BecaExcelencia` | La persona cumple las condiciones académicas para una beca de excelencia. |
| `BecaSocioeconomica:1` | La persona califica para la categoría socioeconómica 1. |
| `BecaSocioeconomica:2` | La persona califica para la categoría socioeconómica 2. |
| `BecaSocioeconomica:3` | La persona califica para la categoría socioeconómica 3. |
| `BecaSocioeconomica:4` | La persona califica para la categoría socioeconómica 4. |
| `BecaSocioeconomica:5` | La persona califica para la categoría socioeconómica 5. |
| `AdmisionCondicional` | La persona cumple las condiciones para admisión condicional. |
| `Validacion` | La información requiere validación antes de emitir una decisión. |
| `Ninguno` | La persona no cumple las condiciones de los programas evaluados. |

---

## 6. Variables evaluadas

El sistema utiliza las siguientes variables de entrada:

| Variable | Descripción |
|---|---|
| Promedio | Promedio académico general de la persona solicitante. |
| Créditos | Cantidad de créditos académicos registrados. |
| Ingreso familiar | Ingreso total reportado por el grupo familiar. |
| Integrantes de la familia | Cantidad de personas que componen el grupo familiar. |
| IDS | Índice utilizado en la evaluación socioeconómica. |
| Discapacidad | Indica si corresponde aplicar la cláusula demostrativa de excepción. |
| Materias reprobadas | Cantidad de materias reprobadas por la persona solicitante. |
| Tipo de vivienda | Condición de vivienda utilizada en la evaluación socioeconómica. |

---

## 7. Representaciones del conocimiento

El sistema combina dos representaciones del conocimiento: reglas de producción mediante NRules y una tabla de decisión DMN.

### 7.1 Reglas de producción NRules

Las reglas de producción se utilizan para evaluar:

- Validaciones de los datos de entrada.
- Beca de excelencia.
- Admisión condicional.
- Excepción por discapacidad.
- Exclusividad entre beneficios.
- Cantidad máxima de materias reprobadas.
- Brechas y acciones sugeridas.

### 7.2 Tabla de decisión DMN

Las becas socioeconómicas se determinan mediante la siguiente tabla de decisión:

```text
Knowledge/BecasSocioeconomicas.dmn
```

La tabla utiliza la política `FIRST` para seleccionar una de las categorías socioeconómicas de la 1 a la 5.

La tabla DMN se integra en el sistema mediante el componente:

```text
EvaluadorDmn
```

---

## 8. Ejemplo de ejecución real

El siguiente ejemplo fue copiado de una ejecución real del proyecto.

### 8.1 Comando

```powershell
dotnet run
```

### 8.2 Entrada

```text
[N1] Normal - Excelencia sin conflicto

Entrada:
promedio=9,80
créditos=120
ingreso=2 500 000
familia=4
IDS=5
discapacidad=False
reprobadas=0
```

### 8.3 Salida real

```text
Salida esperada: BecaExcelencia
Salida real: BecaExcelencia
Estado: CORRECTO

Usuario:
Califica para beca de excelencia por rendimiento académico.

Auditoría:
NRules BecaExcelencia: promedio=9,80 >= 9,50;
créditos=120 >= 24;
reprobadas=0;
versión=2026.1.
```

La coincidencia entre la salida esperada y la salida real permite verificar que el proyecto se ejecutó correctamente.

---

## 9. Explicabilidad y auditoría

Las decisiones generadas por el sistema incluyen los siguientes componentes:

- `ExplicacionUsuario`.
- `ExplicacionAuditoria`.
- Brechas detectadas, cuando corresponda.
- Acciones sugeridas, cuando corresponda.

### 9.1 Explicación para el usuario

La propiedad `ExplicacionUsuario` presenta la decisión en un lenguaje comprensible.

Ejemplo:

```text
Califica para beca de excelencia por rendimiento académico.
```

### 9.2 Explicación de auditoría

La propiedad `ExplicacionAuditoria` permite identificar:

- La regla NRules aplicada.
- La decisión obtenida mediante la tabla DMN.
- Los datos que activaron la regla.
- Los umbrales comparados.
- La versión de las reglas.
- Los conflictos encontrados.
- Las decisiones retiradas por exclusividad.

Ejemplo:

```text
NRules BecaExcelencia:
promedio=9,80 >= 9,50;
créditos=120 >= 24;
reprobadas=0;
versión=2026.1.
```

---

## 10. Manejo de conflictos

Cuando una persona cumple simultáneamente las condiciones para una beca de excelencia y una beca socioeconómica, el sistema aplica una regla de exclusividad.

La beca de excelencia prevalece y la decisión socioeconómica se retira.

### Ejemplo real

```text
[A3] Adversario - Conflicto de becas

Entrada:
promedio=9,80
créditos=100
ingreso=300 000
familia=5
IDS=1
discapacidad=False
reprobadas=0

Salida esperada: BecaExcelencia
Salida real: BecaExcelencia
Estado: CORRECTO

Usuario:
Califica para beca de excelencia por rendimiento académico.
La beca de excelencia prevalece por la regla de exclusividad.

Auditoría:
NRules BecaExcelencia: promedio=9,80 >= 9,50;
créditos=100 >= 24;
reprobadas=0;
versión=2026.1.

Conflicto detectado: excelencia + socioeconómica.
Se retiró la decisión socioeconómica.
```

Este caso permite identificar tanto la regla que determinó la decisión final como la decisión que fue descartada.

---

## 11. Pruebas

Los casos de prueba se encuentran en `Program.cs` y se ejecutan mediante:

```powershell
dotnet run
```

La suite contiene 17 casos:

- 8 casos normales.
- 4 casos de borde.
- 5 casos adversarios.

Cada caso muestra:

- Identificador del caso.
- Tipo de caso.
- Datos de entrada.
- Salida esperada.
- Salida real.
- Estado de la comparación.
- Explicación para el usuario.
- Explicación de auditoría.
- Acción sugerida, cuando corresponde.
- Brecha identificada, cuando corresponde.

### 11.1 Casos normales

| Identificador | Caso |
|---|---|
| N1 | Excelencia sin conflicto |
| N2 | Beca socioeconómica categoría 1 |
| N3 | Beca socioeconómica categoría 2 |
| N4 | Beca socioeconómica categoría 3 |
| N5 | Beca socioeconómica categoría 4 |
| N6 | Beca socioeconómica categoría 5 |
| N7 | Admisión condicional |
| N8 | Ningún programa aplicable |

### 11.2 Casos de borde

| Identificador | Caso |
|---|---|
| B1 | Promedio exacto para excelencia |
| B2 | Ingreso exacto para categoría socioeconómica 1 |
| B3 | IDS máximo válido |
| B4 | Máximo de materias reprobadas permitido |

### 11.3 Casos adversarios

| Identificador | Caso |
|---|---|
| A1 | Ingreso familiar igual a cero |
| A2 | Promedio fuera del rango válido |
| A3 | Conflicto entre beca de excelencia y beca socioeconómica |
| A4 | Excepción por discapacidad |
| A5 | Cantidad excesiva de materias reprobadas |

### 11.4 Resultado verificado

La ejecución completa produjo:

```text
RESUMEN: 17/17 casos correctos
```

Los 17 casos presentaron coincidencia entre la salida esperada y la salida real.

---

## 12. Salvaguardas comprobadas

### 12.1 Ingreso familiar igual a cero

Cuando el ingreso familiar es igual a cero, el sistema no concede automáticamente una beca.

```text
Ingreso familiar en cero: requiere verificación documental.
```

### 12.2 Promedio fuera del rango

Cuando el promedio está fuera del rango aceptado, el sistema devuelve una decisión de validación.

```text
Datos vacíos o fuera de rango.
```

### 12.3 Conflicto entre becas

Cuando coinciden una beca de excelencia y una beca socioeconómica, el sistema aplica la regla de exclusividad y mantiene la beca de excelencia.

### 12.4 Excepción por discapacidad

El sistema contempla una cláusula demostrativa de excepción por discapacidad para la admisión condicional.

### 12.5 Materias reprobadas

Cuando la cantidad de materias reprobadas supera el máximo permitido, el sistema no concede el beneficio y calcula la brecha correspondiente.

```text
No califica: supera en 5 el máximo permitido de materias reprobadas.

Acción:
Aprobar al menos 5 materias.

Brecha:
Materias reprobadas | 5 | Aprobar al menos 5 materias.
```

---

## 13. Estructura del proyecto

La siguiente estructura presenta los principales archivos y carpetas conocidos del proyecto:

```text
ProyectoB_SistemaExpertoBecas/
├── Knowledge/
│   └── BecasSocioeconomicas.dmn
├── Docs/
│   ├── PLAN_PRUEBAS.md
│   ├── MATRIZ_TRAZABILIDAD_COMPLETA.md
│   ├── REFLEXION_CRITICA.md
│   └── COINCIDENCIA_RUBRICA.md
├── Program.cs
├── Dockerfile
└── README.md
```

> Antes de entregar, compruebe que este árbol coincida exactamente con la estructura real. Agregue las carpetas adicionales existentes sin superar dos niveles de profundidad.

---

## 14. Documentación complementaria

- `Docs/PLAN_PRUEBAS.md`: describe los casos normales, de borde y adversarios.
- `Docs/MATRIZ_TRAZABILIDAD_COMPLETA.md`: relaciona las decisiones, reglas, variables y casos de prueba.
- `Docs/REFLEXION_CRITICA.md`: presenta el análisis crítico, las decisiones de diseño y las limitaciones.
- `Docs/COINCIDENCIA_RUBRICA.md`: compara los componentes del proyecto con los criterios de evaluación.

---

## 15. Limitaciones conocidas

1. Los umbrales utilizados en la tabla DMN son supuestos demostrativos.
2. La excepción por discapacidad es un supuesto académico demostrativo.
3. El sistema no sustituye una resolución oficial de beca o admisión.
4. El sistema no verifica documentos académicos, económicos, familiares o médicos.
5. El sistema no consulta bases de datos institucionales.
6. El sistema evalúa únicamente las reglas y programas incluidos en la base de conocimiento.
7. Los resultados dependen de que la información de entrada sea correcta y esté completa.
8. Los criterios demostrativos deben sustituirse por las reglas del reglamento oficial seleccionado.
9. Cada regla debe documentar su fuente antes de utilizarse en un entorno real.
10. La versión documentada corresponde a una aplicación de consola y no expone endpoints HTTP.

---

## 16. Advertencia académica

Los umbrales de la tabla DMN y la excepción por discapacidad se utilizan con fines demostrativos.

Antes de considerar el sistema como una implementación de un reglamento real, el equipo debe:

- Sustituir los umbrales demostrativos por criterios oficiales.
- Identificar el reglamento utilizado.
- Documentar la fuente de cada regla.
- Justificar los criterios durante la defensa.
- Actualizar la matriz de trazabilidad.
- Actualizar las pruebas cuando cambien las reglas.

---

## 17. Verificación de reproducibilidad

Para comprobar la reproducibilidad:

1. Copie el proyecto a una carpeta diferente.
2. Elimine las carpetas generadas `bin` y `obj`.
3. Cierre Visual Studio u otro editor.
4. Abra una terminal nueva en la carpeta copiada.
5. Compruebe la versión de .NET:

```powershell
dotnet --version
```

6. Ejecute únicamente el comando documentado:

```powershell
dotnet run
```

7. Verifique que la ejecución finalice con:

```text
RESUMEN: 17/17 casos correctos
```

8. Si dispone de Docker, repita la prueba:

```powershell
docker build -t proyecto-b-becas .
docker run --rm proyecto-b-becas
```

9. Registre cualquier comando adicional, dependencia faltante o pregunta necesaria.

La prueba se considera satisfactoria cuando otra persona puede ejecutar el proyecto y obtener las decisiones sin solicitar instrucciones adicionales al grupo autor.
