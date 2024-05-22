using System.Drawing.Design;
using System.Xml.Linq;

namespace FolderSizeViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public int CountDir(string dirName)
        {
            string[] children = new string[0];
            try
            {
                children = System.IO.Directory.GetDirectories(dirName);
            }
            catch { }

            int childrenCount = 0;
            foreach (var child in children)
            {
                int childCount = CountDir(child);

                childrenCount += childCount;
            }

            return childrenCount + 1;
        }
        int folderCount = 0;
        int foldersDone = 0;
        private void FolderTree_DragDrop(object sender, DragEventArgs e)
        {
            string[] entries = e.Data.GetData(DataFormats.FileDrop) as string[];
            string dirName = entries[0];
            if (System.IO.Directory.Exists(dirName))
            {
                this.FolderTree.Nodes.Clear();
                this.FolderTree.Enabled = false;

                System.Threading.Thread t = new Thread(new ThreadStart(
                    () =>
                    {
                        this.Invoke(() => this.Text = "Folder Size Viewer [counting directories...]");
                        folderCount = CountDir(dirName);
                        this.Invoke(() => this.Text = "Folder Size Viewer [getting sizes...]");
                        foldersDone = 0;
                        this.pBar.Invoke(() => pBar.Visible = true);
                        var folderData = DoDir(dirName, true);
                        this.FolderTree.Invoke(() => this.FolderTree.Nodes.Add(CreateLabel(folderData, folderData.Size, 0)));
                        this.FolderTree.Invoke(() => AddChildrenToNode(this.FolderTree.Nodes[0], folderData, folderData.Size));
                        this.pBar.Invoke(() => pBar.Visible = false);
                        this.FolderTree.Invoke(() => this.FolderTree.Enabled = true);
                        this.Invoke(() => this.Text = "Folder Size Viewer");
                    }
                ));
                t.Start();
            }
        }

        private static string CreateLabel(FolderData folderData, long totalSize, int maxNameLength)
        {
            double perc = System.Convert.ToDouble(folderData.Size) / System.Convert.ToDouble(totalSize);
            string pBar = ProgressBar(perc * 100, ProgressBarMethod.shade, 20);
            return $"{folderData.Name.PadRight(maxNameLength)} [{pBar}] ({(folderData.Size / 1000).ToString("#,##0")}kb - {folderData.NumberOfFiles.ToString("#,##0")} file{(folderData.NumberOfFiles == 0 ? "" : "s")})";
        }

        void AddChildrenToNode(TreeNode node, FolderData parent, long totalSize)
        {
            int maxNameLength = 0;
            foreach (var child in parent.Children)
            {
                if (child.Name.Length > maxNameLength)
                    maxNameLength = child.Name.Length;
            }

            foreach (var child in parent.Children)
            {
                var currentNode = node.Nodes.Add(CreateLabel(child, totalSize, maxNameLength));
                if (child.Children.Count > 0)
                {
                    AddChildrenToNode(currentNode, child, totalSize);
                }
            }
        }

        public enum ProgressBarMethod
        {
            portion,
            shade
        }
        public static string ProgressBar(double percentValue, ProgressBarMethod method, int width)
        {
            string returnString = "";

            int numChars = System.Convert.ToInt32(Math.Floor(System.Convert.ToDouble(width) * (percentValue / 100)));
            double blockValue = 100.0 / System.Convert.ToDouble(width);
            double remainder = percentValue - blockValue * numChars;
            string midChar = "";

            switch (method)
            {
                case ProgressBarMethod.portion:
                    if (remainder > 0.5 * blockValue)
                        midChar = "▌";
                    break;
                case ProgressBarMethod.shade:
                    if (remainder > 0.25 * blockValue)
                        midChar = "░";
                    if (remainder > 0.5 * blockValue)
                        midChar = "▒";
                    if (remainder > 0.75 * blockValue)
                        midChar = "▓";
                    break;
            }

            returnString = returnString.PadLeft(numChars, '█');
            if (returnString.Length < width)
                returnString += midChar;

            returnString = returnString.PadRight(width, ' ');

            return returnString;
        }

        FolderData DoDir(string dirName, bool top)
        {
            FolderData currentFolder = new FolderData();
            var d = new System.IO.DirectoryInfo(dirName);
            if (top)
                currentFolder.Name = dirName;
            else
                currentFolder.Name = d.Name;

            string[] children = new string[0];

            try
            {
                children = System.IO.Directory.GetDirectories(dirName);
            }
            catch
            {
                currentFolder.Name += " [INACCESSIBLE]";         
            
            }

            long childrenSize = 0;
            long childrenCount = 0;
            foreach (var child in children)
            {
                var childData = DoDir(child, false);
                childrenSize += childData.Size;
                childrenCount += childData.NumberOfFiles;
                currentFolder.Children.Add(childData);
            }
            currentFolder.Children = currentFolder.Children.OrderBy(x => x.Size).Reverse().ToList();
            currentFolder.Size = childrenSize;
            currentFolder.NumberOfFiles = childrenCount;

            string[] files = new string[0];
            try
            {
                files = System.IO.Directory.GetFiles(dirName);
            }
            catch
            { }

            foreach (var file in files)
            {
                var f = new System.IO.FileInfo(file);
                currentFolder.Size += f.Length;
                currentFolder.NumberOfFiles++;
            }
            pBar.Invoke(() => pBar.Value = System.Convert.ToInt32((System.Convert.ToDouble(foldersDone) / System.Convert.ToDouble(folderCount)) * 100));

            foldersDone++;
            return currentFolder;
        }

        private void FolderTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
    }
    public class FolderData
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public long NumberOfFiles { get; set; }
        public List<FolderData> Children { get; set; } = new List<FolderData>();
    }
}
