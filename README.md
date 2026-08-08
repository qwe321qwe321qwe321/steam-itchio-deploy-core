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
repo as a git submodule at the root of the consuming project — it's shared code co-maintained
across both plugins, not vendored third-party code, so avoid burying it under a `ThirdParty/`-style
folder:

```
git submodule add https://github.com/qwe321qwe321qwe321/steam-itchio-deploy-core.git SteamItchIoDeployCore
```

Only `src/` is meant to be compiled by consumers — it carries its own Unity `.asmdef` so a Unity
project can call into it with no extra wiring. `.tests/` holds this repo's own xUnit test suite and
is deliberately named with a leading dot: both Unity's `AssetDatabase` and the default `.NET SDK`
project glob Godot's C# projects use ignore dot-prefixed directories, so it's automatically excluded
from whatever a consuming project compiles, even though the whole repo is checked out as one
submodule (no sparse-checkout or exclude rules required on the consumer's side).

### Editor asset metadata

`src/` also carries a `.gdignore` marker file, so Godot's filesystem scanner skips it entirely —
no `.uid` files get generated for these scripts, and they never show up as importable resources in
the FileSystem dock. `.gdignore` only affects Godot's *resource* importer; the Godot.NET.Sdk C#
build still compiles everything here normally, since that goes through MSBuild's own default item
glob, a completely separate mechanism. This repo's own `.meta` files (for Unity's `AssetDatabase`)
*are* committed, unlike the Godot `.uid` files — Unity has no `.gdignore` equivalent, since for
Unity, being importable as an asset (and therefore needing a `.meta` file with a stable GUID) is a
prerequisite for the C# compiler to see the code at all, not a separate opt-in step. If you add new
files here, open this repo once in a Unity Editor so it generates `.meta` files for them, then
commit those `.meta` files.

Every type here targets `netstandard2.1` and intentionally avoids APIs newer than that (no
`ProcessStartInfo.ArgumentList`, `OperatingSystem.IsWindows()`, `Convert.ToHexString`, etc.) so the
same source compiles under Unity's older API compatibility profiles as well as Godot's .NET 8
runtime.

## Development

```
dotnet build                                   # library only
dotnet test .tests/SteamItchIoDeployerCore.Tests.csproj
```

## License

MIT — see [LICENSE](LICENSE).
