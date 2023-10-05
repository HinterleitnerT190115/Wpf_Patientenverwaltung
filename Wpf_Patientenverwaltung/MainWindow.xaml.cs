using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

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

            using (var sr = new StreamReader(ofd.FileName))
            {
                while (!sr.EndOfStream) {
                    var line = sr.ReadLine();

                    Patient p;
                    if (Patient.TryParse(line, out p))
                    {
                        patientList.Add(p);
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
                    sw.WriteLine(entry);
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
        }

        private void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            patientList.Remove((Patient)listViewPatients.SelectedItem);
        }
    }
}
