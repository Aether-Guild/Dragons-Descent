// buildAssets.csx
// Run with: dotnet script ./buildAssets.csx [-- --force]

#nullable enable
#r "System.Runtime"
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;

// ---------- Parse arguments ----------
var scriptArgs = Args.ToArray();
var forceRebuild = scriptArgs.Contains("--force", StringComparer.OrdinalIgnoreCase);

static string GetFolderHash(string folderPath)
{
    var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories)
                         .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                         .ToArray();
    using var sha = SHA256.Create();
    var sb = new StringBuilder(Math.Max(1024, files.Length * 64));
    foreach (var file in files)
    {
        using var fs = File.OpenRead(file);
        sb.Append(Convert.ToHexString(sha.ComputeHash(fs)));
    }
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
}

static string FindProjectRoot()
{
    var cwd = Directory.GetCurrentDirectory();
    
    // Look for buildAssets.csx in current directory (project root)
    if (File.Exists(Path.Combine(cwd, "buildAssets.csx")))
        return cwd;
    
    // If not found, assume current directory is project root
    return cwd;
}

static bool RunCommand(string command, string arguments, string? workingDir = null)
{
    var psi = new ProcessStartInfo
    {
        FileName = command,
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        WorkingDirectory = workingDir ?? Directory.GetCurrentDirectory()
    };

    using var proc = Process.Start(psi);
    if (proc == null) return false;
    
    proc.OutputDataReceived += (_, e) => { if (e.Data is not null) Console.WriteLine("    " + e.Data); };
    proc.ErrorDataReceived  += (_, e) => { if (e.Data is not null) Console.Error.WriteLine("    " + e.Data); };
    proc.BeginOutputReadLine();
    proc.BeginErrorReadLine();
    proc.WaitForExit();
    
    return proc.ExitCode == 0;
}

// ---------- Paths ----------
var root = FindProjectRoot();

Console.WriteLine($"Project root: {root}");
Console.WriteLine($"Force rebuild: {forceRebuild}");

// ---------- Install/Update AssetBundleBuilder if needed ----------
const string RequiredToolVersion = "1.3.0";
Console.WriteLine($"Checking for AssetBundleBuilder tool (version {RequiredToolVersion})...");

var checkProc = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "tool list --global",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    CreateNoWindow = true
});
checkProc?.WaitForExit();
var toolOutput = checkProc?.StandardOutput.ReadToEnd() ?? "";

// Parse the tool output to check for the tool and its version
var hasCorrectVersion = false;
var hasWrongVersion = false;
var lines = toolOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
foreach (var line in lines)
{
    if (line.Contains("cryptiklemur.assetbundlebuilder", StringComparison.OrdinalIgnoreCase))
    {
        if (line.Contains(RequiredToolVersion))
        {
            hasCorrectVersion = true;
            Console.WriteLine($"  Found CryptikLemur.AssetBundleBuilder version {RequiredToolVersion}");
        }
        else
        {
            hasWrongVersion = true;
            Console.WriteLine($"  Found CryptikLemur.AssetBundleBuilder but wrong version: {line.Trim()}");
        }
        break;
    }
}

if (!hasCorrectVersion)
{
    if (hasWrongVersion)
    {
        Console.WriteLine($"Updating CryptikLemur.AssetBundleBuilder to version {RequiredToolVersion}...");
        RunCommand("dotnet", "tool uninstall --global CryptikLemur.AssetBundleBuilder");
    }
    else
    {
        Console.WriteLine($"Installing CryptikLemur.AssetBundleBuilder version {RequiredToolVersion}...");
    }
    
    if (!RunCommand("dotnet", $"tool install --global CryptikLemur.AssetBundleBuilder --version {RequiredToolVersion}"))
    {
        Console.WriteLine("Failed to install AssetBundleBuilder tool!");
        Environment.Exit(1);
    }
    Console.WriteLine("AssetBundleBuilder installed/updated successfully.");
}

// ---------- Build AssetBundle ----------
var srcAssets = Path.Combine(root, "Assets");
var bundlesDir = Path.Combine(root, "AssetBundles");
var hashFile = Path.Combine(bundlesDir, ".lastassetbuildhash");

if (!Directory.Exists(srcAssets))
{
    Console.WriteLine("No Assets folder found in project root.");
    Environment.Exit(1);
}

Console.WriteLine("--------------------------------------");
Console.WriteLine("Processing Assets folder...");

// Check hash for incremental builds (includes tool version)
var folderHash = GetFolderHash(srcAssets);
var currentHash = $"{folderHash}:{RequiredToolVersion}";
var previousHash = File.Exists(hashFile) ? (File.ReadAllText(hashFile) ?? "").Trim() : "";

if (!forceRebuild)
{
    Console.WriteLine($"    Current hash:  {currentHash}");
    Console.WriteLine($"    Previous hash: {previousHash}");
    
    if (string.Equals(currentHash, previousHash, StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("    No changes detected. Skipping build.");
        Environment.Exit(0);
    }
}
else
{
    Console.WriteLine("    Force rebuild enabled - cleaning existing bundles.");
    if (Directory.Exists(bundlesDir))
    {
        Directory.Delete(bundlesDir, true);
    }
}

Console.WriteLine("    Changes detected or force rebuild. Building asset bundle...");

// Build using AssetBundleBuilder
var bundleName = "onyxae.dragonsdescent";
Console.WriteLine($"    Building asset bundle: {bundleName}");

var buildArgs = $"2022.3.35f1 \"{srcAssets}\" {bundleName} \"{bundlesDir}\"";

if (!RunCommand("assetbundlebuilder", buildArgs, root))
{
    Console.WriteLine("    AssetBundleBuilder failed!");
    Environment.Exit(1);
}

// Save hash for next build
Directory.CreateDirectory(bundlesDir);
File.WriteAllText(hashFile, currentHash, Encoding.ASCII);
Console.WriteLine("    Done building asset bundle.");

Console.WriteLine("--------------------------------------");
Console.WriteLine("Asset bundle built successfully!");