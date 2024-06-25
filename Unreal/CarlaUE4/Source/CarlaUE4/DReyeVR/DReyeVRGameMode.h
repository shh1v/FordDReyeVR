#pragma once

#include "Carla/Actor/DReyeVRCustomActor.h" // ADReyeVRCustomActor
#include "Carla/Game/CarlaGameModeBase.h"   // ACarlaGameModeBase
#include "Carla/Sensor/DReyeVRData.h"       // DReyeVR::
#include "DReyeVRPawn.h"                    // ADReyeVRPawn
#include "DReyeVRUtils.h"                   // SafePtrGet<T>
#include <unordered_map>                    // std::unordered_map

#include "DReyeVRGameMode.generated.h"

class AEgoVehicle;

UCLASS()
class ADReyeVRGameMode : public ACarlaGameModeBase
{
    GENERATED_UCLASS_BODY()

  public:
    ADReyeVRGameMode();

    virtual void BeginPlay() override;

    virtual void BeginDestroy() override;

    virtual void Tick(float DeltaSeconds) override;

    APawn *GetSpectator();
    AEgoVehicle *GetEgoVehicle();
    APlayerController *GetPlayer();
    ADReyeVRPawn *GetPawn();

    void SetEgoVehicle(AEgoVehicle *Ego);

    // input handling
    void SetupPlayerInputComponent();

    // EgoVehicle functions
    void PossessEgoVehicle();
    void PossessSpectator();
    void HandoffDriverToAI();

    // Replay media functions
    void ChangeTimestep(UWorld *World, double AmntChangeSeconds);
    void ReplayPlayPause();
    void ReplayFastForward();
    void ReplayRewind();
    void ReplayRestart();
    void ReplaySpeedUp();
    void ReplaySlowDown();

    // Replayer
    void SetupReplayer();

    // Meta world functions
    void SetVolume();
    FTransform GetSpawnPoint(int SpawnPointIndex = 0) const;

    // Custom actors
    void ReplayCustomActor(const DReyeVR::CustomActorData &RecorderData, const double Per);
    void DrawBBoxes();
    std::unordered_map<std::string, ADReyeVRCustomActor *> BBoxes;

  private:
    // for handling inputs and possessions
    void SetupDReyeVRPawn();
    void SetupSpectator();
    bool SetupEgoVehicle();
    void SpawnEgoVehicle(const FTransform &SpawnPt);

    // TWeakObjectPtr's allow us to check if the underlying object is alive
    // in case it was destroyed by someone other than us (ex. garbage collection)
    TWeakObjectPtr<class APlayerController> Player;
    TWeakObjectPtr<class ADReyeVRPawn> DReyeVR_Pawn;
    TWeakObjectPtr<class APawn> SpectatorPtr;
    TWeakObjectPtr<class AEgoVehicle> EgoVehiclePtr;

    // for toggling bw spectator mode
    bool bIsSpectating = true;

    // for audio control
    float EgoVolumePercent;
    float NonEgoVolumePercent;
    float AmbientVolumePercent;

    bool bDoSpawnEgoVehicleTransform = false; // whether or not to use provided SpawnEgoVehicleTransform
    FTransform SpawnEgoVehicleTransform;

    // for recorder/replayer params
    const double AmntPlaybackIncr = 0.25; // how much the playback speed changes (multiplicative, ex: 1x + 0.1 = 1.1x)
    double ReplayTimeFactor = 1.0;        // same as CarlaReplayer.h::TimeFactor (but local)
    double ReplayTimeFactorMin = 0.0;     // minimum playback of 0 (paused)
    double ReplayTimeFactorMax = 4.0;     // maximum of 4.0x playback
    bool bReplaySync = false;             // false allows for interpolation
    bool bUseCarlaSpectator = false;      // use the Carla spectator or spawn our own
    bool bRecorderInitiated = false;      // allows tick-wise checking for replayer/recorder
};