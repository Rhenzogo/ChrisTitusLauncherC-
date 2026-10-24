# WinUtil Launcher (ChrisTitusLauncherC-)

Lanzador de consola en C# / .NET 8 para Windows que ejecuta **WinUtil** de Chris Titus Tech con privilegios de Administrador.

Comando que ejecuta:

```
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "Invoke-RestMethod christitus.com/win | Invoke-Expression"
```

## Características

- Elevación mediante `ProcessStartInfo` (`Verb = "runas"`, `UseShellExecute = true`).
- **Rama experimental:** sin manifiesto, así que el UAC aparece al lanzar PowerShell (no al abrir el `.exe`) y la cancelación se puede capturar. La rama `main` sí usa manifiesto `requireAdministrator`.
- Manejo limpio de la cancelación de UAC (`Win32Exception` 1223) y de otros errores.
- Publicación como ejecutable único, autocontenido y portable (win-x64).

## Compilar

```powershell
dotnet build WinUtilLauncher.csproj -c Release
```

## Publicar (un solo .exe, x64, autocontenido)

```powershell
dotnet publish WinUtilLauncher.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=none -o ".\publish"
```

Salida: `publish\WinUtilLauncher.exe` (~33 MB).

## Aviso

`Invoke-RestMethod ... | Invoke-Expression` descarga y ejecuta código remoto. Es el método oficial de instalación de WinUtil, pero conviene revisar el origen (christitus.com) antes de ejecutarlo.
