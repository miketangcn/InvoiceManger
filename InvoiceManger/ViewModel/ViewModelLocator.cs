/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:InvoiceManger"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;

namespace InvoiceManger.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<InputViewModel>();
            SimpleIoc.Default.Register<ConfigViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }

        }
        public InputViewModel Input
        {
            get
            {
                return ServiceLocator.Current.GetInstance<InputViewModel>();
            }

        }
        public ConfigViewModel ConfigViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfigViewModel>();
            }

        }

        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }

        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
        public static void InputClose()
        {
            var InputModel = ServiceLocator.Current.GetInstance<InputViewModel>();
            InputModel.Close();
        }
        public static void InputInital()
        {
            var InputModel = ServiceLocator.Current.GetInstance<InputViewModel>();
            //InputModel.Dispose();
            InputModel.Inital();
            // TODO Clear the ViewModels
        }

        internal static void ConfigInital()
        {
            var ConfigModel = ServiceLocator.Current.GetInstance<ConfigViewModel>();
            //InputModel.Dispose();
            ConfigModel.Inital();
        }
    }
}