# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Academic expert system (.NET 8 console app) that decides whether an applicant qualifies for an excellence scholarship, one of five socioeconomic scholarship categories, conditional admission, or none of the above. Every decision carries a user-facing explanation and an audit explanation (rule fired, thresholds compared, conflicts resolved). All domain text (rule messages, docs) is in Spanish — keep it that way when editing rules/messages.

## Commands

```powershell
dotnet run              # restore + build + run all 17 built-in test cases (Program.cs)
dotnet restore
dotnet clean
dotnet build
```

There is no separate test project — `Program.cs` itself defines 17 cases (8 normal, 4 edge, 5 adversarial) and asserts expected vs. actual decisions inline. A correct run ends with:

```
RESUMEN: 17/17 casos correctos
```

Docker:

```powershell
docker build -t proyecto-b-becas .
docker run --rm proyecto-b-becas
```

## Architecture

Two knowledge representations feed a single pipeline, orchestrated by NRules' RETE engine:

1. **NRules production rules** (`Rules/`, namespace `BecasExpertas.Rules`) — validation, excellence scholarship, conditional admission, disability exception, mutual-exclusivity resolution, and gap/suggested-action generation. Rules run by `[Priority(N)]` (higher fires first); this ordering encodes control flow, so when adding a rule pick its priority deliberately relative to existing ones:
   - 500 `ValidarDatosFueraDeRango`, 490 `ValidarIngresoCero`, 480 `ValidarMateriasReprobadas` — validation gates, each calls `ctx.Halt()` to short-circuit all lower-priority rules and produce a `ResultadoValidacion` instead of a `DecisionPrograma`.
   - 450 `ResolverExclusividadBecas` — if both `BecaExcelencia` and `BecaSocioeconomica` decisions exist, retracts the socioeconomic one and appends a note to excellence's explanations. This is the mutual-exclusivity policy: excellence always wins.
   - 350 `ExcepcionDiscapacidad`, 300 `BecaExcelencia`, 250 `BecaSocioeconomicaPorDmn`, 200 `AdmisionCondicional`.
2. **DMN-style decision table** (`Knowledge/BecasSocioeconomicas.dmn`, a simplified custom XML — not full OMG DMN) — maps an adjusted-income band to one of five socioeconomic categories under a `FIRST`-hit policy. Read and evaluated by `Services/EvaluadorDmn.cs` (`CalcularIngresoAjustado` applies household-size, housing-type, IDS, and disability adjustment factors before `ObtenerCategoria` looks up the band). The `.dmn` file is copied to the output directory on build (see `.csproj`) and read via `AppContext.BaseDirectory`, so it must ship alongside the executable/container.

Both representations write into the same NRules working memory as `DecisionPrograma` facts; `ResolverExclusividadBecas` and the case-runner in `Program.cs` reconcile whichever facts survive after `sesion.Fire()`.

Note the internal namespace is `BecasExpertas.*` even though the project/assembly is `ProyectoB.SistemaExpertoBecas` — don't "fix" this mismatch as it's the existing convention across all files.

`Models/ConfiguracionSistema.cs` centralizes all tunable thresholds (min. averages, max failed courses, criteria version string) — change thresholds there, not inline in rules, so the audit trail's `versión=` stamp stays meaningful.

## Known limitations (by design, per README §15-16)

DMN income bands and the disability exception threshold are demonstrative academic placeholders, not sourced from a real regulation. Don't treat them as authoritative when extending the system — if asked to make this "real," the thresholds, their regulatory source, and the traceability docs in `Docs/` all need updating together.
