﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WizCommon;
using System.IO;

namespace WizWork
{
    public partial class Frm_PopUp_ImgIns2 : Form
    {
        private DataSet ds = null;
        System.Windows.Forms.Button[] newButton = null;
        public string DefectYN = string.Empty;
        public string Value = string.Empty;
        public bool CheckedAll = false;

        int sLegendRow = 0; //DailMachineCheck 폼에서 가져온 범례 그리드 로우 행의 갯수
        public int sCurrentRow = 0;//DailMachineCheck 폼의 범례 그리드의 현재 행의 위치
        int a = 0; //가져온 행갯수 카운트용 변수

        public delegate void TextEventHandler(int a, string InspectionLegendFigure, Frm_PopUp_ImgIns2 Pop_II);
        public event TextEventHandler WriteTextEvent;          // 대리자 타입의 이벤트 처리기를 설정합니다. 
        string[] Message = new string[2];
        public string sNo = string.Empty;
        public string sCheckList = string.Empty;
        public string sInsContents = string.Empty;
        public string sMcInsCheck = string.Empty;
        public string sPath = string.Empty;
        public string sFile = string.Empty;

        public string KeypadStr = string.Empty;
        public bool blMod = false;
        bool blOK = false;

        string PK = string.Empty;

        PictureBox org = new PictureBox();
        bool ZoomFuction = true;

        public Frm_PopUp_ImgIns2()
        {
            InitializeComponent();
        }
        public Frm_PopUp_ImgIns2(int LegendRow, int CurrentRow, string No, string CheckList, string InsContents, string McInsCheck, string Path, string File)
        {
            InitializeComponent();
            sLegendRow = LegendRow;
            sCurrentRow = CurrentRow;
            //LoadData(5, 4);

            sNo = No;
            sCheckList = CheckList;
            sInsContents = InsContents;
            sMcInsCheck = McInsCheck;
            sPath = Path;
            sFile = File;
        }

        public Frm_PopUp_ImgIns2(int LegendRow, int CurrentRow, string No, string CheckList, string InsContents, string McInsCheck, string Path, string File, string PK)
        {
            InitializeComponent();
            sLegendRow = LegendRow;
            sCurrentRow = CurrentRow;
            //LoadData(5, 4);

            sNo = No;
            sCheckList = CheckList;
            sInsContents = InsContents;
            sMcInsCheck = McInsCheck;
            sPath = Path;
            sFile = File;

            this.PK = PK;
        }

        #region 그리드뷰 컬럼 셋팅
        private void InitGrid()
        {
            grdData1.Columns.Clear();
            grdData1.ColumnCount = 3;

            int i = 0;
            grdData1.Columns[i].Name = "No";
            grdData1.Columns[i].HeaderText = "No";
            grdData1.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            grdData1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grdData1.Columns[i].ReadOnly = true;
            grdData1.Columns[i].Visible = true;

            grdData1.Columns[++i].Name = "점검항목";
            grdData1.Columns[i].HeaderText = "점검항목";
            grdData1.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdData1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdData1.Columns[i].ReadOnly = true;
            grdData1.Columns[i].Visible = true;

            grdData1.Columns[++i].Name = "확인방법";
            grdData1.Columns[i].HeaderText = "확인방법";
            grdData1.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdData1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdData1.Columns[i].ReadOnly = true;
            grdData1.Columns[i].Visible = true;

            grdData1.Font = new Font("맑은 고딕", 15);
            grdData1.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 12);
            grdData1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdData1.RowTemplate.Height = 30;
            grdData1.ColumnHeadersHeight = 35;
            grdData1.ScrollBars = ScrollBars.Both;
            grdData1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdData1.MultiSelect = false;
            grdData1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(234, 234, 234);
            grdData1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdData1.ReadOnly = true;

