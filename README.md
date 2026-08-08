# Steam / itch.io Deploy Core

Engine-agnostic C# building blocks shared by
[unity-steam-itchio-deployer](https://github.com/qwe321qwe321qwe321/unity-steam-itchio-deployer) and
[godot-steam-itchio-deployer](https://github.com/qwe321qwe321qwe321/godot-steam-itchio-deployer).

Both plugins build once and upload to Steam (via `steamcmd`) and/or itch.io (via `butler`). The
two engines independently re-implemented the same command-building, VDF-generation, and
output-classification logic; this repo pulls that overlap out into one place so a fix (e.g. a new
Steam Guard prompt phrasing) only needs to be made once.

## What lives here

Pure, stateless C# with no `UnityEngine` / `Godot` dependency:

- `SteamCommandBuilder` / `ButlerCommandBuilder` — build `steamcmd`/`butler` argument token arrays.
- `VdfContentBuilder` — renders steamcmd's `app_build`/`depot_build` VDF script text.
- `CliOutputClassifier` — regex-based classification of CLI output lines (Steam Guard prompts,
  auth failures, generic errors).
- `CliArgumentQuoting` — joins an argument token array into a single command-line string, for
  hosts limited to `ProcessStartInfo.Arguments` (a single string) instead of `ArgumentList`.
- `MacroResolver` — substitutes `{Version}` / `{Date}` / `{DateTime}` / `{GitSHA}` in build
  descriptions and user-version strings.
- `GitShaResolver` — resolves `{GitSHA}` via `git rev-parse HEAD`.
- `SteamExitCodeDescriptions` — human-readable explanations of `steamcmd` exit codes.
- `MachineKeyDerivation` — SHA-256/MD5 hashing helpers for deriving machine-bound credential
  encryption key material.
- `DeployTargets` — the `[Flags]` enum shared by both hosts' settings models.

## What deliberately stays out

Process orchestration, UI, and settings storage are **not** shared, because they aren't actually
the same shape in each engine:

- Unity's editor main loop can't `await`, so its CLI runner polls a queue from `EditorWindow.Update`;
  Godot's editor plugin supports real `async`/`await Task`. Forcing one model onto the other engine
  would make it fight its own editor's threading model.
- Settings/credential storage backends differ (`ScriptableObject` + `EditorPrefs` vs. `Resource` +
  an encrypted `ConfigFile`), as do the UI toolkits (`EditorWindow` vs. `EditorPlugin`/`Control`
  tree) and build triggers (`BuildPipeline` vs. the `--export-release` CLI).

## Consuming this from a Unity or Godot project

Both consumer repos compile plain `.cs` files directly (no NuGet package is published); add this
repo as a git submodule and point your `.asmdef`/`.csproj` at the `src/` folder, e.g.:

```
git submodule add https://github.com/qwe321qwe321qwe321/steam-itchio-deploy-core.git ThirdParty/SteamItchIoDeployCore
```

Every type here targets `netstandard2.1` and intentionally avoids APIs newer than that (no
`ProcessStartInfo.ArgumentList`, `OperatingSystem.IsWindows()`, `Convert.ToHexString`, etc.) so the
same source compiles under Unity's older API compatibility profiles as well as Godot's .NET 8
runtime.

## Development

```
dotnet build                                   # library only
dotnet test tests/SteamItchIoDeployerCore.Tests.csproj
```

## License

MIT — see [LICENSE](LICENSE).
