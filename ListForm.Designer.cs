namespace MiLauncher
{
    partial class ListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listView = new System.Windows.Forms.ListView();
            Header = new System.Windows.Forms.ColumnHeader();
            SuspendLayout();
            // 
            // listView
            // 
            listView.AutoArrange = false;
            listView.BackColor = System.Drawing.SystemColors.Window;
            listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { Header });
            listView.Dock = System.Windows.Forms.DockStyle.Fill;
            listView.ForeColor = System.Drawing.SystemColors.WindowText;
            listView.FullRowSelect = true;
            listView.Location = new System.Drawing.Point(0, 0);
            listView.Margin = new System.Windows.Forms.Padding(4);
            listView.MultiSelect = false;
            listView.Name = "listView";
            listView.OwnerDraw = true;
            listView.Size = new System.Drawing.Size(381, 537);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            listView.VirtualMode = true;
            listView.DrawColumnHeader += listView_DrawColumnHeader;
            listView.DrawItem += listView_DrawItem;
            listView.RetrieveVirtualItem += listView_RetrieveVirtualItem;
            listView.KeyDown += listView_KeyDown;
            // 
            // Header
            // 
            Header.Text = "Header";
            // 
            // ListForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(381, 537);
            ControlBox = false;
            Controls.Add(listView);
            Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 128);
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ListForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader Header;
    }
}