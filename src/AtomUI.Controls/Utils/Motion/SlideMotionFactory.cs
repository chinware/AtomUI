using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Utils;

public static partial class MotionFactory
{
   public static MotionConfig BuildSlideUpInMotion(TimeSpan duration, Easing? easing = null,
                                                   FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseOut();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 0.8
         };
         startFrame.Setters.Add(scaleYSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(scaleYSetter);
      }

      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideUpOutMotion(TimeSpan duration, Easing? easing = null,
                                                    FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseIn();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(scaleYSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 0.8
         };
         endFrame.Setters.Add(scaleYSetter);
      }

      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideDownInMotion(TimeSpan duration, Easing? easing = null,
                                                     FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseOut();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 0.8
         };
         startFrame.Setters.Add(scaleYSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(scaleYSetter);
      }

      transformOrigin = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideDownOutMotion(TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseIn();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(scaleYSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleYSetter = new Setter()
         {
            Property = ScaleTransform.ScaleYProperty,
            Value = 0.8
         };
         endFrame.Setters.Add(scaleYSetter);
      }

      transformOrigin = new RelativePoint(0.5, 1.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideLeftInMotion(TimeSpan duration, Easing? easing = null,
                                                     FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseOut();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 0.8
         };
         startFrame.Setters.Add(scaleXSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(scaleXSetter);
      }

      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideLeftOutMotion(TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseIn();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(scaleXSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 0.8
         };
         endFrame.Setters.Add(scaleXSetter);
      }

      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideRightInMotion(TimeSpan duration, Easing? easing = null,
                                                      FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseOut();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 0.8
         };
         startFrame.Setters.Add(scaleXSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 1.0
         };
         endFrame.Setters.Add(scaleXSetter);
      }

      transformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildSlideRightOutMotion(TimeSpan duration, Easing? easing = null,
                                                       FillMode fillMode = FillMode.None)
   {
      easing ??= new QuinticEaseIn();
      var animations = new List<IAnimation>();
      RelativePoint transformOrigin = default;
      var animation = new Animation()
      {
         Duration = duration,
         Easing = easing,
         FillMode = fillMode
      };

      var startFrame = new KeyFrame()
      {
         Cue = new Cue(0.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(opacitySetter);

         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 1.0
         };
         startFrame.Setters.Add(scaleXSetter);
      }
      animation.Children.Add(startFrame);

      var endFrame = new KeyFrame()
      {
         Cue = new Cue(1.0)
      };
      {
         var opacitySetter = new Setter()
         {
            Property = Visual.OpacityProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(opacitySetter);
         var scaleXSetter = new Setter()
         {
            Property = ScaleTransform.ScaleXProperty,
            Value = 0.8
         };
         endFrame.Setters.Add(scaleXSetter);
      }

      transformOrigin = new RelativePoint(1.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }
}