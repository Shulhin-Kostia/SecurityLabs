using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SecurityLab1.FilePropertiesTool;

namespace SecurityLab1
{
    public partial class MainWindow : Window
    {
        private bool _changed = false;
        private bool _newFile = true;
        private string _filePath;

        public MainWindow()
        {
            InitializeComponent();
            fileName.Text = fileName.Text.Remove(fileName.Text.Length - 1);
            _changed = false;
        }

        private void workSpace_TextChanged(object sender, EventArgs e)
        {
            if (_changed != true)
                fileName.Text += "*";
            _changed = true;
        }

        #region File
        private void newFile_Click(object sender, RoutedEventArgs e)
        {
            if (_changed)
            {
                var result = UnsavedChangesCheck();
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            workSpace.Document.Blocks.Clear();
            fileName.Text = "Untitled.txt";

            _changed = false;
            _newFile = true;
            _filePath = "";
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void saveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void renameFile_Click(object sender, RoutedEventArgs e)
        {
            if (_changed || _newFile)
                ShowErrorMessageBox("Can't rename new or unsaved file!\nPlease save file first.");
            else
            {
                var rename = new RenameWindow(_filePath);
                if (rename.ShowDialog() != true)
                {
                    return;
                }

                fileName.Text = rename.NewName;
                _filePath = rename.Path;
            }
        }

        private void printFile_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printFileDialog = new PrintDialog();
            if (printFileDialog.ShowDialog() != true)
            {
                return;
            }

            FlowDocument doc = workSpace.Document;
            IDocumentPaginatorSource idpSource = doc;
            printFileDialog.PrintDocument(idpSource.DocumentPaginator, fileName.Text);
        }

        private void fileProperties_Click(object sender, RoutedEventArgs e)
        {
            if (_changed || _newFile)
                ShowErrorMessageBox("Can't read properties of new or unsaved file!\nPlease save file first.");
            else
                FileProperties.Show(_filePath);
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Edit
        private void cutText_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Text = workSpace.Selection.Text;
            workSpace.Selection.Text = "";
        }

        private void copyText_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Text = workSpace.Selection.Text;
        }

        private void pasteText_Click(object sender, RoutedEventArgs e)
        {
            workSpace.CaretPosition.InsertTextInRun(Clipboard.Text);
        }

        private void deleteText_Click(object sender, RoutedEventArgs e)
        {
            workSpace.Selection.Text = "";
        }
        #endregion

        #region Help
        private void about_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }

        private void activate_Click(object sender, RoutedEventArgs e)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Activation.json");
            var activation = JsonSerializer.Deserialize<Activation>(File.ReadAllText(path));

            if (!activation.IsActivated)
            {
                new ActivationWindow().Show();
            }
            else
            {
                MessageBox.Show(
                     "Program already activated!",
                     "MyNotepad",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information);
            }
        }
        #endregion

        #region LocalMethods
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Activation.json");
            var activation = JsonSerializer.Deserialize<Activation>(File.ReadAllText(path));

            if (_changed)
            {
                UnsavedChangesCheck(ref e);
            }
            if (!e.Cancel && !activation.IsActivated)
            {
                ProposeActivation();
            }
        }

        private void Open()
        {
            if (_changed)
            {
                if (UnsavedChangesCheck() == MessageBoxResult.Cancel)
                    return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            _filePath = openFileDialog.FileName;
            var text = File.ReadAllText(_filePath);
            var rawFileName = openFileDialog.SafeFileName;
            workSpace.Document.Blocks.Clear();
            workSpace.Document.Blocks.Add(new Paragraph(new Run(text)));
            fileName.Text = rawFileName;
            _changed = false;
            _newFile = false;
        }

        private void Save()
        {
            if (_newFile)
            {
                SaveAs();
                return;
            }
            if (_changed)
            {
                var rawFileName = fileName.Text.Remove(fileName.Text.Length - 1);

                var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, rawFileName);
                File.WriteAllText(path, new TextRange(workSpace.Document.ContentStart, workSpace.Document.ContentEnd).Text);

                fileName.Text = rawFileName;
                _changed = false;
            }
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }
            _filePath = saveFileDialog.FileName;
            File.WriteAllText(_filePath, new TextRange(workSpace.Document.ContentStart, workSpace.Document.ContentEnd).Text);
            fileName.Text = saveFileDialog.SafeFileName;
            _changed = false;
            _newFile = false;
        }

        private MessageBoxResult UnsavedChangesCheck()
        {
            return ShowSaveWorkMessageBox();
        }

        private MessageBoxResult UnsavedChangesCheck(ref CancelEventArgs e)
        {
            var result = ShowSaveWorkMessageBox();

            if (result == MessageBoxResult.Yes)
                Save();

            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }

            return result;
        }

        private MessageBoxResult ShowSaveWorkMessageBox()
        {
            var result = MessageBox.Show(
               "Do you want to save your work?",
               "MyNotepad",
               MessageBoxButton.YesNoCancel,
               MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                Save();

            return result;
        }

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(
               message,
               "MyNotepad",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
        }

        private void ProposeActivation()
        {
            MessageBoxResult activate =
                      MessageBox.Show(
                      "You're using a trial version of the program.\nDo you want to activate it?",
                      "MyNotepad",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Question);

            if (activate == MessageBoxResult.Yes)
                new ActivationWindow().Show();
        }
    }
    #endregion
}
