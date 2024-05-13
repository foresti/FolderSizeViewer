namespace FolderSizeViewer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            FolderTree = new TreeView();
            pBar = new ProgressBar();
            SuspendLayout();
            // 
            // FolderTree
            // 
            FolderTree.AllowDrop = true;
            FolderTree.Dock = DockStyle.Fill;
            FolderTree.Location = new Point(0, 0);
            FolderTree.Name = "FolderTree";
            FolderTree.Size = new Size(1237, 695);
            FolderTree.TabIndex = 0;
            FolderTree.DragDrop += FolderTree_DragDrop;
            FolderTree.DragEnter += FolderTree_DragEnter;
            // 
            // pBar
            // 
            pBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pBar.Location = new Point(13, 617);
            pBar.Name = "pBar";
            pBar.Size = new Size(1211, 57);
            pBar.TabIndex = 1;
            pBar.Visible = false;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(18F, 40F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1237, 695);
            Controls.Add(pBar);
            Controls.Add(FolderTree);
            Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Folder Size Viewer";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private TreeView FolderTree;
        private ProgressBar pBar;
    }
}
