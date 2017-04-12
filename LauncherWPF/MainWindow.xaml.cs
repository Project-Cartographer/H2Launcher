using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Dependencies
        private string register_url = "http://www.cartographer.h2pc.org/";
        private bool LoginPanelCheck, SettingsPanelCheck, UpdatePanelCheck;
        BitmapImage TitleLogo = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/h2logo.png"));

        //Disable ALT+Space shortcut for startmenu
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Space)
                e.Handled = true;
            else
                base.OnKeyDown(e);
        }
        //End Disable ALT+Space
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            LoginPanelCheck = false;
            SettingsPanelCheck = false;
            UpdatePanelCheck = false;
        }

        private void PanelAnimation(string Storyboard, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri("pack://application:,,,/Resources/Images/h2logo.png");
            b.EndInit();

            var image = sender as Image;
            image.Source = b;
        }

        private void main_form_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void control_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void control_minimize_Click(object sender, RoutedEventArgs e)
        {
            main_form.WindowState = WindowState.Minimized;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsPanelCheck && !UpdatePanelCheck)
            {
                if (LoginPanel.Margin.Top == -140)
                {
                    PanelAnimation("sbShowLoginMenu", LoginPanel);
                    LoginPanelCheck = true;
                }
                else if (LoginPanel.Margin.Top == 0)
                {
                    PanelAnimation("sbHideLoginMenu", LoginPanel);
                    LoginPanelCheck = false;
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(register_url);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LoginPanelCheck && !UpdatePanelCheck)
            {
                if (SettingPanel.Margin.Right == -220)
                {
                    PanelAnimation("sbShowSettingsMenu", SettingPanel);
                    SettingsPanelCheck = true;
                }
                else if (SettingPanel.Margin.Right == 0)
                {
                    PanelAnimation("sbHideSettingsMenu", SettingPanel);
                    SettingsPanelCheck = false;
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LoginPanelCheck && !SettingsPanelCheck)
            {
                if (UpdatePanel.Margin.Bottom == -250)
                {
                    PanelAnimation("sbShowUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = true;
                }
                else if (UpdatePanel.Margin.Bottom == 0)
                {
                    PanelAnimation("sbHideUpdateMenu", UpdatePanel);
                    UpdatePanelCheck = false;
                }
            }
        }
    }
}
