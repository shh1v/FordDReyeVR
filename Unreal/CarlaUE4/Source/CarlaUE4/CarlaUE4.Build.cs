// Fill out your copyright notice in the Description page of Project Settings.

using System;
using System.IO;
using UnrealBuildTool;

public class CarlaUE4 : ModuleRules
{
    private bool IsWindows(ReadOnlyTargetRules Target)
    {
        return (Target.Platform == UnrealTargetPlatform.Win64) || (Target.Platform == UnrealTargetPlatform.Win32);
    }
    public CarlaUE4(ReadOnlyTargetRules Target) : base(Target)
    {
        PrivatePCHHeaderFile = "CarlaUE4.h";
        ShadowVariableWarningLevel = WarningLevel.Off; // -Wno-shadow

        // include LibCarla so we can #include <carla/x/y/z> headers
        string LibCarlaIncludePath =
            Path.GetFullPath(Path.Combine(ModuleDirectory, "..", "..", "..", "..", "LibCarla", "source"));
        PublicIncludePaths.Add(LibCarlaIncludePath);
        PrivateIncludePaths.Add(LibCarlaIncludePath);

        // Define the path to the Boost library
        string BoostPath = Path.Combine(ModuleDirectory, "D:/CarlaDReyeVR/carla/Build/boost-1.72.0-install");

        PublicIncludePaths.AddRange(
            new string[] {
                Path.Combine(BoostPath, "include") // Path to Boost include directory
            }
        );

        PublicAdditionalLibraries.AddRange(
            new string[] {
                Path.Combine(BoostPath, "lib", "libboost_date_time-vc142-mt-x64-1_72.lib"),
                Path.Combine(BoostPath, "lib", "libboost_filesystem-vc142-mt-x64-1_72.lib"),
                Path.Combine(BoostPath, "lib", "libboost_python37-vc142-mt-x64-1_72.lib"),
                Path.Combine(BoostPath, "lib", "libboost_system-vc142-mt-x64-1_72.lib")
            }
        );

        PublicDefinitions.Add("BOOST_ALL_NO_LIB"); // Prevent Boost from auto-linking other libraries

        PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "UMG", "MediaAssets", "ZeroMQ", "DataConfigCore", "Sockets", "Networking"});

        if (Target.Type == TargetType.Editor)
        {
            PublicDependencyModuleNames.AddRange(new string[] { "UnrealEd" });
        }
        else
        {
            // only build this carla exception in package mode
            PublicDefinitions.Add("NO_DREYEVR_EXCEPTIONS");
        }

        // Add module for SteamVR support with UE4
        PublicDependencyModuleNames.AddRange(new string[] { "HeadMountedDisplay" });

        if (IsWindows(Target))
        {
            bEnableExceptions = true; // enable unwind semantics for C++-style exceptions
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Edit these variables to enable/disable features of DReyeVR
        bool UseSRanipalPlugin = false;
        bool UseLogitechPlugin = true;
        bool UseFoveatedRender = false; // currently only supported in editor
        ////////////////////////////////////////////////////////////////////////////////////

        if (!IsWindows(Target))
        {
            // adjust definitions so they are OS-compatible
            UseSRanipalPlugin = false; // SRanipal only works on Windows
            UseLogitechPlugin = false; // LogitechWheelPlugin also only works on Windows
            UseFoveatedRender = false; // Vive VRS plugin requires engine fork
        }

        // Add these preprocessor definitions to code
        PublicDefinitions.Add("USE_SRANIPAL_PLUGIN=" + (UseSRanipalPlugin ? "true" : "false"));
        PublicDefinitions.Add("USE_LOGITECH_PLUGIN=" + (UseLogitechPlugin ? "true" : "false"));
        PublicDefinitions.Add("USE_FOVEATED_RENDER=" + (UseFoveatedRender ? "true" : "false"));

        // Add plugin dependencies
        if (UseSRanipalPlugin)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "SRanipal", "SRanipalEye" });
            PrivateIncludePathModuleNames.AddRange(new string[] { "SRanipal", "SRanipalEye" });
        }

        if (UseLogitechPlugin)
        {
            PrivateDependencyModuleNames.AddRange(new string[] { "LogitechWheelPlugin" });
        }

        if (UseFoveatedRender)
        {
            // Add VRS plugin and and EyeTracker dependencies
            PublicDependencyModuleNames.AddRange(new string[] { "EyeTracker", "VRSPlugin" });
        }

        PrivateDependencyModuleNames.AddRange(new string[] { "ImageWriteQueue" });
    }
}
