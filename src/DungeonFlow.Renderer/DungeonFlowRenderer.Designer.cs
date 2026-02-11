namespace DungeonFlow.Renderer
{
	partial class DungeonFlowRenderer
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
			DungeonPreview = new PictureBox();
			Generate = new Button();
			((System.ComponentModel.ISupportInitialize)DungeonPreview).BeginInit();
			SuspendLayout();
			// 
			// DungeonPreview
			// 
			DungeonPreview.Location = new Point(12, 12);
			DungeonPreview.Name = "DungeonPreview";
			DungeonPreview.Size = new Size(512, 512);
			DungeonPreview.TabIndex = 0;
			DungeonPreview.TabStop = false;
			// 
			// Generate
			// 
			Generate.Location = new Point(164, 541);
			Generate.Name = "Generate";
			Generate.Size = new Size(189, 46);
			Generate.TabIndex = 1;
			Generate.Text = "Generate";
			Generate.UseVisualStyleBackColor = true;
			Generate.Click += Generate_Click;
			// 
			// DungeonFlowRenderer
			// 
			AcceptButton = Generate;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(539, 599);
			Controls.Add(Generate);
			Controls.Add(DungeonPreview);
			Name = "DungeonFlowRenderer";
			Text = "DungeonFlow Renderer";
			((System.ComponentModel.ISupportInitialize)DungeonPreview).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private PictureBox DungeonPreview;
		private Button Generate;
	}
}
