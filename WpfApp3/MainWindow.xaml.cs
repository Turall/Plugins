using MyPlugins;
using PluginOne;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Assembly> assemblies = new List<Assembly>();
        string[] str;
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo directory = new DirectoryInfo("Plugins");
            var files = directory.GetFiles();
            foreach (var item in files)
            {
                assemblies.Add(Assembly.LoadFile(item.FullName));
                PluginComBox.Items.Add(item.Name.Split('.')[0]);
            }
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            //FilmPlugin filmPlugin = new FilmPlugin();
            //var plot = await filmPlugin.Do(SearchTxtBox.Text);
            //ResultTxtBox.Text = plot;
            foreach (var item in assemblies[PluginComBox.SelectedIndex].GetTypes())
            {
                if (PluginComBox.SelectedItem != null)
                {
                    if (item.GetInterface("IPlugin") != null)
                    {
                        try
                        {
                            IPlugin info = Activator.CreateInstance(item) as IPlugin;
                            var str = await info.Do(SearchTxtBox.Text);
                            ResultTxtBox.Text = str;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            ResultTxtBox.Text = "Input Error";
                        }
                    }

                }
                else return;

            }
        }
    }
}
