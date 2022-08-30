﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;
using WizCommon;
using System.Data.SqlClient;

namespace WizIns
{
    public partial class Frm_Info : Form
    {
        WizWorkLib Lib = new WizWorkLib();

        public Frm_Info()
        {
            InitializeComponent();
        }

        private void Frm_tlpTest_Info_Load(object sender, EventArgs e)
        {
            SetScreen();
            NoticeSet();
        }
        //TableLayoutPanel 세팅
        private void SetScreen()
        {
            //WizardLogo이미지 Common 프로젝트에서 가져오기
            Assembly _assembly;
            Stream _imageStream;

            _assembly = Assembly.Load("WizMes_Com");
            _imageStream = _assembly.GetManifestResourceStream("Common.Resources.WizardLogo.png");

            var img = Bitmap.FromStream(_imageStream);
            //PictureBox컨트롤 셋팅
            PictureBox pbLogo = new PictureBox();
            pbLogo.Dock = DockStyle.Fill;
            pbLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pbLogo.Image = img;
            //패널 배치 및 조정
            tlp_Info.Controls.Add(pbLogo, 0, 3);
            tlp_Info.SetColumnSpan(lblName, 6);
            tlp_Info.SetColumnSpan(p_lbl_Notice, 6);
            tlp_Info.SetColumnSpan(p_txt_Notice, 6);
            tlp_Info.SetRowSpan(pbLogo, 3);
        }

        //공지사항 값 불러오기, 당일 날짜
        private void NoticeSet()
        {
            p_txt_Notice.Text = "오늘은 " + string.Format(@"{0:yyyy년 MM월 dd일}", DateTime.Now) + " 입니다 \r\n\r\n";
            SqlParameter[] param = {
                                            new SqlParameter("SCompanyID",""),
                                            new SqlParameter("SDATE", DateTime.Now.ToString("yyyyMMdd")),
                                            new SqlParameter("EDATE", DateTime.Now.ToString("yyyyMMdd"))
                                    };
            DataSet ds = DataStore.Instance.ExecuteDataSet("xp_Info_sInfoByDate", param, false);
            DataStore.Instance.CloseConnection();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                p_txt_Notice.Text = p_txt_Notice.Text + dr["Info"].ToString() + "\r\n\r\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Frm_tins_SetConfig fts = new Frm_tins_SetConfig();
            //fts.ShowDialog();
            Frm_PopUpSel fts = new Frm_PopUpSel();
            fts.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //frm_tins_Result_Q ftr = new frm_tins_Result_Q();
            
            frm_tins_BoxLabelPrint_U ftr = new frm_tins_BoxLabelPrint_U();
            ftr.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frm_tins_Order_Q ftr = new frm_tins_Order_Q();
            ftr.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WizCommon.Popup.Frm_tins_LabelRegister ins_lblreg = new WizCommon.Popup.Frm_tins_LabelRegister();
            ins_lblreg.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frm_tins_Defect_U ftdu = new frm_tins_Defect_U();
            ftdu.Owner = this;
            ftdu.ShowDialog();
        }
    }
}
