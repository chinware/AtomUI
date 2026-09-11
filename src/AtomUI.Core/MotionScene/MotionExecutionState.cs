namespace AtomUI.MotionScene;

internal enum MotionExecutionState
{
    // No motion is scheduled or running for the owner.
    Idle,

    // The motion is scheduled but has not started.
    Pending,

    // The motion is actively running.
    Playing,

    // The motion completed and the owner is applying its final effect.
    Completing
}
