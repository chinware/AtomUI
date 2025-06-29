﻿using AtomUI.Animations;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

public enum MotionSpiritType
{
    Animation,
    Transition
}

public interface IMotion
{
    RelativePoint RenderTransformOrigin { get; }
    IList<Animation> Animations { get; }
    IList<INotifyTransitionCompleted> Transitions { get; }
    TimeSpan Duration { get; }
    Easing Easing { get; }
    FillMode PropertyValueFillMode { get; }
    MotionSpiritType SpiritType { get; set; }

    void Run(MotionActorControl actor,
             Action? aboutToStart = null,
             Action? completedAction = null);
}