﻿namespace WizIns
{
    partial class Frm_tins_Info
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
            this.tlp_Info = new System.Windows.Forms.TableLayoutPanel();
            this.p_lbl_Notice = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.p_txt_Notice = new System.Windows.Forms.TextBox();
            this.lblComTel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tlp_Info.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Info
            // 
            this.tlp_Info.ColumnCount = 6;
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.40179F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.59821F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 277F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 239F));
            this.tlp_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlp_Info.Controls.Add(this.p_lbl_Notice, 0, 1);
            this.tlp_Info.Controls.Add(this.lblName, 0, 0);
            this.tlp_Info.Controls.Add(this.p_txt_Notice, 0, 2);
            this.tlp_Info.Controls.Add(this.lblComTel, 2, 4);
            this.tlp_Info.Controls.Add(this.label3, 1, 4);
            this.tlp_Info.Controls.Add(this.label4, 1, 3);
            this.tlp_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Info.Location = new System.Drawing.Point(0, 0);
            this.tlp_Info.Name = "tlp_Info";
            this.tlp_Info.RowCount = 6;
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.25397F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.74603F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 344F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlp_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tlp_Info.Size = new System.Drawing.Size(996, 620);
            this.tlp_Info.TabIndex = 0;
            // 
            // p_lbl_Notice
            // 
            this.p_lbl_Notice.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.p_lbl_Notice.Dock = System.Windows.Forms.DockStyle.Left;
            this.p_lbl_Notice.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
            this.p_lbl_Notice.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.p_lbl_Notice.Location = new System.Drawing.Point(3, 115);
            this.p_lbl_Notice.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.p_lbl_Notice.Name = "p_lbl_Notice";
            this.p_lbl_Notice.Size = new System.Drawing.Size(170, 46);
            this.p_lbl_Notice.TabIndex = 13;
            this.p_lbl_Notice.Text = "공지사항";
            this.p_lbl_Notice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblName
            // 
            this.lblName.BackColor = System.Drawing.SystemColors.Control;
            this.lblName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Font = new System.Drawing.Font("맑은 고딕", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblName.Location = new System.Drawing.Point(3, 3);
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(174, 106);
            this.lblName.TabIndex = 15;
            this.lblName.Text = "검사/포장 시스템 - WizIns";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p_txt_Notice
            // 
            this.p_txt_Notice.BackColor = System.Drawing.SystemColors.Window;
            this.p_txt_Notice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_txt_Notice.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.p_txt_Notice.Location = new System.Drawing.Point(3, 166);
            this.p_txt_Notice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.p_txt_Notice.Multiline = true;
            this.p_txt_Notice.Name = "p_txt_Notice";
            this.p_txt_Notice.ReadOnly = true;
            this.p_txt_Notice.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.p_txt_Notice.Size = new System.Drawing.Size(171, 340);
            this.p_txt_Notice.TabIndex = 18;
            // 
            // lblComTel
            // 
            this.lblComTel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblComTel.Font = new System.Drawing.Font("맑은 고딕", 14.25F);
            this.lblComTel.Location = new System.Drawing.Point(441, 576);
            this.lblComTel.Name = "lblComTel";
            this.lblComTel.Size = new System.Drawing.Size(271, 35);
            this.lblComTel.TabIndex = 19;
            this.lblComTel.Text = "사무실 : 053-355-0935~6";
            this.lblComTel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 14.25F);
            this.label3.Location = new System.Drawing.Point(180, 576);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 35);
            this.label3.TabIndex = 20;
            this.label3.Text = "http://www.wizis.co.kr";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 16.25F);
            this.label4.Location = new System.Drawing.Point(180, 508);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(255, 68);
            this.label4.TabIndex = 21;
            this.label4.Text = "(주)위저드 정보시스템";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Frm_tins_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 620);
            this.Controls.Add(this.tlp_Info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Frm_tins_Info";
            this.Activated += new System.EventHandler(this.Frm_Info_Activated);
            this.Load += new System.EventHandler(this.Frm_tlpTest_Info_Load);
            this.tlp_Info.ResumeLayout(false);
            this.tlp_Info.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp_Info;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox p_txt_Notice;
        private System.Windows.Forms.Label p_lbl_Notice;
        private System.Windows.Forms.Label lblComTel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}