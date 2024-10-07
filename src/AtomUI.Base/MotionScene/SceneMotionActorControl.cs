﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.MotionScene;

internal class SceneMotionActorControl : MotionActorControl
{
    public event EventHandler? SceneShowed;
    
    #region 内部属性定义

    /// <summary>
    /// 当 DispatchInSceneLayer 为 true 的时候，必须指定一个动画 SceneLayer 的父窗口，最好不要是 Popup
    /// </summary>
    public TopLevel? SceneParent { get; set; }

    #endregion
    
    protected Control? _ghost;
    
    protected virtual void BuildGhost()
    {
    }
    
    public Control GetAnimatableGhost()
    {
        return _ghost ?? this;
    }

    protected virtual Point CalculateTopLevelGhostPosition()
    {
        return default;
    }
    
    /// <summary>
    /// 在这个接口中，Actor 根据自己的需求对 sceneLayer 进行设置，主要就是位置和大小
    /// </summary>
    /// <param name="sceneLayer"></param>
    public virtual void NotifySceneLayerCreated(AbstractMotion motion, SceneLayer sceneLayer)
    {
        var ghost = GetAnimatableGhost();

        Size motionTargetSize;
        // Popup.Child can't be null here, it was set in ShowAtCore.
        if (ghost.DesiredSize == default)
        {
            // Popup may not have been shown yet. Measure content
            motionTargetSize = LayoutHelper.MeasureChild(ghost, Size.Infinity, new Thickness());
        }
        else
        {
            motionTargetSize = ghost.DesiredSize;
        }

        var sceneSize     = motion.CalculateSceneSize(motionTargetSize);
        var scenePosition = motion.CalculateScenePosition(motionTargetSize, CalculateTopLevelGhostPosition());
        sceneLayer.MoveAndResize(scenePosition, sceneSize);
    }
    
    /// <summary>
    /// 当动画目标控件被添加到动画场景中之后调用，这里需要根据 Motion 的种类设置初始位置和大小
    /// </summary>
    /// <param name="motionTarget"></param>
    public virtual void NotifyMotionTargetAddedToScene(Control motionTarget)
    {
        Canvas.SetLeft(motionTarget, 0);
        Canvas.SetTop(motionTarget, 0);
    }
    
    public virtual void NotifySceneShowed()
    {
        SceneShowed?.Invoke(this, EventArgs.Empty);
    }
}