            foreach (DataGridViewColumn col in grdData1.Columns)
            {
                col.DataPropertyName = col.Name;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void InitGrid2()
        {
            grdData2.Columns.Clear();
            grdData2.ColumnCount = 1;

            int i = 0;
            grdData2.Columns[i].Name = "점검내용";
            grdData2.Columns[i].HeaderText = "점검내용";
            grdData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            grdData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdData2.Columns[i].ReadOnly = true;
            grdData2.Columns[i].Visible = true;

            grdData2.Font = new Font("맑은 고딕", 15);
            grdData2.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 12);
            grdData2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grdData2.ColumnHeadersHeight = 35;
            grdData2.RowTemplate.Height = 30;
            grdData2.ScrollBars = ScrollBars.Both;
            grdData2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdData2.MultiSelect = false;
            grdData2.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(234, 234, 234);
            grdData2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdData2.ReadOnly = true;

            foreach (DataGridViewColumn col in grdData2.Columns)
            {
                col.DataPropertyName = col.Name;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        #endregion

        private void FillGrid(string No, string CheckList, string InsContents, string McInsCheck)
        {
            ClearData();
            grdData1.Rows.Add(No, CheckList, McInsCheck);
            grdData2.Rows.Add(InsContents);
        }
        private void ClearData()
        {
            lblImgName.Text = string.Empty;
            tbInputText.Text = string.Empty;
            picImg.Image = null;
            grdData1.Rows.Clear();
            grdData2.Rows.Clear();
        }

        void LoadData(int Horizontal, int Vertical)
        {
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add(Code_sCmCode.CODEGBN, "MCLEGEND");
            sqlParameter.Add(Code_sCmCode.SRELATION, "");

            ds = DataStore.Instance.ProcedureToDataSet("xp_Code_sCmCode", sqlParameter, false);

            if (ds.Tables[0].Rows.Count > 0)
            {
                this.newButton = new Button[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (i == Horizontal * Vertical - 1)
                    {
                        break;
                    }

                    Button newButton = new Button();
                    //tlpReason.Controls.Add(newButton, (i % Horizontal), (i / Horizontal));
                    DataRow dr = ds.Tables[0].Rows[i];

                    newButton.Text = dr[Code_sCmCode.CODE_NAME].ToString();
                    newButton.Tag = dr[Code_sCmCode.CODE_ID].ToString();//ReasonCode;
                    newButton.Dock = DockStyle.Fill;
                    //newButton.FlatStyle = FlatStyle.Popup;
                    newButton.Font = new Font("맑은 고딕", 14, FontStyle.Bold);
                    newButton.ForeColor = Color.Black;
                    newButton.Click += new System.EventHandler(this.SelectReasonBtn);
                    this.newButton[i] = newButton;
                }
            }
        }

        /// <summary>
        /// 키패드 키 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputKey(object sender, EventArgs e)
        {
            KeypadStr = tbInputText.Text;
            blOK = false;
            if (((Button)sender) == btnKeyClear) // 모두 지우기
            {
                KeypadStr = "";
            }
            else if (((Button)sender) == btnKeyBackSpace) // 지우기
            {
                KeypadStr = BackKeyProc(KeypadStr);
            }
            else if (((Button)sender) == btnKeyClose) // 취소
            {
                DialogResult = DialogResult.No;
                Close();
            }
            else if (((Button)sender) == btnKeyInput) // 확인
            {
                KeypadStr = tbInputText.Text;
                blOK = true;
                DialogResult = DialogResult.OK;
                SelectReasonBtn(sender, e);
                //Close();
                //return;
            }
            else if (((Button)sender) == btnKeyPoint)//.
            {
                if (!KeypadStr.Contains("."))//. 한번찍으면 더이상 못찍음
                {
                    KeypadStr += ((Button)sender).Text;
                }
            }
            else // 문자버튼
            {
                KeypadStr += ((Button)sender).Text;
            }
            if (!blOK)
            {
                tbInputText.Text = KeypadStr; // 입력된 내용 보여주기
            }
            string BackKeyProc(string text)
            {
                if (KeypadStr.Length <= 0) { return text; }

                text = KeypadStr.Substring(0, KeypadStr.Length - 1);

                return text;
            }
        }

        private void SelectReasonBtn(object sender, EventArgs e)
        {
            if (sLegendRow >= sCurrentRow)
            {
                WriteTextEvent(sCurrentRow, tbInputText.Text, this);
                if (blMod)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                sCurrentRow = sCurrentRow + 1;
                if (sLegendRow == sCurrentRow)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                if (sNo == "" && sCheckList == "" && sInsContents == "" && sMcInsCheck == "")
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                else
                {
                    FillGrid(sNo, sCheckList, sInsContents, sMcInsCheck);
                }

                if (sPath != "" || sFile != "")
                {
                    lblImgName.Text = sFile;
                    FtpDownload(sPath, sFile);
                }
            }
        }
        private void SetScreen()
        {
            //패널 배치 및 조정          

            pnlForm.Dock = DockStyle.Fill;
            foreach (Control control in pnlForm.Controls)
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(0, 0, 0, 0);
                foreach (Control contro in control.Controls)
                {
                    contro.Dock = DockStyle.Fill;
                    contro.Margin = new Padding(0, 0, 0, 0);
                    foreach (Control contr in contro.Controls)
                    {
                        contr.Dock = DockStyle.Fill;
                        contr.Margin = new Padding(0, 0, 0, 0);
                        foreach (Control cont in contr.Controls)
                        {
                            cont.Dock = DockStyle.Fill;
                            cont.Margin = new Padding(0, 0, 0, 0);
                            foreach (Control con in cont.Controls)
                            {
                                con.Dock = DockStyle.Fill;
                                con.Margin = new Padding(0, 0, 0, 0);
                                foreach (Control co in con.Controls)
                                {
                                    co.Dock = DockStyle.Fill;
                                    co.Margin = new Padding(0, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            tlpCal.Padding = new Padding(4);
            foreach (Control con in tlpCal.Controls)
            {
                if (con is Button)
                {
                    con.Margin = new Padding(2);
                }
                //btn.Margin = new Padding(5);
            }
            tlpCal.SetColumnSpan(tbInputText, 5);
            tlpCal.SetColumnSpan(btnKey0, 2);
        }

        private void Frm_PopUp_ImgIns_Load(object sender, EventArgs e)
        {
            InitGrid();
            InitGrid2();
            SetScreen();

            FillGrid(sNo, sCheckList, sInsContents, sMcInsCheck);

            if (sPath != "" || sFile != "")
            {
                lblImgName.Text = sFile;
                FtpDownload(sPath, sFile);
            }
        }

        private void Frm_PopUp_ImgIns_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Control pic in Controls)
            {
                if (pic is PictureBox)
                {
                    if (((PictureBox)(pic)).Image != null)
                    {
                        ((PictureBox)(pic)).Image = null;
                        ((PictureBox)(pic)).Image.Dispose();
                    }
                }
            }
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            PopUp popup = new PopUp();
            popup.Picture = picImg.Image; //OrderPopUp의 이미지를 PopUp의 PictureBox에 할당. 
            popup.Show();
        }

        private void FtpDownload(string Path, string File)
        {
            if (Path != "" && File != null)
            {
                //if (PK != string.Empty)
                //{
                //    Path += @"/" + PK;
                //}

                FTP_EX _ftp = null;
                INI_GS gs = new INI_GS();

                string FTP_ADDRESS = "ftp://" + gs.GetValue("FTPINFO", "FileSvr", "wizis.iptime.org") + ":" + gs.GetValue("FTPINFO", "FTPPort", "21");
                string FTP_ID = "wizuser";
                string FTP_PASS = "wiz9999";

                _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
                string LocalDirPath = Application.StartupPath + "\\" + "#Temp" + "\\" + Path + "\\"; //FTP서버내의 폴더명과 같은 폴더명을 LOCAL에서 사용하자;

                string FtpFolderPath = Path;//gs.GetValue("FTPINFO", "FTPIMAGEPATH", "/ImageData") + "/" + File; // ex)/ImageData/00065
                string[] fileListSimple;

                string Local_File = string.Empty;           //local 경로
                //picImg                                    //사진
                //lblImgName                                //text가 파일명 , tag 폴더명
                try
                {
                    fileListSimple = _ftp.directoryListSimple(FtpFolderPath, Encoding.UTF8);

                    //로컬경로 생성
                    DirectoryInfo dir = new DirectoryInfo(LocalDirPath);//로컬
                    if (dir.Exists == false)//로컬 폴더 존재 유무 확인 후 없을 시 생성
                    { dir.Create(); }
                    //로컬경로 생성

                    bool ftpExistFile = false;
                    picImg.Tag = Path + "/" + File;//파일 경로 + 파일명
                    Local_File = LocalDirPath + "\\" + File;//로컬경로

                    //파일 존재 유무 확인 있을때 ftpExistFile변수 True 없을때 False
                    foreach (string filename in fileListSimple)
                    {
                        if (string.Compare(filename.ToUpper(), File.ToUpper()) == 0)
                        { ftpExistFile = true; break; }
                    }

                    if (ftpExistFile == false)
                    {
                        Message[0] = "[FTP] " + File + " 이미지가 존재하지 않습니다.";
                        Message[1] = "[파일 존재하지 않음]";
                        throw new Exception();
                    }

                    else if (_ftp.GetFileSize(picImg.Tag.ToString()) == 0)//파일사이즈가 0일때
                    {
                        Message[0] = "[FTP] " + File + "이미지 파일 사이즈가 0입니다. 사무실프로그램에서 파일을 다시 업로드 해주시기 바랍니다.";
                        Message[1] = "[파일 크기 오류]";
                        throw new Exception();
                    }

                    else//파일 사이즈가 0이 아닐때 기존폴더안의 파일들 삭제 후 다운로드
                    {
                        //FTP 다운로드 부분
                        FileInfo file = new FileInfo(Local_File);
                        if (file.Exists == true)//로컬 품명코드 폴더안의 파일 삭제
                        { file.Delete(); }
                        if (_ftp.download(picImg.Tag.ToString(), Local_File.ToString()))
                        {
                            FileStream fs = new FileStream(Local_File.ToString(), FileMode.Open, FileAccess.Read);
                            picImg.Image = System.Drawing.Image.FromStream(fs);
                            fs.Close();
                            //picImg.SizeMode = PictureBoxSizeMode.StretchImage;

                            //picImg.SizeMode = PictureBoxSizeMode.Zoom;

                            // 확대 축소 기능 사용 여부에 따른 사진 세팅 2021-07-26
                            if (ZoomFuction == true)
                            {
                                //// 확대 축소 기능 사용 1.0
                                //picImg.SizeMode = PictureBoxSizeMode.AutoSize;
                                //picImg.Dock = DockStyle.None;

                                scaleValue = 1;

                                setInitialImage();

                                // 확대 축소 기능 사용 1.1
                                picImg.SizeMode = PictureBoxSizeMode.StretchImage;
                                picImg.Dock = DockStyle.None;
                            }
                            else
                            {
                                // 확대 축소 기능 사용 안함
                                picImg.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            org = new PictureBox();
                            org.Image = picImg.Image;

                            p = ImgPanel.AutoScrollPosition;
                        }
                        else
                        {
                            Message[0] = "[FTP] 파일 다운로드 실패. 통신상태를 확인해주세요.";
                            Message[1] = "[FTP파일 다운 오류]";
                            throw new Exception();
                        }
                    }
                }
                catch (Exception excpt)
                {
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[0], Message[1], 3, 1);
                }
            }
        }

        #region 이전본 백업 2020.12.10 이전

        private void FtpDownload_bak(string Path, string File)
        {
            if (Path != "" && File != null)
            {
                //if (PK != string.Empty)
                //{
                //    Path += @"/" + PK;
                //}

                FTP_EX _ftp = null;
                INI_GS gs = new INI_GS();

                string FTP_ADDRESS = "ftp://" + gs.GetValue("FTPINFO", "FileSvr", "wizis.iptime.org") + ":" + gs.GetValue("FTPINFO", "FTPPort", "25000");
                string FTP_ID = "wizuser";
                string FTP_PASS = "wiz9999";


                _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
                string LocalDirPath = Application.StartupPath + "\\" + "#Temp" + "\\" + Path + "\\"; //FTP서버내의 폴더명과 같은 폴더명을 LOCAL에서 사용하자;

                string FtpFolderPath = Path;//gs.GetValue("FTPINFO", "FTPIMAGEPATH", "/ImageData") + "/" + File; // ex)/ImageData/00065
                string[] fileListSimple;

                string Local_File = string.Empty;           //local 경로
                //picImg                                    //사진
                //lblImgName                                //text가 파일명 , tag 폴더명
                try
                {
                    fileListSimple = _ftp.directoryListSimple(FtpFolderPath, Encoding.Default);

                    //로컬경로 생성
                    DirectoryInfo dir = new DirectoryInfo(LocalDirPath);//로컬
                    if (dir.Exists == false)//로컬 폴더 존재 유무 확인 후 없을 시 생성
                    { dir.Create(); }
                    //로컬경로 생성

                    bool ftpExistFile = false;
                    picImg.Tag = Path + "/" + File;//파일 경로 + 파일명
                    Local_File = LocalDirPath + "\\" + File;//로컬경로

                    //파일 존재 유무 확인 있을때 ftpExistFile변수 True 없을때 False
                    foreach (string filename in fileListSimple)
                    {
                        if (string.Compare(filename, File) == 0)
                        { ftpExistFile = true; break; }
                    }

                    if (ftpExistFile == false)
                    {
                        Message[0] = "[FTP] " + File + " 이미지가 존재하지 않습니다.";
                        Message[1] = "[파일 존재하지 않음]";
                        throw new Exception();
                    }

                    else if (_ftp.GetFileSize(picImg.Tag.ToString()) == 0)//파일사이즈가 0일때
                    {
                        Message[0] = "[FTP] " + File + " 이미지의 파일사이즈가 0입니다. 사무실프로그램에서 파일을 다시 업로드 해주시기 바랍니다.";
                        Message[1] = "[파일 크기 오류]";
                        throw new Exception();
                    }

                    else//파일 사이즈가 0이 아닐때 기존폴더안의 파일들 삭제 후 다운로드
                    {
                        //FTP 다운로드 부분
                        FileInfo file = new FileInfo(Local_File);
                        if (file.Exists == true)//로컬 품명코드 폴더안의 파일 삭제
                        { file.Delete(); }
                        if (_ftp.download(picImg.Tag.ToString(), Local_File.ToString()))
                        {
                            FileStream fs = new FileStream(Local_File.ToString(), FileMode.Open, FileAccess.Read);
                            picImg.Image = System.Drawing.Image.FromStream(fs);
                            fs.Close();
                            //picImg.SizeMode = PictureBoxSizeMode.StretchImage;
                            picImg.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            Message[0] = "FTP파일 다운로드 실패. 통신상태를 확인해주세요.";
                            Message[1] = "[FTP파일 다운 오류]";
                            throw new Exception();
                        }
                    }
                }
                catch (Exception excpt)
                {
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[0], Message[1], 3, 1);
                }
            }
        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbInputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                SelectReasonBtn(sender, e);
                Close();
            }
        }

        private void tbInputText_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyCode = (int)e.KeyChar;  // 46 : Point  
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)) && !(keyCode == 46))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
            else if (keyCode == 46)
            {
                if (string.IsNullOrEmpty(tbInputText.Text) || tbInputText.Text.Contains('.') == true)
                {
                    e.Handled = true;
                }
            }

        }

