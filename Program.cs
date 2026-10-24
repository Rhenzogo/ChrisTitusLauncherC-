using System;
using System.ComponentModel;
using System.Diagnostics;

namespace WinUtilLauncher;

/// <summary>
/// Lanzador mínimo que ejecuta WinUtil (christitus.com/win) en una instancia elevada
/// de PowerShell, solicitando permisos de Administrador mediante el aviso de UAC.
/// </summary>
internal static class Program
{
    /// <summary>Código de error de Win32 que devuelve ShellExecute cuando el usuario cancela el UAC.</summary>
    private const int ErrorCancelled = 1223;

    /// <summary>Comando nativo de PowerShell a ejecutar (equivalente a: irm christitus.com/win | iex).</summary>
    private const string PowerShellCommand = "Invoke-RestMethod christitus.com/win | Invoke-Expression";

    private static int Main()
    {
        ConfigureConsole();

        Console.WriteLine("=== WinUtil Launcher ===");
        Console.WriteLine();
        Console.WriteLine("Se va a ejecutar:");
        Console.WriteLine("  powershell.exe -NoProfile -ExecutionPolicy Bypass -Command \"{0}\"", PowerShellCommand);
        Console.WriteLine();
        Console.WriteLine("Windows solicitara permisos de Administrador (UAC)...");
        Console.WriteLine();

        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{PowerShellCommand}\"",
            Verb = "runas",            // Fuerza la elevacion (aviso de UAC).
            UseShellExecute = true,    // Requerido por Verb = "runas".
            WindowStyle = ProcessWindowStyle.Normal
        };

        try
        {
            using var elevatedProcess = Process.Start(startInfo);

            if (elevatedProcess is null)
            {
                WriteError("No fue posible iniciar la instancia elevada de PowerShell.");
                Pause();
                return 1;
            }

            Console.WriteLine("Proceso elevado iniciado correctamente (PID: {0}).", elevatedProcess.Id);
            Console.WriteLine("WinUtil se esta ejecutando en su propia ventana de PowerShell.");
            return 0;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == ErrorCancelled)
        {
            // El usuario pulso "No" en el cartel de UAC.
            WriteError("Permisos rechazados: se cancelo el aviso de UAC.");
            Console.WriteLine("No se ha ejecutado ningun comando. Puede volver a intentarlo cuando lo desee.");
            Pause();
            return 2;
        }
        catch (Win32Exception ex)
        {
            WriteError($"Error de Windows al iniciar PowerShell (codigo {ex.NativeErrorCode}): {ex.Message}");
            Pause();
            return 3;
        }
        catch (Exception ex)
        {
            WriteError($"Error inesperado: {ex.GetType().Name} - {ex.Message}");
            Pause();
            return 4;
        }
    }

    private static void ConfigureConsole()
    {
        try
        {
            Console.Title = "WinUtil Launcher";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch
        {
            // La consola no admite cambios de titulo/codificacion: no es un error critico.
        }
    }

    private static void WriteError(string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = previousColor;
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Presione cualquier tecla para salir...");
        Console.ReadKey(intercept: true);
        Console.WriteLine();
    }
}
