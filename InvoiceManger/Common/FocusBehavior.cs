using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace InvoiceManger.Common
{
   public static class FocusBehavior//   控件获得焦点的类详见印象笔记
    {
        private static Dictionary<UIElement, RoutedEventHandler> handlers = new Dictionary<UIElement, RoutedEventHandler>();
        public static bool? GetIsFocused(DependencyObject obj)
        {
            return (bool?)obj.GetValue(IsFocusedProperty);
        }
        public static void SetIsFocused(DependencyObject obj, bool? value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }
        public static readonly DependencyProperty IsFocusedProperty =DependencyProperty.RegisterAttached("IsFocused",typeof(bool?),typeof(FocusBehavior),
                new UIPropertyMetadata()
                  {
                     DefaultValue = null,
                     PropertyChangedCallback =(s, e) => 
                     {
                            UIElement sender = (UIElement)s;
                            RoutedEventHandler x;
                            if (!handlers.TryGetValue(sender, out x))
                            {
                                Attach(sender);
                            }
                            if (e.NewValue != null)
                            {
                             if ((bool)e.NewValue)
                             {
                                 sender.Focus();
                                 Keyboard.Focus(sender);
                             }
                            }

                     }});
        private static void Attach(UIElement sender)
        {
            RoutedEventHandler handler = (s, e) => {
                UIElement ui = (UIElement)s;
                if (e.RoutedEvent == UIElement.GotFocusEvent)
                {
                    ui.SetValue(IsFocusedProperty, true);
                }
                if (e.RoutedEvent == UIElement.LostFocusEvent)
                {
                    ui.SetValue(IsFocusedProperty, false);
                }
            };
            sender.GotFocus += handler;
            sender.LostFocus += handler;
            handlers.Add(sender, handler);
        }
    }
}

