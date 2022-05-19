using System.IO;
using System.Windows;

namespace SecurityLab1
{
    public partial class RenameWindow : Window
    {
        private FileInfo _fi;
        public string Path { get; private set; }
        public string NewName { get; private set; }

        public RenameWindow(string filePath)
        {
            InitializeComponent();

            _fi = new FileInfo(filePath);
            newNameField.Text = _fi.Name.Remove(_fi.Name.LastIndexOf($"{_fi.Extension}"));
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string[] files = Directory.GetFiles(_fi.DirectoryName, "*.txt");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.Equals(newNameField.Text + _fi.Extension))
                {
                    MessageBox.Show(
                  "File with the same name already exists in this directory!\nPlease choose another name.",
                  "Rename",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);

                    newNameField.Text = _fi.Name.Remove(_fi.Name.LastIndexOf($"{_fi.Extension}"));
                    return;
                }

            }
            if (_fi.Exists)
            {
                
                Path = _fi.FullName.Replace(_fi.Name, newNameField.Text + _fi.Extension);
                _fi.MoveTo(Path);
                NewName = _fi.Name;

                DialogResult = true;
            }
            else
            {
                MessageBox.Show(
                      "Something went wrong!\nCan't find the file you're trying to rename.",
                      "Rename",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }

            Close();
        }

        private void NameExists(string name)
        {

        }

    }
}