        #region 이미지 확대 축소 2021-07-26
        Point p = new Point();
        double scaleValue = 1;

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (org != null
               && org.Image != null)
            {
                //picImg.Image = null;
                //picImg.Image = ZoomPicture(org.Image, 1.1, 1.1);
                //org.Image = picImg.Image;

                scaleValue += 0.2;

                picImg.ClientSize = new Size(
                     (int)(scaleValue * picImg.Image.Width),
                     (int)(scaleValue * picImg.Image.Height));

                ImgPanel.AutoScrollPosition = p;
            }
        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            if (org != null
               && org.Image != null)
            {
                //picImg.Image = null;
                //picImg.Image = ZoomPicture(org.Image, 0.9, 0.9);
                //org.Image = picImg.Image;

                scaleValue -= 0.2;

                picImg.ClientSize = new Size(
                    (int)(scaleValue * picImg.Image.Width),
                    (int)(scaleValue * picImg.Image.Height));

                ImgPanel.AutoScrollPosition = p;
            }
        }

        private void setInitialImage()
        {
            int width = ImgPanel.Size.Width - 5;
            int height = ImgPanel.Size.Height - 5;

            scaleValue = 1.0 * width / picImg.Image.Width;

            picImg.ClientSize = new Size(
                    (int)(scaleValue * picImg.Image.Width),
                    (int)(scaleValue * picImg.Image.Height));

            //if (height < picImg.Image.Height)
            //{
            //    scaleValue = 1.0 * height / picImg.Image.Height;

            //    picImg.ClientSize = new Size(
            //            (int)(scaleValue * picImg.Image.Width),
            //            (int)(scaleValue * picImg.Image.Height));
            //}
        }
        #endregion
    }
}