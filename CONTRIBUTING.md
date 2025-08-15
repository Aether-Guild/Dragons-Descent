# Contributing to Dragons Descent

This document explains how to set up your development environment and build asset bundles for the Dragons Descent RimWorld mod.

## Requirements

### Software Requirements

- **.NET SDK** (6.0 or later) - Required for running the build script
- **dotnet-script** - Global tool for running C# scripts
- **Unity 2022.3.35f1** - The specific Unity version used for building asset bundles

### Installation

1. Install the .NET SDK from [Microsoft's website](https://dotnet.microsoft.com/download)

2. Install dotnet-script globally:
   ```bash
   dotnet tool install -g dotnet-script
   ```

3. The build script will automatically install the required AssetBundleBuilder tool when first run.

## Project Structure

```
Dragons-Descent/
├── Assets/                 # Unity assets to be bundled
│   ├── Materials/
│   ├── Prefabs/
│   ├── Textures/
│   └── ...
├── AssetBundles/          # Generated asset bundles (auto-created)
├── buildAssets.csx        # Asset bundle build script
└── CONTRIBUTING.md        # This file
```

## Building Asset Bundles

### Basic Usage

To build asset bundles from the `Assets` directory:

```bash
# From the project root directory
dotnet script buildAssets.csx
```

### Force Rebuild

To force a complete rebuild (ignoring incremental build cache):

```bash
dotnet script buildAssets.csx -- --force
```

### What the Build Script Does

1. **Checks for changes** - Uses file hashing to detect if assets have changed since last build
2. **Installs tools** - Automatically installs/updates the AssetBundleBuilder tool (v1.2.0)
3. **Builds bundle** - Creates an asset bundle from all files in the `Assets` directory
4. **Removes platform suffix** - Renames the output from `onyxae.dragonsdescent_<platform>` to just `onyxae.dragonsdescent`
5. **Updates cache** - Saves build hash for incremental builds

### Output

The build script generates:
- `AssetBundles/onyxae.dragonsdescent` - The main asset bundle file
- `AssetBundles/onyxae.dragonsdescent.manifest` - Unity manifest file
- `AssetBundles/.lastassetbuildhash` - Build cache file (do not commit)

## Development Workflow

1. **Add/modify assets** in the `Assets` directory
2. **Run build script** to generate updated asset bundles
3. **Test in RimWorld** to verify assets load correctly
4. **Commit changes** to both `Assets/` and `AssetBundles/` directories

## Troubleshooting

### "No Assets folder found"
- Ensure you have an `Assets` directory in the project root
- The `Assets` directory should contain your Unity assets

### "AssetBundleBuilder failed"
- Check that you have Unity 2022.3.35f1 installed
- Verify your assets are valid Unity assets
- Try running with `--force` to clean rebuild

### Build script won't run
- Ensure you have dotnet-script installed: `dotnet tool list -g`
- Check that you're running from the project root directory
- Verify .NET SDK is properly installed

## File Organization

### Assets Directory Structure
Organize your assets logically within the `Assets` directory:

```
Assets/
├── Materials/
│   ├── DragonScales.mat
│   └── FireEffect.mat
├── Prefabs/
│   ├── DragonBreath.prefab
│   └── IceProjectile.prefab
├── Textures/
│   ├── Dragons/
│   ├── Effects/
│   └── UI/
└── Animations/
    ├── DragonFlight/
    └── BreathWeapons/
```

### What to Commit
- ✅ `Assets/` directory and all contents
- ✅ `AssetBundles/` directory and generated bundles
- ✅ `buildAssets.csx` script
- ❌ `.lastassetbuildhash` files (build cache)

## Notes

- Asset bundles are built for the current platform only
- The build process is incremental - only rebuilds when assets change
- Unity 2022.3.35f1 is required for compatibility with RimWorld
- Generated asset bundles should be committed to the repository