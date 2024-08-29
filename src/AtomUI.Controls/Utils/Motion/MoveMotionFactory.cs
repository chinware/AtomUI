using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Utils;

public static partial class MotionFactory
{
   public static MotionConfig BuildMoveDownInMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = offset
         };
         startFrame.Setters.Add(translateYSetter);
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
         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(translateYSetter);
      }
      animation.Children.Add(endFrame);

      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildMoveDownOutMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(translateYSetter);
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
         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = offset
         };
         endFrame.Setters.Add(translateYSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildMoveUpInMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = -offset
         };
         startFrame.Setters.Add(translateYSetter);
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
         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(translateYSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildMoveUpOutMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = -offset
         };
         startFrame.Setters.Add(translateYSetter);
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
         var translateYSetter = new Setter()
         {
            Property = TranslateTransform.YProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(translateYSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildMoveLeftInMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = -offset
         };
         startFrame.Setters.Add(translateXSetter);
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
         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(translateXSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildLeftOutMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(translateXSetter);
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
         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = -offset
         };
         endFrame.Setters.Add(translateXSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildMoveRightInMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = offset
         };
         startFrame.Setters.Add(translateXSetter);
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
         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = 0.0
         };
         endFrame.Setters.Add(translateXSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }

   public static MotionConfig BuildRightOutMotion(double offset, TimeSpan duration, Easing? easing = null,
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

         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = 0.0
         };
         startFrame.Setters.Add(translateXSetter);
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
         var translateXSetter = new Setter()
         {
            Property = TranslateTransform.XProperty,
            Value = offset
         };
         endFrame.Setters.Add(translateXSetter);
      }
      animation.Children.Add(endFrame);
      transformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

      animations.Add(animation);
      return new MotionConfig(transformOrigin, animations);
   }
}