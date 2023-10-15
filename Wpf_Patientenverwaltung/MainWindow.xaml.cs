using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.XPath;

namespace Wpf_Patientenverwaltung
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Patient> patientList = new ObservableCollection<Patient>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewPatients.ItemsSource = patientList;
            btnAddDisease.IsEnabled = false;
            btnDeleteSelectedPatient.IsEnabled = false;

            //TEMP for Testing
            textBoxFirstname.Text = "Max";
            textBoxLastname.Text = "Muster";
            datePickerBirthday.SelectedDate = DateTime.Now;
            radioButtonMale.IsChecked = false;
            radioButtonFemale.IsChecked = true;
            checkBoxBedWetter.IsChecked = true;
            comboboxDiseases.SelectedIndex = 0;
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog() {
                Title = "Open patient list",
                CheckFileExists = true,
                Filter = "CSV-Files *.csv|*.csv|All Files *.*|*.*"
            };
            if(ofd.ShowDialog() != true) { return; }

            patientList.Clear();
            stackPanelPatients.Children.Clear();

            using (var sr = new StreamReader(ofd.FileName))
            {
                while (!sr.EndOfStream) {
                    var line = sr.ReadLine();

                    Patient p;
                    if (Patient.TryParse(line, out p))
                    {
                        patientList.Add(p);
                        AddItemToStackPanel(p);
                    }
                }
            }
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save patient list",
                CheckPathExists = true,
                OverwritePrompt = true,
                Filter = "CSV-Files *.csv|*.csv|All Files *.*|*.*"
            };
            if(sfd.ShowDialog() != true) { return; }

            using (var sw = new StreamWriter(sfd.FileName))
            {
                foreach (var entry in patientList)
                {
                    sw.WriteLine(entry.ToString(asCSV: true));
                }
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void miInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Simple patient managment app.\n\nMade by Thomas Hinterleitner", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            var firstname = textBoxFirstname.Text;
            var lastname = textBoxLastname.Text;
            var birthday = datePickerBirthday.SelectedDate;
            var isMale = radioButtonMale.IsChecked;
            var isFemale = radioButtonFemale.IsChecked;
            var isBedWetter = checkBoxBedWetter.IsChecked;

            string invalidMsg = "";
            if(String.IsNullOrEmpty(firstname))
            {
                invalidMsg += "Firstname must not be empty!\n";
            }
            if(String.IsNullOrEmpty(lastname))
            {
                invalidMsg += "Lastname must not be empty!\n";
            }
            if(birthday == null)
            {
                invalidMsg += "Birthday must not be empty!\n";
            }
            if(isMale == null || isFemale == null)
            {
                invalidMsg += "Gender must be selected!\n";
            }
            if(isBedWetter == null)
            {
                invalidMsg += "Bedwetter Checkbox must either be checked or not!\n";
            }

            if(invalidMsg != "")
            {
                MessageBox.Show(invalidMsg, "Error adding patient", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var p = new Patient()
            {
                Firstname = firstname,
                Lastname = lastname,
                Birthday = (DateTime)birthday,
                IsMale = (bool)isMale,
                IsBedWetter = (bool)isBedWetter,
            };



            patientList.Add(p);
            AddItemToStackPanel(p);
        }

        private void AddItemToStackPanel(Patient p)
        {
            string name = p.Firstname + " " + p.Lastname;
            stackPanelPatients.Children.Add(new Label()
            {
                Content = name,
                Background = System.Windows.Media.Brushes.Red,
                Width = 150,
                Height = 30,
                Margin = new Thickness(5)
            });
        }

        private void btnAddDisease_Click(object sender, RoutedEventArgs e)
        {
            string disease = (string)((System.Windows.Controls.ComboBoxItem)comboboxDiseases.SelectedValue).Content;
            patientList[listViewPatients.SelectedIndex].Diseases.Add(disease);

            //Otherwise diseases will not be updated in the ListView
            CollectionViewSource.GetDefaultView(listViewPatients.ItemsSource).Refresh();
        }

        private void btnDeleteSelectedPatient_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (Patient)listViewPatients.SelectedItem;

            patientList.Remove(selectedPatient);
            for (int i = 0; i < stackPanelPatients.Children.Count; i++)
            {
                Label label = (Label)stackPanelPatients.Children[i];

                string name = selectedPatient.Firstname + " " + selectedPatient.Lastname;

                if(label.Content.ToString().Equals(name))
                {
                    stackPanelPatients.Children.RemoveAt(i);
                    break;
                }
            }
        }

        private void btnDeleteAllPatients_Click(object sender, RoutedEventArgs e)
        {
            patientList.Clear();
            stackPanelPatients.Children.Clear();
        }

        private void listViewPatients_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                btnAddDisease.IsEnabled = true;
                btnDeleteSelectedPatient.IsEnabled = true;
            } else
            {
                btnAddDisease.IsEnabled = false;
                btnDeleteSelectedPatient.IsEnabled = false;
            }
        }
    }
}
