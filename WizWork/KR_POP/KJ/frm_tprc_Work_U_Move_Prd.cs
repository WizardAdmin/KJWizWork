﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using System.Management;
using System.Printing;
using System.Data.SqlClient;
using WizCommon;
using System.Runtime.InteropServices;

namespace WizWork
{
    public partial class frm_tprc_Work_U_Move_Prd : Form
    {
        private string m_LogQty = "";
        private string m_WorkQty = "";
        private string m_InstDetSeq = "";
        private string m_LogID = "";
        private string m_StationNO = "";


        private string updateJobID = "";        // 물고 들어간 Job ID
        private int nProcessID = 0;
        private string m_ProcessID = "";
        private string m_LabelID = "";          // Start Save Label ID
        private string ProdQty = "";

        private string m_WorkStartDate = "";
        private string m_WorkStartTime = "";

        /////////////////////////////
        ///
        private string m_MachineName = "";
        private string m_MachineID = "";
        private string m_LotID = "";
        private string m_MtrExceptYN = "";
        private string m_OutwareExceptYN = "";

        private string m_LastArticleYN = "";
        private string m_OrderID = "";
        private int m_OrderSeq = 0;
        private string m_OrderNO = "";
        private string m_UnitClss = "";
        private string m_UnitClssName = "";

        private string m_EffectDate = "";
        private string m_ProdAutoInspectYN = "";
        private string m_OrderArticleID = "";
        private string m_ArticleID = "";
        private string m_LabelGubun = "";

        private string m_ArticleIDMove = "";    //2021-09-29 외주 생산 시 이동용 변수
        private string m_LabelGubunMove = "";   //2021-09-29 외주 생산 시 이동용 변수
        private string m_UnitClssMove = "";     //2021-09-29 외주 생산 시 이동용 변수

        private string m_Inspector = "";
        private double m_RemainQty = 0;   //      입고수량
        private double m_LocRemainQty = 0;  //    '자품목 현 재고량
        private double m_douReqQty = 0; //현재품목 소요량
        private double m_douProdCapa = 0;

        private string m_ParentArticleID = "";
        public double m_CycleTime = 0;

        // 앞선 1번의 결과물을 알아야 뒤 라벨에 이 값을 그대로 박아 넣을 수 있으니까 가져와야 해.
        private string Wh_Ar_DayOrNightID = ""; // 주간이냐 / 야간이냐. 

        // PL_InputDet_SEQ 가 1이더라도, 라벨프린트 발행여부 에 따라, 뽑을지 말지 결정해야 한다.
        private string Wh_Ar_LabelPrintYN = "";

        private string m_StartSaveLabelID = ""; //2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가

        INI_GS gs = new INI_GS();
        //public Sub_TMold[] Sub_TMold = null;
        public Sub_TWkResult Sub_TWkResult = new Sub_TWkResult();
        public Sub_TWkResultArticleChild Sub_TWkResultArticleChild = new Sub_TWkResultArticleChild();
        public Sub_TWkLabelPrint Sub_TWkLabelPrint = new Sub_TWkLabelPrint();
        public Sub_TtdChange Sub_Ttd = new Sub_TtdChange();
        public TTag Sub_m_tTag = new TTag();
        public TTagSub Sub_m_tItem = new TTagSub();
        public List<TTagSub> list_m_tItem = new List<TTagSub>();
        public List<Sub_TWkResultArticleChild> list_TWkResultArticleChild = new List<Sub_TWkResultArticleChild>();
        public List<Sub_TWkResult> list_TWkResult_Another = new List<Sub_TWkResult>();
        public List<Sub_TWkResult> list_TWkResult = new List<Sub_TWkResult>();
        public List<Sub_TWkLabelPrint> list_TWkLabelPrint = new List<Sub_TWkLabelPrint>();
        public List<Sub_TWkResult_SplitAdd> list_TWkResult_SplitAdd = new List<Sub_TWkResult_SplitAdd>();

        public List<Sub_TMold> list_TMold = null;

        private List<string> lstStartLabel = new List<string>(); //2021-09-29 추가 스타트 라벨용

        List<string> lData = null;
        private string IsTagID = "";
        string[] m_sData = null;
        WizWorkLib Lib = new WizWorkLib();
        string MoldIDList = "";
        private string sTdGbn = "";
        
        private string ChildCheckYN = "N";
        private string LabelPrintYN = "N";      // 공정 간 이동전표를 뽑아햐 합니까? 그냥 저장만 하면 됩니까?
        string[] Message = new string[2];
        
        public bool blPRReuslt = true;  //해당 작업지시의 평량결과가 있는지 없는지
        public bool blpldClose = false; //해당 작업지시에서 현재공정보다 앞공정의 작업실적이 있는지 없는지
        public bool blSHExit = false;     //성형 작업종료 [uwkResult프로시저와 나머지 프로시저 사용]
        public string JobID0401 = "";
        public bool blClose = false;
        bool YLabelOK = false;          //바코드 스캔 시 Y라벨로 넣을 것인지에 대한 BOOL값

        WizCommon.Popup.Frm_CMNumericKeypad FK = null;
        WizCommon.Popup.Frm_CMKeypad FCK = null;

        private List<string> lstLabelList = new List<string>();
        private List<string> lstLabelListMove = new List<string>(); //2021-10-06 외주 이동용 리스트
        private List<float> lstQtyMove = new List<float>();  //2021-10-06 외주 이동용 리스트
        private List<float> lstQty = new List<float>();
        private double AnotherQty = 0;

        // 불량 리스트
        Dictionary<string, frm_tprc_Work_Defect_U_CodeView> dicDefect = new Dictionary<string, frm_tprc_Work_Defect_U_CodeView>();

        public frm_tprc_Work_U_Move_Prd()
        {
            InitializeComponent();
        }
        public frm_tprc_Work_U_Move_Prd(string JobID, string strProcessID, string StartSaveLabelID, string WorkStartDate, string WorkStartTime, string DayOrNightID)
        {
            InitializeComponent();
            updateJobID = JobID;
            m_ProcessID = strProcessID;
            m_LabelID = StartSaveLabelID;
            m_WorkStartDate = WorkStartDate;
            m_WorkStartTime = WorkStartTime;
            Wh_Ar_DayOrNightID = DayOrNightID;
        }

        public frm_tprc_Work_U_Move_Prd(string strProcessID, string DayOrNightID)
        {
            InitializeComponent();
            //updateJobID = JobID;
            m_ProcessID = strProcessID;
            this.lstStartLabel = Frm_tprc_Main.lstStartLabel;
            m_WorkStartDate = DateTime.Today.ToString("yyyyMMdd");
            m_WorkStartTime = DateTime.Now.ToString("HHmmss");
            //m_LabelID = StartSaveLabelID;
            //m_WorkStartDate = WorkStartDate;
            //m_WorkStartTime = WorkStartTime;
            Wh_Ar_DayOrNightID = DayOrNightID;
        }


        private void FormLoading()
        {
            m_MachineName = Frm_tprc_Main.g_tBase.Machine;
            m_MachineID = Frm_tprc_Main.g_tBase.MachineID;
            txtCarModel.Text = m_MachineName;

            FormInit();

            Frm_tprc_Main.list_g_tInsSub.Clear();
            txtDefectQty.Text = "";

            txtCycleTime.Text = stringFormatN0(m_CycleTime);
        }

        private void frm_tprc_Work_U_Load(object sender, EventArgs e)
        {
            FormLoading();
        }

        private void FormInit()
        {
            InitGridData1();
            InitGridData2();
            InitgrdBoxList();

            InitPanel();

            SetFormDataClear();

            Form_Activate();
        }

        #region InitPanel : Dock → Fill

        private void InitPanel()
        {
            
            tlpForm.Dock = DockStyle.Fill;
            tlpForm.Margin = new Padding(1, 1, 1, 1);

            foreach (Control control in tlpForm.Controls)
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(1, 1, 1, 1);
                foreach (Control contro in control.Controls)//tlp 상위에서 3번째
                {
                    contro.Dock = DockStyle.Fill;
                    contro.Margin = new Padding(1, 1, 1, 1);
                    foreach (Control contr in contro.Controls)
                    {
                        contr.Dock = DockStyle.Fill;
                        contr.Margin = new Padding(1, 1, 1, 1);
                        foreach (Control cont in contr.Controls)
                        {
                            cont.Dock = DockStyle.Fill;
                            cont.Margin = new Padding(1, 1, 1, 1);
                            foreach (Control con in cont.Controls)
                            {
                                con.Dock = DockStyle.Fill;
                                con.Margin = new Padding(1, 1, 1, 1);
                                foreach (Control co in con.Controls)
                                {
                                    co.Dock = DockStyle.Fill;
                                    co.Margin = new Padding(1, 1, 1, 1);
                                    foreach (Control c in co.Controls)
                                    {
                                        c.Dock = DockStyle.Fill;
                                        c.Margin = new Padding(1, 1, 1, 1);
                                        foreach (Control ctl in c.Controls)
                                        {
                                            ctl.Dock = DockStyle.Fill;
                                            ctl.Margin = new Padding(1, 1, 1, 1);
                                            foreach (Control ct in ctl.Controls)
                                            {
                                                ct.Dock = DockStyle.Fill;
                                                ct.Margin = new Padding(1, 1, 1, 1);
                                                foreach (Control cl in ct.Controls)
                                                {
                                                    cl.Dock = DockStyle.Fill;
                                                    cl.Margin = new Padding(1, 1, 1, 1);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        #endregion


        private void InitgrdWDcar()
        {
            int i = 0;
            //grdWDcar.Columns.Clear();
            
            foreach (DataGridViewColumn col in GridData1.Columns)
            {
                col.DataPropertyName = col.Name;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            return;
        }

        private void FillGridwdcard()
        {
            //xp_work_sWDcar
            int i = 0;
            DataSet ds = null;
            ds = DataStore.Instance.ProcedureToDataSet("xp_work_sWDcar", null, false);
            if (ds.Tables[0].Rows.Count > 0)
            {
                
            }

        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Frm_tprc_Main.list_g_tsplit.Clear();
            this.Close();
        }

        private void InitGridData1()
        {
            int i = 0;
            GridData1.Columns.Clear();
            GridData1.ColumnCount = 2;
            // Set the Colums Hearder Names
            GridData1.Columns[i].Name = "RowSeq";
            GridData1.Columns[i].HeaderText = "No";
            //GridData1.Columns[i].Width = 30;
            GridData1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData1.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData1.Columns[i].ReadOnly = true;
            GridData1.Columns[i++].Visible = true;

            GridData1.Columns[i].Name = "LotID";
            GridData1.Columns[i].HeaderText = "LotID";
            //GridData1.Columns[i].Width = 140;
            GridData1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridData1.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData1.Columns[i].ReadOnly = true;
            GridData1.Columns[i++].Visible = true;

            GridData1.Font = new Font("맑은 고딕", 15);//, FontStyle.Bold);
            GridData1.RowTemplate.Height = 30;
            GridData1.ColumnHeadersHeight = 37;
            GridData1.ScrollBars = ScrollBars.Both;
            GridData1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridData1.MultiSelect = false;
            GridData1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GridData1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(234, 234, 234);
            GridData1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridData1.ReadOnly = true;

            foreach (DataGridViewColumn col in GridData1.Columns)
            {
                col.DataPropertyName = col.Name;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            return;
        }

        private void InitGridData2()
        {
            int i = 0;
            GridData2.Columns.Clear();
            GridData2.ColumnCount = 21;

            // Set the Colums Hearder Names
            GridData2.Columns[i].Name = "RowSeq";
            GridData2.Columns[i].HeaderText = "No";
            //GridData2.Columns[i].Width = 30;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "InstID";
            GridData2.Columns[i].HeaderText = "InstID";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "DetSeq";
            GridData2.Columns[i].HeaderText = "DetSeq";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "ChildSeq";
            GridData2.Columns[i].HeaderText = "ChildSeq";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "ChildArticleID";
            GridData2.Columns[i].HeaderText = "ChildArticleID";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "Article";
            GridData2.Columns[i].HeaderText = "품명";
            //GridData2.Columns[i].Width = 120;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "BuyerArticle";
            GridData2.Columns[i].HeaderText = "품번";
            //GridData2.Columns[i].Width = 120;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "BarCode";
            GridData2.Columns[i].HeaderText = "바코드";
            //GridData2.Columns[i].Width = 120;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "ScanExceptYN";
            GridData2.Columns[i].HeaderText = "체크";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "LabelGubun";
            GridData2.Columns[i].HeaderText = "LabelGubun";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "Flag";
            GridData2.Columns[i].HeaderText = "Flag";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "ScanExceptYN1";
            GridData2.Columns[i].HeaderText = "예외";
            GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "RemainQty";
            GridData2.Columns[i].HeaderText = "전체 재고량";
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //GridData2.Columns[i].Width = 100;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = ConvertDouble(Frm_tprc_Main.g_tBase.sInstDetSeq) == 1 ? true : false;

            GridData2.Columns[i].Name = "LocRemainQty";
            GridData2.Columns[i].HeaderText = "라벨 재고량";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "UnitClss";
            GridData2.Columns[i].HeaderText = "하위품단위";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "UnitClssName";
            GridData2.Columns[i].HeaderText = "재고단위";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "ReqQty";
            GridData2.Columns[i].HeaderText = "소모량";
            //GridData2.Columns[i].Width = 40;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "ProdCapa";
            GridData2.Columns[i].HeaderText = "생산가능량";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "ProdUnitClss";
            GridData2.Columns[i].HeaderText = "생산단위";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Columns[i].Name = "ProdUnitClssName";
            GridData2.Columns[i].HeaderText = "생산단위";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = true;

            GridData2.Columns[i].Name = "EffectDate";
            GridData2.Columns[i].HeaderText = "유효기간";
            //GridData2.Columns[i].Width = 80;
            GridData2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            GridData2.Columns[i].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            GridData2.Columns[i].ReadOnly = true;
            GridData2.Columns[i++].Visible = false;

            GridData2.Font = new Font("맑은 고딕", 15);//, FontStyle.Bold);
            GridData2.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 12);//, FontStyle.Bold);
            GridData2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GridData2.RowTemplate.Height = 30;
            GridData2.ColumnHeadersHeight = 35;
            GridData2.ScrollBars = ScrollBars.Both;
            GridData2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridData2.MultiSelect = false;
            GridData2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GridData2.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(234, 234, 234);
            GridData2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridData2.ReadOnly = true;

            foreach (DataGridViewColumn col in GridData2.Columns)
            {
                col.DataPropertyName = col.Name;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            return;

        }

        /// <summary>
        /// Box GridList
        /// </summary>
        private void InitgrdBoxList()
        {
            int i = 0;
            //   grdBoxList.Columns.Clear();           

        }

        //2021-09-28 거래처 콤보 박스
        private void SetCombox()
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("LabelID", m_LabelID); 
                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_Move_Custom", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CustomBox.DataSource = dt;

                        CustomBox.ValueMember = dt.Columns["CustomID"].ToString();

                        CustomBox.DisplayMember = dt.Columns["KCustom"].ToString();

                        //dt.Rows[i][1].ToString() == dt.Rows[i][2].ToString()
                        //2021-09-29 기본 거래처는 외주 이동한 거래처로 나오게 하기 위해 
                        if (dt.Rows[i][1].ToString() == dt.Rows[i][2].ToString())
                        {
                            CustomBox.SelectedIndex = i;
                        }
                        //CustomBox.SelectedItem = dt.Rows[i].ItemArray[1].ToString();

                        //txtQtyPerBox.Text = dt.Rows[i].ItemArray[3].ToString();
                    }

                }
                else
                {
                    WizCommon.Popup.MyMessageBox.ShowBox("거래처가 없거나 외주이동한 라벨이 아닙니다. \n사무실 프로그램에서 거래처별 등록품목 또는 외주이동을 확인해주세요.", "[조회 실패]", 3, 1); //2021-10-06 문구 수정
                }
            }
            catch
            {
                WizCommon.Popup.MyMessageBox.ShowBox("데이터를 불러오지를 못합니다 \n관리자에게 문의해주세요.", "[조회 실패]", 3, 1);
                return;
            }
        }



        #region 자동 잔량이동처리 ← 하기 전에, 잔량이동처리할 라벨이 있는지 체크 해야 됨.

        private bool SaveMoveFromTo(double JobID, double WorkQty)
        {
            bool flag = false;
            // [xp_prdWork_iMoveByLotID]
            List<WizCommon.Procedure> Prolist = new List<WizCommon.Procedure>();
            List<List<string>> ListProcedureName = new List<List<string>>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                //2021-04-06 잔량 불러오기 후 LOT이동처리를 하기 위해 추가
                if (m_LabelID != (m_StartSaveLabelID == "" ? m_LabelID : m_StartSaveLabelID))
                {
                    sqlParameter.Add("JobID", Frm_tprc_Main.list_g_tsplit[0].JobID);
                }
                else
                {
                    sqlParameter.Add("JobID", JobID);
                }
                
                sqlParameter.Add("WorkQty", WorkQty);

                WizCommon.Procedure pro1 = new WizCommon.Procedure();
                pro1.Name = "[xp_prdWork_iMoveByLotID]";
                pro1.OutputUseYN = "N";
                pro1.OutputName = "LabelID";
                pro1.OutputLength = "20";

                Prolist.Add(pro1);
                ListParameter.Add(sqlParameter);

                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputToCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {
                    WizCommon.Popup.MyMessageBox.ShowBox("잔량이동처리가 완료 되었습니다.", "[완료]", 0, 1);
                    flag = true;
                }
                else
                {
                    foreach (KeyValue kv in list_Result)
                    {
                        if (kv.key.ToLower() == "failure")
                        {
                            throw new Exception(kv.value.ToString());
                        }
                    }
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
                flag = false;
            }

            return flag;
        }

        #endregion

        #region 해당 작업 건이 이미 등록된 건이 아닌지 체크 하기.

        private bool CheckAlreadyWorkIn()
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("JobID", ConvertDouble(updateJobID));

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_prdWork_CheckAlreadyWorkIn", sqlParameter, false);

                if (dt != null
                    && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    if (dt.Columns.Count == 1
                        && dr["Result"].ToString().ToUpper().Equals("SUCCESS"))
                    {
                        flag = true;
                    }
                    else if (dt.Columns.Count > 1)
                    {
                        // 예시 : 
                        // CNC 공정 1호기에 이미 같은 일자 및 시간으로 무작업이 등록되어 있습니다.
                        Message[0] = "[작업 중복 등록 오류]";
                        Message[1] = "해당 작업 건은 " + DatePickerFormat(dr["WorkEndDate"].ToString()) + " " + DateTimeFormat(dr["WorkEndTime"].ToString()) + "에 이미 실적이 등록 되었습니다.\r\n작업실적을 종료합니다.";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("중복 체크 구문 오류 [ CheckAlreadyWorkIn ] + \r\n" + ex.Message, "저장 전 체크 오류", 0, 1);
                return false;
            }

            return flag;
        }

        #endregion

        #region 해당 작업 건을 취소를 했는 지 안했는 지 체크하기
        private bool CheckAlreadyWorkOut()
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("JobID", ConvertDouble(updateJobID));

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_prdWork_CheckAlreadyWorkOut", sqlParameter, false);

                if (dt != null
                    && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    if (dt.Columns.Count == 1
                        && dr["Result"].ToString().ToUpper().Equals("SUCCESS"))
                    {
                        flag = true;
                    }
                    else if (dr["Result"].ToString().ToUpper().Equals("FAIL")) //old : dt.Columns.Count > 1 2021-08-20
                    {                       
                        Message[0] = "[작업 등록 오류]";
                        Message[1] = "해당 작업 건은 취소 되었습니다. 확인 후 다시 진행해주세요.";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("중복 체크 구문 오류 [ CheckAlreadyWorkOut ] + \r\n" + ex.Message, "저장 전 체크 오류", 0, 1);
                return false;
            }

            return flag;
        }
        #endregion

        #region 투입 라벨 생산가능량 교차 검증용

        private bool CheckProdCapa(double WorkQty)
        {
            bool flag = false;

            try
            {
                for (int i = 0; i < GridData2.Rows.Count; i++)
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("LabelID", GridData2.Rows[i].Cells["BarCode"].Value.ToString().Trim().ToUpper());
                    sqlParameter.Add("ArticleID", GridData2.Rows[i].Cells["ChildArticleID"].Value.ToString());

                    DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_prdWork_CheckProdCapa_B", sqlParameter, false); //2021-10-28 외주공정일 경우 외주창고에서 재고 찾기

                    if (dt != null
                        && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];

                        if (dt.Columns.Count > 0)
                        {
                            double LocRemainQty = ConvertDouble(dr["LocRemainQty"].ToString());
                            double ReaQty = ConvertDouble(GridData2.Rows[i].Cells["ReqQty"].Value.ToString());

                            // 생산 가능량
                            double ProdCapa = LocRemainQty / ReaQty;
                            ProdCapa = Math.Ceiling(ProdCapa); //2021-06-23 소수점 올림
                            if (WorkQty > ProdCapa)
                            {
                                WizCommon.Popup.MyMessageBox.ShowBox(sqlParameter["LabelID"].ToString() + " 라벨의 사용 이력이 있습니다. 정보를 갱신합니다.\r\n생산가능량을 확인해주세요.", "저장전 체크", 0, 1);
                                return false;
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            WizCommon.Popup.MyMessageBox.ShowBox(sqlParameter["LabelID"].ToString() + " 라벨 정보를 찾을 수 없습니다. [ CheckProdCapa ]\r\n관리자에게 문의해주세요.", "저장전 체크 오류", 0, 1);
                            return false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("투입 라벨 생산가능량 체크 오류 [ CheckProdCapa ] + \r\n" + ex.Message, "저장전 체크 오류", 0, 1);
                return false;
            }

            return flag;
        }

        #endregion

        #region 날짜 체크 하기(하루전까지만 허용)
        private bool CheckToday()
        {
            bool flag = false;
            int Yday = 0;
            int Day = 0;
            
            DateTime Now = DateTime.Now;
            DateTime YesterDay = Now.AddDays(-1);
            try
            {
                //Today = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                Yday = Int32.Parse(YesterDay.ToString("yyyyMMdd"));
                Day = Int32.Parse(Convert.ToDateTime(mtb_From.Text).ToString("yyyyMMdd"));

                //Day = Int32.Parse(mtb_From.Text);
                if (Yday > Day)
                {
                    Message[0] = "[작업 등록 오류]";
                    Message[1] = "해당 작업 일자를 확인 해주세요.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);                   
                }
                else
                {
                    flag = true;
                }                     
              
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("중복 체크 구문 오류 [ CheckToday ] + \r\n" + ex.Message, "저장 전 체크 오류", 0, 1);
                return false;
            }

            return flag;
        }
        #endregion

        //2021-09-29 추가
        private bool MoveID()
        {
            bool flag = false;

            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("OrderID", "");
                sqlParameter.Add("CompanyID", "0001"); //지엘에스 CustomID
                sqlParameter.Add("OutClss", "15");     //무조건 외주에서 사내로 들어와서 하드코딩

                sqlParameter.Add("CustomID", CustomBox.SelectedValue.ToString());

                sqlParameter.Add("BuyerDirectYN", "N"); // 이건 무조건 N
                sqlParameter.Add("WorkID", "0001");
                sqlParameter.Add("ExchRate", 0);
                sqlParameter.Add("UnitPriceClss", "");

                sqlParameter.Add("InsStuffInYN", "Y");
                sqlParameter.Add("OutcustomID", CustomBox.SelectedValue.ToString());  // 아놔암ㄴ이라ㅓㅁㅇ니라ㅓ 출고처

                sqlParameter.Add("Outcustom", CustomBox.SelectedText.ToString());
                sqlParameter.Add("LossRate", 0);
                sqlParameter.Add("LossQty", 0);

                sqlParameter.Add("OutRoll", 1); //박스 수 무조건 1박스
                sqlParameter.Add("OutQty", ConvertDouble(txtTotalLabelQty.Text)); //생산수량만큼 사내로 이동
                sqlParameter.Add("OutRealQty", ConvertDouble(txtTotalLabelQty.Text)); //생산수량만큼 사내로 이동
                sqlParameter.Add("OutDate", Convert.ToDateTime(mtb_From.Text).ToString("yyyyMMdd"));      //2021-09-29 이동날짜는 시작일로 저장
                sqlParameter.Add("ResultDate", Convert.ToDateTime(mtb_From.Text).ToString("yyyyMMdd"));   //2021-09-29 이동날짜는 시작일로 저장

                sqlParameter.Add("Remark", "생산에 의한 사내이동");
                sqlParameter.Add("OutType", 0); // CM_Code 테이블의 OUTTYP - 출고방식 : 0 : 수동 / 1 : 검사기준 자동 / 2: 기타출고 / 3 : PDA출고 > 사무실에서 하는거니까 0!!!!
                                                //sqlParameter.Add("OutSubType", tgnMoveByID.IsChecked == false ? (tgnMoveByQty.IsChecked == true ? "2" : "3") : "1"); // 1 : ID기준, 2 : 수량기준, 3 : 부분처리 2021-08-06 수량으로 이동 주석 처리로 인한 수정
                sqlParameter.Add("OutSubType", "1"); // 1 : ID기준(외주 생산 이동도 포함), 2 : 수량기준, 3 : 부분처리 

                sqlParameter.Add("Amount", 0);
                sqlParameter.Add("VatAmount", 0);
                sqlParameter.Add("VatINDYN", "");

                sqlParameter.Add("FromLocID", "B0001"); //2021-09-29 외주에서 사내로 이동
                sqlParameter.Add("ToLocID", "A0001");  //2021-09-29 외주에서 사내로 이동
                sqlParameter.Add("UnitClss", m_UnitClssMove); //2021-09-29 단위
                sqlParameter.Add("ArticleID", m_ArticleIDMove); //2021-09-29 ArticleID
                sqlParameter.Add("DvlyCustomID", ""); //2021-09-27 추가(PDA 추가랑 같은 프로시저 사용)
                sqlParameter.Add("UserID", Frm_tprc_Main.g_tBase.PersonID);

                sqlParameter.Add("OutSeq", 0); // output > ioutware 프로시저에서 새로 설정됨. 
                sqlParameter.Add("OutwareNo", ""); // output > OutwareID 임

                Dictionary<string, int> outputParam = new Dictionary<string, int>();
                outputParam.Add("OutwareNo", 12);
                outputParam.Add("OutSeq", 10);

                Dictionary<string, string> dicResult = DataStore.Instance.ExecuteProcedureOutputNoTran("xp_Outware_iOutware", sqlParameter, outputParam, true); //xp_Outware_iOutware 2021-09-27
                string result = dicResult["OutwareNo"];
                string resultSeq = dicResult["OutSeq"];

                if ((result != string.Empty) || (result != "9999"))
                {
                    int seq = 0; //선스캔 과 일괄 스캔 구분하기 위해 변수 추가
                    if (seq == 0)
                    {
                       
                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();

                        sqlParameter.Add("OutwareID", result);
                        sqlParameter.Add("OrderID", "");
                        sqlParameter.Add("OutSeq", ConvertInt(resultSeq));
                        sqlParameter.Add("OutSubSeq", 1);
                        sqlParameter.Add("OrderSeq", "");

                        sqlParameter.Add("LineSeq", 0);
                        sqlParameter.Add("LineSubSeq", 0);
                        sqlParameter.Add("RollSeq", 0);
                        sqlParameter.Add("LabelID", m_LabelID); //m_LabelID
                        sqlParameter.Add("LabelGubun", m_LabelGubunMove); // 2 : BoxID / 3: LotID

                        sqlParameter.Add("LotNo", ""); // 얘는 도대체 뭐여
                        sqlParameter.Add("Gubun", "N"); // N : 정상 / S : 샘플 / D : Defect(결함, 불량) > 부분처리 일경우에는 !!!!! 적용 되는 것들
                        sqlParameter.Add("StuffQty", 0);
                        sqlParameter.Add("OutQty", ConvertDouble(txtWorkQty.Text)); //ConvertDouble(txtWorkQty.Text)
                        sqlParameter.Add("OutRoll", 1); // 박스 갯수 - 라벨 하나당 박스 1개로 처리 하니, 1로 저장

                        sqlParameter.Add("UnitPrice", 0);
                        sqlParameter.Add("UserID", Frm_tprc_Main.g_tBase.PersonID);
                        sqlParameter.Add("CustomBoxID", "");
                        sqlParameter.Add("BoxID", "");


                        Procedure pro2 = new Procedure();
                        pro2.Name = "xp_Outware_iOutwareSub";
                        pro2.OutputUseYN = "N";
                        pro2.OutputName = "REQ_ID";
                        pro2.OutputLength = "10";

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter);
                        seq++;
                    }

                   if(seq > 0 && lstLabelList.Count > 0)
                   {
                        //2021-10-06 외주 이동 용 리스트 생성
                        for (int i = 0; i < lstLabelList.Count; i++)
                        {
                            // 일괄 스캔으로 가져온 실적들 세팅하기	
                            lstLabelListMove[i] = lstLabelList[i];
                            lstQtyMove[i] = lstQty[i];

                            // OutwareSub 에 등록
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            sqlParameter.Add("OutwareID", result);
                            sqlParameter.Add("OrderID", "");
                            sqlParameter.Add("OutSeq", ConvertInt(resultSeq));
                            sqlParameter.Add("OutSubSeq", 2+i);
                            sqlParameter.Add("OrderSeq", "");

                            sqlParameter.Add("LineSeq", 0);
                            sqlParameter.Add("LineSubSeq", 0);
                            sqlParameter.Add("RollSeq", 0);
                            sqlParameter.Add("LabelID", lstLabelListMove[i]); //m_LabelID
                            sqlParameter.Add("LabelGubun", m_LabelGubunMove); // 2 : BoxID / 3: LotID

                            sqlParameter.Add("LotNo", ""); // 얘는 도대체 뭐여
                            sqlParameter.Add("Gubun", "N"); // N : 정상 / S : 샘플 / D : Defect(결함, 불량) > 부분처리 일경우에는 !!!!! 적용 되는 것들
                            sqlParameter.Add("StuffQty", 0);
                            sqlParameter.Add("OutQty", lstQtyMove[i]); //ConvertDouble(txtWorkQty.Text)
                            sqlParameter.Add("OutRoll", 1); // 박스 갯수 - 라벨 하나당 박스 1개로 처리 하니, 1로 저장

                            sqlParameter.Add("UnitPrice", 0);
                            sqlParameter.Add("UserID", Frm_tprc_Main.g_tBase.PersonID);
                            sqlParameter.Add("CustomBoxID", "");
                            sqlParameter.Add("BoxID", "");


                            Procedure pro2 = new Procedure();
                            pro2.Name = "xp_Outware_iOutwareSub";
                            pro2.OutputUseYN = "N";
                            pro2.OutputName = "REQ_ID";
                            pro2.OutputLength = "10";

                            Prolist.Add(pro2);
                            ListParameter.Add(sqlParameter);
                        }
                   }
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("OutwareID", result);
                    sqlParameter.Add("sUserID", Frm_tprc_Main.g_tBase.PersonID);
                    sqlParameter.Add("sOutmsg", "");

                    Procedure pro3 = new Procedure();
                    pro3.Name = "xp_StuffIN_iStuffINByOutware";
                    pro3.OutputUseYN = "N";
                    pro3.OutputName = "REQ_ID";
                    pro3.OutputLength = "10";

                    Prolist.Add(pro3);
                    ListParameter.Add(sqlParameter);

                }

                string[] Confirm = new string[2];
                Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);
                if (Confirm[0] != "success")
                {
                    MessageBox.Show("[저장실패]\r\n" + Confirm[1].ToString());
                    flag = false;
                    //return false;
                }
                else
                {
                    //MessageBox.Show("성공");
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;

        }





        private void cmdSave_Click(object sender, EventArgs e)
        {       
            // 해당 작업 건이 이미 등록된 건이 아닌지 체크하기
            if (CheckAlreadyWorkIn() == false)
            {
                Frm_tprc_Main.list_g_tsplit.Clear();
                this.Close();
                return;
            }

            //해당 작업 건을 취소를 했는 지 안했는 지 체크하기 2021-08-20
            //외주 이동 생산은 임시 인서트를 하지 않아 주석 처리 2021-09-29
            //if (CheckAlreadyWorkOut() == false)
            //{
            //    Frm_tprc_Main.list_g_tsplit.Clear();
            //    this.Close();
            //    return;
            //}

            //날짜를 오늘 기준으로 하루 전으로만 변경이 되게 여기서 체크 2021-08-20 
            if (CheckToday() == false)
            {
                return;
            }

            // 2021-05-06 퇴사하기전에 남겨놓은 작업이 있는 경우를 때문에 작업자가 퇴사,근속 확인 후에 넘어가도록 추가, 
            // 똑같은 사람이 여러번 입사와 퇴사를 할수 있어 PersonID로 구분
            string Person = "";
            string[] PersonResign = new string[2];
            string sql = "select EndDate from mt_Person where Name = '" + txtNowWorker.Text + "' and PersonID = '"+ Frm_tprc_Main.g_tBase.PersonID + "'";
            PersonResign = DataStore.Instance.ExecuteQuery(sql, false);
            Person = PersonResign[1];

            if (Person != null && Person != "")
            {
                WizCommon.Popup.MyMessageBox.ShowBox("작업자 : " + txtNowWorker.Text + ", 퇴사자 입니다. 작업취소 후 작업자를 다시 선택해 주세요", "[저장 전 오류]", 0, 1);
                return;
            }

            ///MoveID(); //2021-09-29 외주 이동처리(mt_machine 에서 창고 가져옴(outware))

            m_douProdCapa = double.Parse(String.Format("{0:n0}", m_douProdCapa)); //2021-04-09 소수점 버림으로써 생산가능량에서 소수점버린 것과 같이 비교하기 위해 여기서도 소수점 버린것으로 비교
            if (m_douProdCapa != 0 || m_douProdCapa == 0)   // 생산가능량 : m_douProdCapa 
            {
                // 최소한, 생산가능량이 머라도 떴을 때, 이게 0이라는건 애초에 문제가 많고. +
                // 0이면 prescan 라인에서 막혀야 하니까.
                //2021-07-27 잔량이동처리를 사용하지 않아 주석처리 함
                //if (Lib.GetDouble(txtWorkQty.Text) > m_douProdCapa
                //    && ConvertInt(Frm_tprc_Main.g_tBase.sInstDetSeq) == 1)
                //{
                //for (int i = 0; i < GridData2.Rows.Count; i++)
                //{
                //    double CapaQty = ConvertDouble(GridData2.Rows[i].Cells["ProdCapa"].Value.ToString());
                //    double RemainQty = ConvertDouble(GridData2.Rows[i].Cells["RemainQty"].Value.ToString());
                //    double ReqQty = ConvertDouble(GridData2.Rows[i].Cells["ReqQty"].Value.ToString());

                //    double TotalCapaQty = RemainQty * ReqQty;

                //    if (ConvertDouble(txtWorkQty.Text) > TotalCapaQty && (TotalCapaQty.ToString().Contains("-") == false || TotalCapaQty != 0)) //2021-04-08 전체 재고량이 마이너스가 아니고 0이 아닐 경우에만 비교함
                //    {
                //        WizCommon.Popup.MyMessageBox.ShowBox("총 생산 가능량 : " + stringFormatN0(TotalCapaQty) + "\r\n 작업 수량을 전체 재고량 이하로 설정해주세요.", "[저장 전 오류]", 0, 1);
                //        return;
                //    }

                //    Message[0] = "[하위품 생산가능량 부족]";
                //    Message[1] = "하위품(해당 투입 라벨)의 생산가능량이 부족합니다.\r\n(최대 생산 가능량 : " + CapaQty + " )\r\n자동 잔량이동처리를 하시겠습니까?";
                //    if (WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 0) == DialogResult.OK)
                //    {
                //        if (SaveMoveFromTo(ConvertDouble(updateJobID), ConvertDouble(txtWorkQty.Text)))
                //        {
                //            FillGridData2(Frm_tprc_Main.g_tBase.sInstID, Frm_tprc_Main.g_tBase.ProcessID);
                //            //2021-04-06 잔량 불러오기 후 LOT이동처리를 위해 추가
                //            if (m_LabelID != (m_StartSaveLabelID == "" ? m_LabelID : m_StartSaveLabelID))
                //            {
                //                LotMoveBarcodeEnter();
                //            }
                //            else
                //            {
                //                BarcodeEnter();
                //            }
                //        }
                //        else
                //        {
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        return;
                //    }
                //}
                //}
                //else 
                //if (ConvertInt(Frm_tprc_Main.g_tBase.sInstDetSeq) != 1
                //    && Lib.GetDouble(txtWorkQty.Text) > m_douProdCapa)
                if (Lib.GetDouble(txtWorkQty.Text) > m_douProdCapa)
                {
                    // 내 순수 작업물량이 생산가능량 보다 크다면, 막아야 한다.
                    Message[0] = "[작업수량]";
                    Message[1] = "작업수량이 생산가능 수량보다 더 큽니다." +
                        "생산실적 저장을 중단합니다.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    return;
                }
            }

            // 투입 라벨 생산가능량 교차 검증
            if (CheckProdCapa(ConvertDouble(txtWorkQty.Text)) == false)
            {
                BarcodeEnter();             
                return;
            }


            bool SaveStart_OK = true;
            double LabelPaper_Qty = 0;              // 라벨에 찍혀나올 qty 값.
            double LabelPaper_Count = 0;            // 라벨 발행 부수.
            double LabelPaper_OneMoreQty = 0;       // 한장 더 이 수만큼 뽑거나  / or / 이 수만큼 split에 담아두거나.
            string Split_GBN = string.Empty;

            if (Frm_tprc_Main.g_tBase.sInstDetSeq == "1")
            {
                SaveStart_OK = false;

                string AllQty = txtTotalLabelQty.Text;
                string StandardQty = txtLotProdQty.Text;
                string BringRemainQty = txtRemainAdd.Text;
                string MyQty = txtWorkQty.Text;

                double d_AllQty = 0;
                double d_StandardQty = 0;
                double d_BringRemainQty = 0;
                double d_MyQty = 0;



                Double.TryParse(AllQty, out d_AllQty);
                Double.TryParse(StandardQty, out d_StandardQty);
                Double.TryParse(BringRemainQty, out d_BringRemainQty);
                Double.TryParse(MyQty, out d_MyQty);

                double d_DefectQty = ConvertDouble(txtDefectQty.Text);
                // d

                if (d_AllQty == 0)
                {
                    Message[0] = "[작업수량]";
                    Message[1] = "작업수량이 입력되지 않았습니다. \r\n" +
                        "라벨발행을 중단합니다.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    return;
                }
                if (d_AllQty < d_DefectQty)
                {
                    Message[0] = "[작업수량]";
                    Message[1] = "작업수량(" + d_AllQty + ")이 불량수량(" + d_DefectQty + ")보다 작습니다. \r\n" +
                        "라벨발행을 중단합니다.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    return;
                }
                if (d_AllQty - d_DefectQty == 0)
                {
                    Message[0] = "[작업수량]";
                    Message[1] = "불량(" + d_DefectQty + ")을 제외한 작업 수량(" + d_AllQty + ")이 0개 입니다. \r\n" +
                        "라벨발행을 중단합니다.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    return;
                }
                else if (d_StandardQty == 0)
                {
                    Message[0] = "[기준수량]";
                    Message[1] = "생산박스 당 수량이 입력되지 않았습니다. \r\n" +
                        "라벨발행을 중단합니다.";
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    return;
                }

                if (!CheckIsSameWorkTime())
                {
                    return;
                }

                // 가져온 잔량이 있는 상태에서,
                if (Frm_tprc_Main.list_g_tsplit.Count > 0)
                {
                    //int value = ((int)d_StandardQty * Frm_tprc_Main.list_g_tsplit.Count) - (int)d_BringRemainQty;
                    //if (value > (int)d_MyQty)
                    //{
                    //    Message[0] = "[박스수량 문제]";
                    //    Message[1] = "최소 " + value.ToString() + "개 이상은 작업해야 박스수량을 맞출 수 있습니다. \r\n" +
                    //        "라벨발행을 중단합니다.";
                    //    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    //    return;
                    //}

                    int TotalQty = ConvertInt(txtTotalLabelQty.Text);
                    int BoxQty = ConvertInt(txtLotProdQty.Text);
                    int DefectQty = ConvertInt(txtDefectQty.Text);
                    TotalQty = TotalQty - DefectQty;

                    //int result = 0;

                    //if (TotalQty < BoxQty)
                    //{
                    //    result = BoxQty - TotalQty;
                    //}
                    //else if (TotalQty > BoxQty)
                    //{
                    //    result = BoxQty == 0 ? 0 : TotalQty % BoxQty;

                    //    if (result != 0)
                    //    {
                    //        result = BoxQty - result;
                    //    }
                    //}


                    // ↓ 2020.10.19 GLS 한박스가 안되더라고 바코드 출력과 함께 작업 완료 되도록 요청
                    //if (TotalQty < BoxQty)
                    //{
                    //    int result = 0;
                    //    result = BoxQty - TotalQty;

                    //    Message[0] = "[박스수량 문제]";
                    //    if (DefectQty > 0)
                    //    {
                    //        Message[1] = "등록된 불량 " + d_DefectQty + "개를 제외하고 최소 " + result.ToString() + "개 이상 \r\n더 작업해야 합니다.";       
                    //    }
                    //    else
                    //    {
                    //        Message[1] = "최소 " + result.ToString() + "개 이상 작업해야합니다.";
                    //    }

                    //    Message[1] += "\r\n(잔량 불러오기 기능을 사용하는 경우,\r\n  최소 한 박스 이상 작업을 해야 합니다.)";

                    //    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    //    return;
                    //}
                }

                bool PopUp_HeaderMessage = false;
                if (Wh_Ar_LabelPrintYN == "N")
                {
                    PopUp_HeaderMessage = true;
                }
                else { PopUp_HeaderMessage = false; }

                // 첫 공정으로서 > 전표발행을 담당하는 공정이라면,
                // 라벨발행 유형 타입을 설정합니다.
                frm_PopUp_ChoiceLabelPrintType ChoiceType = new frm_PopUp_ChoiceLabelPrintType(d_AllQty - d_DefectQty, d_StandardQty, PopUp_HeaderMessage);
                ChoiceType.Owner = this;
                ChoiceType.WriteTextEvent += ChoiceType_WriteTextEvent;
                ChoiceType.ShowDialog();

                void ChoiceType_WriteTextEvent(string Message, double Qty, double PrintCount, double RemainOneMoreQty)
                {
                    if (Message == "Cancel")
                    { return; }
                    else
                    {
                        Split_GBN = Message;
                        LabelPaper_Qty = Qty;
                        LabelPaper_Count = PrintCount;
                        LabelPaper_OneMoreQty = RemainOneMoreQty;
                        SaveStart_OK = true;
                    }
                }
            }          

            if (SaveStart_OK == true)
            {
                Save_Function(Split_GBN, LabelPaper_Qty, LabelPaper_Count, LabelPaper_OneMoreQty);
            }                        
        }

        #region 만약에 같은 시간, 공정, 호기로 작업한게 있다면.. 막기

        private bool CheckIsSameWorkTime()
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("sProcessID", Frm_tprc_Main.g_tBase.ProcessID);
                sqlParameter.Add("sMachineID", Frm_tprc_Main.g_tBase.MachineID);
                sqlParameter.Add("WorkStartDate", mtb_From.Text.Replace("-", ""));
                sqlParameter.Add("WorkEndDate", mtb_To.Text.Replace("-", ""));
                sqlParameter.Add("WorkStartTime", dtStartTime.Value.ToString("HHmmss"));
                sqlParameter.Add("WorkEndTime", dtEndTime.Value.ToString("HHmmss"));

                //DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_prdWork_CheckIsSameWorkTime", sqlParameter, false); //2021-05-18
                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_CheckWorkTime", sqlParameter, false); 
                if (dt != null
                    && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    if (dt.Columns.Count == 1
                        && dr["Msg"].ToString().ToUpper().Equals(""))
                    {

                        flag = true;
                    }
                    else if (dr["Msg"].ToString().ToUpper().Equals("이미 동일 시간대에 생산실적이 있습니다."))  //else if (dt.Columns.Count > 1) 2021-05-18 시간대가 겹치면 안들어가도록 수정
                    {
                        // 예시 : 
                        // CNC 공정 1호기에 이미 같은 일자 및 시간으로 무작업이 등록되어 있습니다.

                        Message[0] = "[작업 중복 등록 오류]";
                        Message[1] = dr["Msg"].ToString();
                        //Message[1] = dr["Process"].ToString() + " 공정 " + dr["MachineNo"].ToString() + "에 같은 일자 및 시간으로\r\n이미 작업이 등록되어 있습니다.";
                        //Message[1] += "\r\n[" + dr["Process"].ToString() + " / " + dr["MachineNo"].ToString() + " / " + dr["Name"].ToString() + "]";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("중복 체크 구문 오류 [ CheckIsSameWorkTime ] + \r\n" + ex.Message, "중복 체크 오류", 0, 1);
                return false;
            }

            return flag;
        }

        #endregion


        #region 실제 주요 저장 구문. (Save_Function)
        // 저장 _ 실제 주요 로직 구문.
        private void Save_Function(string Split_GBN, double LabelPaper_Qty, 
                                   double LabelPaper_Count, double LabelPaper_OneMoreQty)
        {
            try
            {

                Cursor = Cursors.WaitCursor;

                string sWDNO = "";
                string sWDID = "";
                float sWDQty = 0;
                int iCnt = 0;
                int nColorRow = 0;

                float StartTime = 0;
                float EndTime = 0;
                bool Success = false;
                float sLogID = 0;

                string FacilityCollectQty = "";
                string LotTotalQty = "";
                string CycleTime = "";



                ProdQty = Lib.GetDouble(txtWorkQty.Text).ToString();    // 순수 내 작업물량.
                //StartTime = float.Parse(mtb_From.Text.Replace("-", "") + dtStartTime.Value.ToString("HHmmss"));
                //EndTime = float.Parse(mtb_To.Text.Replace("-", "") + dtEndTime.Value.ToString("HHmmss"));
                DateTime S_DT = DateTime.ParseExact(mtb_From.Text.Replace("-", "") + dtStartTime.Value.ToString("HHmmss"), "yyyyMMddHHmmss", null);
                DateTime E_DT = DateTime.ParseExact(mtb_To.Text.Replace("-", "") + dtEndTime.Value.ToString("HHmmss"), "yyyyMMddHHmmss", null);



                FacilityCollectQty = Lib.GetDouble(txtFacilityCollectQty.Text).ToString();      // 설비수집 수량
                LotTotalQty = Lib.GetDouble(txtTotalLabelQty.Text).ToString();                    // 로트별 총 수량
                CycleTime = Lib.GetDouble(txtCycleTime.Text).ToString();                      // CycleTime


                if (S_DT > E_DT)
                {
                    throw new Exception("시작시간이 종료시간보다 더 큽니다.");
                }
                if (Frm_tprc_Main.g_tBase.ProcessID == "" || Frm_tprc_Main.g_tBase.MachineID == "")
                {
                    throw new Exception("공정 또는 호기가 선택되지 않았습니다.");
                }
                if (Frm_tprc_Main.g_tBase.PersonID == "")//수정필요
                {
                    throw new Exception("작업자가 선택되지 않았습니다.");
                }
                if (ProdQty == "0")
                {
                    throw new Exception("작업수량이 입력되지 않았습니다.");
                }

                //'-------------------------------------------------------------------------------
                //'생산실적 잔량 초과 실적 등록 불가
                //'-------------------------------------------------------------------------------
                if (txtInUnitClss.Text == "EA") // 생산제품의 단위가 갯수(EA)로 세리고 있는 공정이라면
                {
                    if ((int)Lib.GetDouble(ProdQty) > (int)Lib.GetDouble(txtInstRemainQty.Text))
                    {
                        Message[0] = "[확인]";
                        Message[1] = "생산잔량: " + txtInstRemainQty.Text + "입니다. 초과된 생산 실적을 등록하시겠습니까?";
                        // 등록 불가 합니다. \r\n 계속 진행 하시겠습니까?";
                        if (WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 0) == DialogResult.No)
                        {
                            Cursor = Cursors.Default;
                            return;
                        }
                    }
                }
                else                // Gram이라 가정한다면.
                {
                    double douProdQty = 0;
                    double douInstRemainQty = 0;
                    douProdQty = Lib.GetDouble(ProdQty) / 1000;//kg단위로 변환
                    douInstRemainQty = Lib.GetDouble(txtInstRemainQty.Text);//kg단위
                    if (douProdQty > douInstRemainQty)
                    {
                        Message[0] = "[확인]";
                        Message[1] = "생산잔량: " + txtInstRemainQty.Text + "kg입니다. 초과된 생산 실적을 등록하시겠습니까?";
                        if (WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 0) == DialogResult.No)
                        {
                            Cursor = Cursors.Default;
                            return;
                        }
                    }
                }
                //18.06.18 주석 
                //txtInstRemainQty.Text = string.Format("{0:#,###}", float.Parse(Lib.CheckNum(InstQty.ToString())) - float.Parse(Lib.CheckNum(WorkQty.ToString())) - float.Parse(Lib.CheckNum(ProdQty)));
                //18.06.18 주석 


                //'-----------------------------------------------------------------------------------------
                //'탭/다이스 교환
                //'-----------------------------------------------------------------------------------------

                Sub_Ttd.TdChangeYN = "N";
                sWDNO = "";
                sWDID = "";

                int TWkRCon = 1;

                // 첫공정인지 아닌지 이걸로 판단할거야.
                // 가장 중요한 데이터 중 하나가 된듯..
                int InstDetSeq = 0;
                int.TryParse(Lib.GetDouble(txtInstDetSeq.Text).ToString(), out InstDetSeq);

                if (InstDetSeq == 1)
                {
                    if (Frm_tprc_Main.list_g_tsplit.Count > 0)
                    {
                        // Main.TSplit에서 가져온 리스트가 있다.
                        if (Main_TSplit_ConnectionEvent() == true)
                        {
                            list_TWkResult_SplitAdd.Clear();
                        }
                        else
                        {
                            Message[0] = "[오류]";
                            Message[1] = "Main.TSplit 연계작업에 실패하여 저장구문을 종료합니다.";
                            WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                            Cursor = Cursors.Default;
                            return;                            
                        }
                    }
                    // 연계작업에 성공했다면, 혹은 연계데이터가 없다면,

                    // 첫 공정이다.      // 총 도는 횟수는 라벨 페이퍼 카운트에 맞춰야 하고,
                    TWkRCon = (int)LabelPaper_Count;

                    if (TWkRCon == 0)
                    {
                        TWkRCon = 1;
                    }
                    if ((Split_GBN == "YO" || Split_GBN == "YC") && (LabelPaper_OneMoreQty > 0))
                    {
                        TWkRCon = TWkRCon + 1;
                    }

                    if (Frm_tprc_Main.list_g_tsplit.Count > 0)
                    {
                        TWkRCon = TWkRCon - Frm_tprc_Main.list_g_tsplit.Count;
                        if (TWkRCon < 0) { TWkRCon = 0; }

                        // TWkRCon = 0
                        // 실적 저장을 하는데 얘가 0 이다 라는 말은
                        // 장입량 : 350
                        // 잔량 : 100 개
                        // 작업수량을 : 250 
                        // → 생산 수량이 박스장입량 미만일 경우 [처음 선택한 해당 작업 이력]이 [작업 중]으로 남아있게 됨
                        // → [처음 선택한 해당 작업 이력] 삭제하기
                        if (TWkRCon == 0)
                        {
                            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Add("JobID", float.Parse(updateJobID));// grdData.Rows[rowIndex].Cells["JobID"].Value.ToString()); 2021-04-06
                            sqlParameter.Add("CreateUserID", Frm_tprc_Main.g_tBase.PersonID);
                            sqlParameter.Add("sRtnMsg", "");
                            string[] sConfirm = new string[2];
                            sConfirm = DataStore.Instance.ExecuteProcedure("xp_wkResult_dWkResult", sqlParameter, true); //삭제
                            if (sConfirm[0].ToUpper() == "SUCCESS")
                            {
                                //MessageBox.Show("삭제 완료");
                            }

                            // 만약에 불량이 있다면
                            if (ConvertInt(txtDefectQty.Text) > 0)
                            {
                                int DCnt = Frm_tprc_Main.g_tBase.DefectCnt;

                                List<WizCommon.Procedure> Prolist = new List<WizCommon.Procedure>();
                                List<List<string>> ListProcedureName = new List<List<string>>();
                                List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

                                for (int i = 0; i < DCnt; i++)
                                {
                                    Dictionary<string, object> sqlParameter4 = new Dictionary<string, object>();

                                    sqlParameter4.Add("WkDefectID", "");
                                    sqlParameter4.Add("OrderID", Frm_tprc_Main.list_g_tInsSub[i].OrderID);
                                    sqlParameter4.Add("OrderSeq", Frm_tprc_Main.list_g_tInsSub[i].OrderSeq);
                                    sqlParameter4.Add("ProcessID", Frm_tprc_Main.list_g_tInsSub[i].ProcessID);
                                    sqlParameter4.Add("MachineID", Frm_tprc_Main.list_g_tInsSub[i].MachineID);

                                    sqlParameter4.Add("DefectQty", Frm_tprc_Main.list_g_tInsSub[i].nDefectQty);
                                    sqlParameter4.Add("BoxID", Frm_tprc_Main.list_g_tInsSub[i].BoxID);
                                    sqlParameter4.Add("DefectID", Frm_tprc_Main.list_g_tInsSub[i].DefectID);
                                    sqlParameter4.Add("XPos", Frm_tprc_Main.list_g_tInsSub[i].XPos);
                                    sqlParameter4.Add("YPos", Frm_tprc_Main.list_g_tInsSub[i].YPos);

                                    sqlParameter4.Add("InspectDate", Frm_tprc_Main.list_g_tInsSub[i].InspectDate);
                                    sqlParameter4.Add("InspectTime", Frm_tprc_Main.list_g_tInsSub[i].InspectTime);
                                    sqlParameter4.Add("PersonID", Frm_tprc_Main.list_g_tsplit[0].PersonID);
                                    sqlParameter4.Add("JobID", Frm_tprc_Main.list_g_tsplit[0].JobID);
                                    sqlParameter4.Add("CreateUserID", Frm_tprc_Main.list_g_tsplit[0].PersonID);

                                    WizCommon.Procedure pro5 = new WizCommon.Procedure();
                                    pro5.Name = "xp_wkResult_iInspect";
                                    pro5.OutputUseYN = "N";
                                    pro5.OutputName = "JobID";
                                    pro5.OutputLength = "20";

                                    Prolist.Add(pro5);
                                    ListParameter.Add(sqlParameter4);
                                }

                                List<KeyValue> list_Result = new List<KeyValue>();
                                list_Result = DataStore.Instance.ExecuteAllProcedureOutputToCS(Prolist, ListParameter);

                                if (list_Result[0].key.ToLower() == "success")
                                {
                                    list_Result.RemoveAt(0);
                                }
                                else
                                {
                                    foreach (KeyValue kv in list_Result)
                                    {
                                        if (kv.key.ToLower() == "failure")
                                        {
                                            throw new Exception(kv.value.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


       
                //'-------------------------------------------------------------------------------
                //'상위품 설정
                //'-------------------------------------------------------------------------------

                list_TWkResult = new List<Sub_TWkResult>();
                
                float WorkQty = 0;
                //float Lot_ProdQty = 0;

                float nCycleTime = 0;
                for (int i = 0; i < TWkRCon; i++)
                {
                    list_TWkResult.Add(new Sub_TWkResult());

                    list_TWkResult[i].JobID = (float)ConvertDouble(updateJobID);         //float.Parse(updateJobID);
                    list_TWkResult[i].InstID = txtInstID.Text;                    
                    list_TWkResult[i].InstDetSeq = InstDetSeq;
                    list_TWkResult[i].LabelID = txtPreInsertLabelBarCode.Text;
                    list_TWkResult[i].LabelGubun = this.txtLabelGubun.Text;

                    list_TWkResult[i].ProcessID = Frm_tprc_Main.g_tBase.ProcessID;
                    list_TWkResult[i].MachineID = Frm_tprc_Main.g_tBase.MachineID;
                    list_TWkResult[i].ScanDate = mtb_From.Text.Replace("-", "");
                    list_TWkResult[i].ScanTime = dtStartTime.Value.ToString("HHmmss");

                    list_TWkResult[i].ArticleID = txtArticleID.Text;


                    // 작업수량 결정 구문 (wk_result 작업수량은 내 순수 작업수량에 맞춰야 한다.)
                    ////////////////////////////////
                    ///
                    if (InstDetSeq == 1)
                    {
                        // 첫 공정
                        if (TWkRCon == 1)
                        {
                            // Update값 물고난 뒤에 한번만 발행하면 되는 케이스.
                            if ((Split_GBN == "YO" || Split_GBN == "YC") && (LabelPaper_OneMoreQty > 0))
                            {
                                float.TryParse(LabelPaper_OneMoreQty.ToString(), out WorkQty);
                                list_TWkResult[i].SplitYNGBN = Split_GBN;
                            }
                            else
                            {
                                float.TryParse(LabelPaper_Qty.ToString(), out WorkQty);
                                list_TWkResult[i].SplitYNGBN = "NC";
                            }
                        }
                        else
                        {
                            if ((i + 1 == TWkRCon) && (Split_GBN == "YO" || Split_GBN == "YC") && (LabelPaper_OneMoreQty > 0))
                            {
                                float.TryParse(LabelPaper_OneMoreQty.ToString(), out WorkQty);
                                list_TWkResult[i].SplitYNGBN = Split_GBN;
                            }
                            else
                            {
                                float.TryParse(LabelPaper_Qty.ToString(), out WorkQty);
                                list_TWkResult[i].SplitYNGBN = "NC";
                            }                            
                        }


                            //float.TryParse(ProdQty.ToString(), out WorkQty);
                            //float.TryParse(txtLotProdQty.Text, out Lot_ProdQty);

                            //if (WorkQty > Lot_ProdQty)
                            //{
                            //    // 아직은 생산박스 기준보다 내가 생산한 자체수량이 더 크다.
                            //    // 1. for문 돌아야 하니까, workqty 값 삭감.
                            //    WorkQty = WorkQty - Lot_ProdQty;
                            //    // 2. 삭감한 workqty값을 ProdQty로 재 이식.
                            //    ProdQty = WorkQty.ToString();

                            //    // 3. 생산박스 기준치 만큼 wk_result로
                            //    float.TryParse(LabelPaper_Qty.ToString(), out WorkQty);
                            //}
                            //else if (WorkQty == Lot_ProdQty)
                            //{
                            //    float.TryParse(ProdQty.ToString(), out WorkQty);
                            //}
                            //else if (WorkQty < Lot_ProdQty)
                            //{
                            //    if (WorkQty == 0)
                            //    {
                            //    }
                            //    else
                            //    {
                            //        // 1. 남아있는 WorkQty 값만큼 wk_result로 보내고,
                            //        float.TryParse(ProdQty.ToString(), out WorkQty);
                            //        // 2. 혹, 앞으로 for문을 더 돌아야 할 지도 모르니까, ProdQty는 0으로 이식.
                            //        ProdQty = "0";
                            //    }
                            //}                            
                                              
                    }
                    else
                    {
                        float.TryParse(ProdQty, out WorkQty);
                        list_TWkResult[i].SplitYNGBN = Split_GBN;
                    }

                    list_TWkResult[i].WorkQty = WorkQty;

                    // 작업수량 결정 구문 끝.
                    ////////////////////////////////
                    ///


                    float.TryParse(CycleTime, out nCycleTime);
                    list_TWkResult[i].CycleTime = nCycleTime;


                    // 2020.06.09 둘리
                    // 불량 → 그냥 실적 저장일때, 불량 갯수를 제외해줘야 한다
                     if (LabelPrintYN == "N"
                        && i == 0)
                    {
                        float DefectQty = 0;
                        float.TryParse(txtDefectQty.Text, out DefectQty);
                        list_TWkResult[i].WorkQty = WorkQty - DefectQty;
                        txtDefectQty.Text = string.Empty;
                    }
                    //// 불량 테스트 - 둘리
                    //if (i == TWkRCon - 1)
                    //{
                    //    if (txtDefectQty.Text != string.Empty)
                    //    {
                    //        float DefectQty = 0;
                    //        float.TryParse(txtDefectQty.Text, out DefectQty);
                    //        list_TWkResult[i].WorkQty = WorkQty - DefectQty;
                    //        txtDefectQty.Text = string.Empty;
                    //    }
                    //}

                    list_TWkResult[i].Comments = Frm_tprc_Main.g_tBase.ProcessID + "작업종료에 따른 데이터 저장(Insert)";
                    list_TWkResult[i].ReworkOldYN = "";
                    list_TWkResult[i].ReworkLinkProdID = "";

                    list_TWkResult[i].WorkStartDate = mtb_From.Text.Replace("-", "");
                    list_TWkResult[i].WorkStartTime = dtStartTime.Value.ToString("HHmmss");
                    list_TWkResult[i].WorkEndDate = mtb_To.Text.Replace("-", "");
                    list_TWkResult[i].WorkEndTime = dtEndTime.Value.ToString("HHmmss");
                    list_TWkResult[i].JobGbn = "1";

                    list_TWkResult[i].sNowReworkCode = "";
                    list_TWkResult[i].WDNO = sWDNO;
                    list_TWkResult[i].WDID = sWDID;
                    list_TWkResult[i].WDQty = sWDQty;
                    float.TryParse(m_LogID, out sLogID);
                    list_TWkResult[i].sLogID = sLogID;

                    list_TWkResult[i].s4MID = "";
                    list_TWkResult[i].DayOrNightID = Wh_Ar_DayOrNightID;
                    list_TWkResult[i].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;
                    

                    //'------------------------------------------------------------------------------------------
                    list_TWkResult[i].sLastArticleYN = m_LastArticleYN;
                    list_TWkResult[i].ProdAutoInspectYN = m_ProdAutoInspectYN;
                    list_TWkResult[i].sOrderID = m_OrderID;
                    list_TWkResult[i].nOrderSeq = m_OrderSeq;                    
                                                           
                }


                if (Frm_tprc_Main.list_tMold.Count > 0)
                {
                    int nCount = list_TWkResult.Count;
                    list_TMold = new List<Sub_TMold>();
                    for (int i = 0; i < nCount; i++)
                    {
                        list_TMold.Add(new Sub_TMold());
                    }

                    //list_TMold = new Sub_TMold[nCount];
                }

                //if (Frm_tprc_Main.g_tMol.sMoldID != "")
                //{
                //    if (TWkRCon == 1)
                //    {
                //        Sub_TMold = new Sub_TMold[1];

                //        Sub_TMold[0].sMoldID = Frm_tprc_Main.g_tMol.sMoldID;
                //        Sub_TMold[0].sRealCavity = Frm_tprc_Main.g_tMol.sRealCavity;
                //        Sub_TMold[0].sHitCount = int.Parse(Lib.CheckNum(Sub_TWkResult[0].WorkQty.ToString()));
                //    }

                //}
                if (Frm_tprc_Main.list_tMold.Count > 0)
                {
                    if (Frm_tprc_Main.list_tMold[0].sMoldID != "")
                    {
                        //if (TWkRCon > 1)
                        //{
                        //    //Sub_TMold = new Sub_TMold[/*list_TWkResult.Count*/TWkRCon];
                        //}
                        for (int i = 0; i < TWkRCon/*list_TWkResult.Count*/; i++)
                        {
                            for (int j = 0; j < Frm_tprc_Main.list_tMold.Count; j++)
                            {
                                list_TMold[i].sMoldID = Frm_tprc_Main.list_tMold[j].sMoldID;
                                list_TMold[i].sRealCavity = Frm_tprc_Main.list_tMold[j].sRealCavity;

                            }
                        }
                    }
                }

                iCnt = 0;

                //'-------------------------------------------------------------------------------
                //'하위품 설정
                //'-------------------------------------------------------------------------------

                nColorRow = GridData2.Rows.Count;
                if (nColorRow > 0)
                {
                    list_TWkResultArticleChild = new List<Sub_TWkResultArticleChild>();

                    for (int i = 0; i < nColorRow; i++)
                    {
                        DataGridViewRow row = GridData2.Rows[i];

                        list_TWkResultArticleChild.Add(new Sub_TWkResultArticleChild());

                        list_TWkResultArticleChild[i].JobID = 0;
                        list_TWkResultArticleChild[i].ChildLabelID = Lib.CheckNull(row.Cells["BarCode"].Value.ToString());
                        list_TWkResultArticleChild[i].ChildLabelGubun = Lib.CheckNull(row.Cells["LabelGubun"].Value.ToString());
                        list_TWkResultArticleChild[i].ChildArticleID = Lib.CheckNull(row.Cells["ChildArticleID"].Value.ToString());
                        list_TWkResultArticleChild[i].ReworkOldYN = "";
                        list_TWkResultArticleChild[i].ReworkLinkChildProdID = "";
                        list_TWkResultArticleChild[i].OutDate = DateTime.Now.ToString("yyyyMMdd");
                        list_TWkResultArticleChild[i].OutTime = DateTime.Today.ToString("HHmmss");
                        list_TWkResultArticleChild[i].Flag = Lib.CheckNull(row.Cells["Flag"].Value.ToString());
                        list_TWkResultArticleChild[i].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;

                        //Sub_TWkResultArticleChild[i].JobID = 0;
                        ////Sub_TWkResultArticleChild[i].JobSeq = int.Parse(Lib.CheckNull(row.Cells["Seq"].Value.ToString()));
                        //Sub_TWkResultArticleChild[i].ChildLabelID = Lib.CheckNull(row.Cells["BarCode"].Value.ToString());
                        //Sub_TWkResultArticleChild[i].ChildLabelGubun = Lib.CheckNull(row.Cells["LabelGubun"].Value.ToString());
                        //Sub_TWkResultArticleChild[i].ChildArticleID = Lib.CheckNull(row.Cells["ChildArticleID"].Value.ToString());
                        //Sub_TWkResultArticleChild[i].ReworkOldYN = "";
                        //Sub_TWkResultArticleChild[i].ReworkLinkChildProdID = "";
                        //Sub_TWkResultArticleChild[i].OutDate = Lib.MakeDate(4, DateTime.Today.ToString("yyyyMMdd"));
                        //Sub_TWkResultArticleChild[i].OutTime = Lib.MakeDate(4, DateTime.Today.ToString("HHmmss"));
                        //Sub_TWkResultArticleChild[i].Flag = Lib.CheckNull(row.Cells["Flag"].Value.ToString());
                        //Sub_TWkResultArticleChild[i].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;

                        iCnt++;
                    }
                }

                //'-------------------------------------------------------------------------------
                //'불량입력화면에서 가져온 불량 수량 만큼의 정보에 데이타 설정
                //'-------------------------------------------------------------------------------

                if (Frm_tprc_Main.g_tBase.DefectCnt > 0)
                {
                    for (int i = 0; i < Frm_tprc_Main.g_tBase.DefectCnt; i++)
                    {
                        Frm_tprc_Main.list_g_tInsSub[i].BoxID = txtBoxID.Text;
                        Frm_tprc_Main.list_g_tInsSub[i].OrderID = m_OrderID;
                        Frm_tprc_Main.list_g_tInsSub[i].OrderSeq = m_OrderSeq;
                    }
                }

                //'--------------------------------------------------------------------------------
                //'   현재 진행하는 건이 첫 공정 이라면 공동이동전표 발행 
                //'--------------------------------------------------------------------------------
                // 첫 공정이다.
                if ((LabelPrintYN == "Y"))
                {
                    int mInstDetSeq = 0;
                    long nQtyPerBox = 0;
                    list_TWkLabelPrint = new List<Sub_TWkLabelPrint>();

                    // 잔량 불러오기 라벨 건이 있다면, 그 라벨도 인쇄 될 수 있도록.


                    for (int i = 0; i < TWkRCon; i++)
                    {
                        // 마지막 바퀴이고, 구별자가 YO면서, 나누기 잔량값이 있다면,
                        // 그건 split 저장도 함께/.
                        if ((i + 1 == TWkRCon) && (Split_GBN == "YO") && (LabelPaper_OneMoreQty > 0 || LabelPaper_Count == 0))
                        {
                            list_TWkResult_SplitAdd = new List<Sub_TWkResult_SplitAdd>();

                            list_TWkResult_SplitAdd.Add(new Sub_TWkResult_SplitAdd());
                            list_TWkResult_SplitAdd[0].JobID = list_TWkResult[i].JobID + i;
                            list_TWkResult_SplitAdd[0].SplitSeq = 1;
                            list_TWkResult_SplitAdd[0].WorkPersonID = Frm_tprc_Main.g_tBase.PersonID;

                            if (LabelPaper_Count == 0)
                            {
                                // 이 조건에 합격해서 들어왔다면,
                                list_TWkResult_SplitAdd[0].WorkQty = LabelPaper_Qty;
                            }
                            else
                            {
                                list_TWkResult_SplitAdd[0].WorkQty = LabelPaper_OneMoreQty;
                            }                            
                            list_TWkResult_SplitAdd[0].ScanDate = "";

                            list_TWkResult_SplitAdd[0].ScanTime = "";
                            list_TWkResult_SplitAdd[0].WorkStartDate = mtb_From.Text.Replace("-", "");
                            list_TWkResult_SplitAdd[0].WorkEndDate = mtb_To.Text.Replace("-", "");
                            list_TWkResult_SplitAdd[0].WorkStartTime = dtStartTime.Value.ToString("HHmmss");
                            list_TWkResult_SplitAdd[0].WorkEndTime = dtEndTime.Value.ToString("HHmmss");

                            list_TWkResult_SplitAdd[0].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;
                        }
                        
                        list_TWkLabelPrint.Add(new Sub_TWkLabelPrint());

                        list_TWkLabelPrint[i].sLabelID = "";
                        list_TWkLabelPrint[i].sProcessID = Frm_tprc_Main.g_tBase.ProcessID;

                        list_TWkLabelPrint[i].sArticleID = txtArticleID.Text;
                        list_TWkLabelPrint[i].sLabelGubun = "7";


                        list_TWkLabelPrint[i].sPrintDate = mtb_From.Text.Replace("-", "");

                        list_TWkLabelPrint[i].sReprintDate = "";
                        list_TWkLabelPrint[i].nReprintQty = 0;
                        list_TWkLabelPrint[i].sInstID = txtInstID.Text;

                        int.TryParse(Lib.GetDouble(txtInstDetSeq.Text).ToString(), out mInstDetSeq);
                        list_TWkLabelPrint[i].nInstDetSeq = mInstDetSeq;
                        list_TWkLabelPrint[i].sOrderID = m_OrderID;

                        list_TWkLabelPrint[i].nPrintQty = 1;

                        if (TWkRCon == 1)
                        {
                            // Update값 물고난 뒤에 한번만 발행하면 되는 케이스.
                            if ((Split_GBN == "YO" || Split_GBN == "YC") && (LabelPaper_OneMoreQty > 0))
                            {
                                long.TryParse(LabelPaper_OneMoreQty.ToString(), out nQtyPerBox);
                                list_TWkLabelPrint[i].nQtyPerBox = nQtyPerBox;
                            }
                            else
                            {
                                long.TryParse(LabelPaper_Qty.ToString(), out nQtyPerBox);
                                list_TWkLabelPrint[i].nQtyPerBox = nQtyPerBox;
                            }
                        }
                        else
                        {
                            if ((i + 1 == TWkRCon) && (Split_GBN == "YC" || Split_GBN == "YO") && (LabelPaper_OneMoreQty > 0))
                            {
                                long.TryParse(LabelPaper_OneMoreQty.ToString(), out nQtyPerBox);
                                list_TWkLabelPrint[i].nQtyPerBox = nQtyPerBox;
                            }
                            else
                            {
                                long.TryParse(LabelPaper_Qty.ToString(), out nQtyPerBox);
                                list_TWkLabelPrint[i].nQtyPerBox = nQtyPerBox;
                            }
                        }

                        list_TWkLabelPrint[i].sCreateuserID = Frm_tprc_Main.g_tBase.PersonID;
                        list_TWkLabelPrint[i].sLastUpdateUserID = Frm_tprc_Main.g_tBase.PersonID;
                                               
                    }
                }
                //

                //'-------------------------------------------------------------------------------
                //'생산실적  저장
                //'-------------------------------------------------------------------------------

                Success = AddNewWorkResult(iCnt, Frm_tprc_Main.g_tBase.DefectCnt);
                if (Success)
                {
                    //  첫 공정(DETSEQ가 1)   &&  MT_ARTICLE  LABELPRINTYN = Y  &&  1개 이상의 박스당 생산수
                    if ((LabelPrintYN == "Y") && (Wh_Ar_LabelPrintYN == "Y") && (LabelPaper_Count > 0))
                    {
                        if ((Split_GBN == "YC" && LabelPaper_OneMoreQty > 0))
                        {
                            PrintWorkCard((int)LabelPaper_Count + 1);
                        }
                        else
                        {
                            PrintWorkCard((int)LabelPaper_Count);
                        }

                    }
                    else
                    {
                        Message[0] = "[저장 완료]";
                        Message[1] = "저장이 완료되었습니다.";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                    }

                    Frm_tprc_Main.list_g_tsplit.Clear();
                    SetFormDataClear();
                    cmdExit_Click(null, null);  // 나가.
                }
                else
                {
                    throw new Exception();
                }
                //'    '-----------------------------------------------------------------------------------------
                //     '저장된 결과 재 조회
                //'    '-----------------------------------------------------------------------------------------
                //FillGridData1();
                Cursor = Cursors.Default;
            }

            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의<cmdSave>\r\n{0}", excpt.Message), "[오류]", 0, 1);
                Cursor = Cursors.Default;
            }
        }

        #endregion



        private void Fpas_WriteTextEvent(List<KeyValue> AddSaveResult)
        {
            foreach (KeyValue kv in AddSaveResult)
            {

            }
        }        

        //'생산 등록, 생산 하위품 등록, 생산 불량 등록한다
        private bool AddNewWorkResult(int nCnt, int nDefectCnt)
        {
            List<WizCommon.Procedure> Prolist = new List<WizCommon.Procedure>();
            List<List<string>> ListProcedureName = new List<List<string>>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();
            try
            {
                for (int i = 0; i < lstLabelList.Count; i++)
                {
                    // 일괄 스캔으로 가져온 실적들 세팅하기	
                    Sub_TWkResult BatchScan = list_TWkResult[0].Copy();
                    BatchScan.LabelID = lstLabelList[i];
                    BatchScan.WorkQty = lstQty[i];
                    BatchScan.Comments = BatchScan.ProcessID + " 일괄 실적 저장 : 외주공정 : " + m_LabelID;
                    list_TWkResult.Add(BatchScan);
                }
                // 값이 세팅한대로 나오는지 체크	
                //for (int i = 0; i < list_TWkResult.Count; i++)	
                //{	
                //    Console.WriteLine(list_TWkResult[i].LabelID);	
                //}	

                for (int i = 0; i < list_TWkResult.Count; i++)
                {
                    //'*****************************************************************************************************
                    //'                  공정이동전표 등록
                    //'*****************************************************************************************************
                    if (list_TWkLabelPrint.Count > i)
                    {
                        if (list_TWkLabelPrint[i].sProcessID != "")
                        {                            
                            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                            sqlParameter.Add("LabelID", list_TWkLabelPrint[i].sLabelID);
                            sqlParameter.Add("LabelGubun", list_TWkLabelPrint[i].sLabelGubun);
                            sqlParameter.Add("ProcessID", list_TWkLabelPrint[i].sProcessID);
                            sqlParameter.Add("ArticleID", list_TWkLabelPrint[i].sArticleID);
                            sqlParameter.Add("PrintDate", list_TWkLabelPrint[i].sPrintDate);

                            sqlParameter.Add("ReprintDate", list_TWkLabelPrint[i].sReprintDate);
                            sqlParameter.Add("ReprintQty", list_TWkLabelPrint[i].nReprintQty);
                            sqlParameter.Add("InstID", list_TWkLabelPrint[i].sInstID);
                            sqlParameter.Add("InstDetSeq", list_TWkLabelPrint[i].nInstDetSeq);
                            sqlParameter.Add("OrderID", list_TWkLabelPrint[i].sOrderID);

                            sqlParameter.Add("PrintQty", list_TWkLabelPrint[i].nPrintQty);
                            sqlParameter.Add("LabelPrintQty", 1);
                            sqlParameter.Add("nQtyPerBox", list_TWkLabelPrint[i].nQtyPerBox);                   
                            sqlParameter.Add("CreateUserID", list_TWkLabelPrint[i].sCreateuserID);

                            WizCommon.Procedure pro1 = new WizCommon.Procedure();
                            pro1.Name = "[xp_WizWork_iwkLabelPrint_C]";
                            pro1.OutputUseYN = "Y";
                            pro1.OutputName = "LabelID";
                            pro1.OutputLength = "20";

                            Prolist.Add(pro1);
                            ListParameter.Add(sqlParameter);                            
                        }
                    }
                    //'************************************************************************************************
                    //'                               상위품 생산 //xp_wkResult_iWkResult
                    //'************************************************************************************************
                    Dictionary<string, object> sqlParameter1 = new Dictionary<string, object>();
                    WizCommon.Procedure pro2 = new WizCommon.Procedure();

                    if (i == 0)
                    {
                        sqlParameter1.Add("JobID", 0);
                        sqlParameter1.Add("InstID", list_TWkResult[i].InstID);
                        sqlParameter1.Add("InstDetSeq", list_TWkResult[i].InstDetSeq);

                        if (list_TWkLabelPrint.Count > i)
                        {
                            sqlParameter1.Add("LabelID", list_TWkLabelPrint[i].sLabelID);
                        }
                        else
                        {
                            sqlParameter1.Add("LabelID", list_TWkResult[i].LabelID);
                        }
                        sqlParameter1.Add("StartSaveLabelID", txtPreInsertLabelBarCode.Text);
                        sqlParameter1.Add("LabelGubun", list_TWkResult[i].LabelGubun);
                        sqlParameter1.Add("ProcessID", list_TWkResult[i].ProcessID);
                        sqlParameter1.Add("MachineID", list_TWkResult[i].MachineID);
                        sqlParameter1.Add("ScanDate", list_TWkResult[i].ScanDate);
                        sqlParameter1.Add("ScanTime", list_TWkResult[i].ScanTime);

                        sqlParameter1.Add("ArticleID", list_TWkResult[i].ArticleID);
                        sqlParameter1.Add("WorkQty", list_TWkResult[i].WorkQty);
                        sqlParameter1.Add("Comments", list_TWkResult[i].Comments);
                        sqlParameter1.Add("ReworkOldYN", list_TWkResult[i].ReworkOldYN);
                        sqlParameter1.Add("ReworkLinkProdID", list_TWkResult[i].ReworkLinkProdID);

                        sqlParameter1.Add("WorkStartDate", list_TWkResult[i].WorkStartDate);
                        sqlParameter1.Add("WorkStartTime", list_TWkResult[i].WorkStartTime);
                        sqlParameter1.Add("WorkEndDate", list_TWkResult[i].WorkEndDate);
                        sqlParameter1.Add("WorkEndTime", list_TWkResult[i].WorkEndTime);
                        sqlParameter1.Add("JobGbn", list_TWkResult[i].JobGbn);

                        sqlParameter1.Add("NoReworkCode", list_TWkResult[i].sNowReworkCode);
                        sqlParameter1.Add("WDNO", list_TWkResult[i].WDNO);
                        sqlParameter1.Add("WDID", list_TWkResult[i].WDID);
                        sqlParameter1.Add("WDQty", list_TWkResult[i].WDQty);
                        sqlParameter1.Add("LogID", list_TWkResult[i].sLogID);

                        sqlParameter1.Add("s4MID", list_TWkResult[i].s4MID);
                        sqlParameter1.Add("DayOrNightID", list_TWkResult[i].DayOrNightID);
                        sqlParameter1.Add("SplitYNGBN", list_TWkResult[i].SplitYNGBN);
                        sqlParameter1.Add("CycleTime", list_TWkResult[i].CycleTime);
                        sqlParameter1.Add("CreateUserID", list_TWkResult[i].CreateUserID);

                        //pro2.Name = "xp_wkResult_uWkResultOne";
                        //pro2.Name = "xp_WizWork_uWkResultOne";//하이테크만 이렇게 사용 // 2020.02.21 둘리 : CycleTime 추가되도록 수정
                        pro2.Name = "xp_wkResult_iWkResult";
                        pro2.OutputUseYN = "Y";
                        pro2.OutputName = "JobID";
                        pro2.OutputLength = "20";
                    }
                    else
                    {
                        sqlParameter1.Add("JobID", 0);
                        sqlParameter1.Add("InstID", list_TWkResult[i].InstID);
                        sqlParameter1.Add("InstDetSeq", list_TWkResult[i].InstDetSeq);
                        if (list_TWkLabelPrint.Count > i)
                        {
                            sqlParameter1.Add("LabelID", list_TWkLabelPrint[i].sLabelID);
                        }
                        else
                        {
                            sqlParameter1.Add("LabelID", list_TWkResult[i].LabelID);
                        }
                        // 일괄 스캔 → 스타트 라벨 세팅을 어떻게 하지?	
                        if (i > 0 && lstLabelList.Count > 0)
                        {
                            sqlParameter1.Add("StartSaveLabelID", lstLabelList[i - 1]);
                        }
                        else
                        {
                            sqlParameter1.Add("StartSaveLabelID", txtPreInsertLabelBarCode.Text);
                        }
                        //sqlParameter1.Add("StartSaveLabelID", txtPreInsertLabelBarCode.Text);

                        sqlParameter1.Add("LabelGubun", list_TWkResult[i].LabelGubun);
                        sqlParameter1.Add("ProcessID", list_TWkResult[i].ProcessID);
                        sqlParameter1.Add("MachineID", list_TWkResult[i].MachineID);
                        sqlParameter1.Add("ScanDate", list_TWkResult[i].ScanDate);
                        sqlParameter1.Add("ScanTime", list_TWkResult[i].ScanTime);

                        sqlParameter1.Add("ArticleID", list_TWkResult[i].ArticleID);
                        sqlParameter1.Add("WorkQty", list_TWkResult[i].WorkQty);
                        sqlParameter1.Add("Comments", list_TWkResult[i].Comments);
                        sqlParameter1.Add("ReworkOldYN", list_TWkResult[i].ReworkOldYN);
                        sqlParameter1.Add("ReworkLinkProdID", list_TWkResult[i].ReworkLinkProdID);

                        sqlParameter1.Add("WorkStartDate", list_TWkResult[i].WorkStartDate);
                        sqlParameter1.Add("WorkStartTime", list_TWkResult[i].WorkStartTime);
                        sqlParameter1.Add("WorkEndDate", list_TWkResult[i].WorkEndDate);
                        sqlParameter1.Add("WorkEndTime", list_TWkResult[i].WorkEndTime);
                        sqlParameter1.Add("JobGbn", list_TWkResult[i].JobGbn);

                        sqlParameter1.Add("NoReworkCode", list_TWkResult[i].sNowReworkCode);
                        sqlParameter1.Add("WDNO", list_TWkResult[i].WDNO);
                        sqlParameter1.Add("WDID", list_TWkResult[i].WDID);
                        sqlParameter1.Add("WDQty", list_TWkResult[i].WDQty);
                        sqlParameter1.Add("LogID", list_TWkResult[i].sLogID);

                        sqlParameter1.Add("s4MID", list_TWkResult[i].s4MID);                        
                        sqlParameter1.Add("DayOrNightID", list_TWkResult[i].DayOrNightID);
                        sqlParameter1.Add("SplitYNGBN", list_TWkResult[i].SplitYNGBN);
                        sqlParameter1.Add("CycleTime", list_TWkResult[i].CycleTime);
                        sqlParameter1.Add("CreateUserID", list_TWkResult[i].CreateUserID);

                        pro2.Name = "xp_wkResult_iWkResult";
                        pro2.OutputUseYN = "Y";
                        pro2.OutputName = "JobID";
                        pro2.OutputLength = "20";
                    }

                    Prolist.Add(pro2);
                    ListParameter.Add(sqlParameter1);

                    //'****************************************************************************************************
                    //'정상작업의 경우    Sub_TWkResult.JobGbn = "1"
                    //'****************************************************************************************************
                    if (list_TWkResult[0].JobGbn == "1")
                    {
                        //'************************************************************************************************
                        //'                               하위품 스켄이력 등록
                        //'************************************************************************************************
                        
                        if (nCnt > 0)
                        {
                            for (int k = 0; k < nCnt; k++)
                            {
                                Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();

                                if (i == 0)
                                {
                                    sqlParameter2.Add("JobID", list_TWkResult[i].JobID);
                                }
                                else
                                {
                                    sqlParameter2.Add("JobID", 0);
                                }

                                // 일괄 스캔 → 하위품 스캔이력도 세팅을 어떻게 하지?	
                                if (i > 0 && lstLabelList.Count > 0)
                                {
                                    sqlParameter2.Add("ChildLabelID", lstLabelList[i - 1]);//	
                                }
                                else
                                {
                                    sqlParameter2.Add("ChildLabelID", list_TWkResultArticleChild[k].ChildLabelID);//	
                                }

                                sqlParameter2.Add("ChildLabelGubun", list_TWkResultArticleChild[k].ChildLabelGubun);
                                sqlParameter2.Add("ChildArticleID", list_TWkResultArticleChild[k].ChildArticleID);
                                sqlParameter2.Add("ReworkOldYN", list_TWkResultArticleChild[k].ReworkOldYN);
                                sqlParameter2.Add("ReworkLinkChildProdID", list_TWkResultArticleChild[k].ReworkLinkChildProdID);
                                sqlParameter2.Add("CreateUserID", list_TWkResultArticleChild[k].CreateUserID);

                                WizCommon.Procedure pro3 = new WizCommon.Procedure();
                                pro3.Name = "xp_wkResult_iWkResultArticleChild";
                                pro3.OutputUseYN = "N";
                                pro3.OutputName = "JobID";
                                pro3.OutputLength = "20";

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter2);

                            }
                        }

                        // '************************************************************************************************
                        //'               한 박스 덜채운 작업분량 임시저장고  //xp_wkResult_iwkresult Split
                        //'************************************************************************************************

                        // 여유분 split 에 넣어야 할 데이터가 존재한다면,
                        // (YO 타입이면서, 잔량 QTY가 존재한다)
                        if (list_TWkResult_SplitAdd.Count > 0)
                        {
                            // 마지막 바퀴째에 넣어야 한다.
                            if (i + 1 == list_TWkResult.Count)
                            {
                                string InstID = txtInstID.Text;
                                int InstDetSeq = 0;
                                int.TryParse(Lib.GetDouble(txtInstDetSeq.Text).ToString(), out InstDetSeq);

                                Dictionary<string, object> sqlParameter3 = new Dictionary<string, object>();

                                if (i == 0)
                                {
                                    sqlParameter3.Add("JobID", list_TWkResult[i].JobID);
                                }
                                else
                                {
                                    sqlParameter3.Add("JobID", 0);
                                }
                                 
                                sqlParameter3.Add("SplitSeq", list_TWkResult_SplitAdd[0].SplitSeq);
                                sqlParameter3.Add("WorkPersonID", list_TWkResult_SplitAdd[0].WorkPersonID);
                                sqlParameter3.Add("WorkQty", list_TWkResult_SplitAdd[0].WorkQty);
                                sqlParameter3.Add("ScanDate", list_TWkResult_SplitAdd[0].ScanDate);

                                sqlParameter3.Add("ScanTime", list_TWkResult_SplitAdd[0].ScanTime);
                                sqlParameter3.Add("WorkStartDate", list_TWkResult_SplitAdd[0].WorkStartDate);
                                sqlParameter3.Add("WorkEndDate", list_TWkResult_SplitAdd[0].WorkEndDate);
                                sqlParameter3.Add("WorkStartTime", list_TWkResult_SplitAdd[0].WorkStartTime);
                                sqlParameter3.Add("WorkEndTime", list_TWkResult_SplitAdd[0].WorkEndTime);

                                sqlParameter3.Add("DayOrNightID", Wh_Ar_DayOrNightID);
                                sqlParameter3.Add("MachineID", Frm_tprc_Main.g_tBase.MachineID);
                                sqlParameter3.Add("CycleTime", Lib.GetDouble(txtCycleTime.Text));
                                sqlParameter3.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                                sqlParameter3.Add("InstDetSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);

                                sqlParameter3.Add("CreateUserID", list_TWkResult_SplitAdd[0].CreateUserID);

                                WizCommon.Procedure pro4 = new WizCommon.Procedure();
                                pro4.Name = "xp_wkResult_iWkResult_Split";
                                pro4.OutputUseYN = "N";
                                pro4.OutputName = "JobID";
                                pro4.OutputLength = "20";

                                Prolist.Add(pro4);
                                ListParameter.Add(sqlParameter3);
                            }

                        }

                        // '************************************************************************************************
                        //'                              불량 등록 시   //xp_wkResult_iInspect
                        //'************************************************************************************************
                        if (i == 0)
                        {
                            //if (nDefectCnt > 0)
                            //{
                            //    for (int k = 0; k < nDefectCnt; k++)
                            //    {
                            //        Dictionary<string, object> sqlParameter4 = new Dictionary<string, object>();

                            //        sqlParameter4.Add("WkDefectID", "");
                            //        sqlParameter4.Add("OrderID", Frm_tprc_Main.list_g_tInsSub[k].OrderID);
                            //        sqlParameter4.Add("OrderSeq", Frm_tprc_Main.list_g_tInsSub[k].OrderSeq);
                            //        sqlParameter4.Add("ProcessID", Frm_tprc_Main.list_g_tInsSub[k].ProcessID);
                            //        sqlParameter4.Add("MachineID", Frm_tprc_Main.list_g_tInsSub[k].MachineID);

                            //        sqlParameter4.Add("DefectQty", Frm_tprc_Main.list_g_tInsSub[k].nDefectQty);
                            //        sqlParameter4.Add("BoxID", Frm_tprc_Main.list_g_tInsSub[k].BoxID);
                            //        sqlParameter4.Add("DefectID", Frm_tprc_Main.list_g_tInsSub[k].DefectID);
                            //        sqlParameter4.Add("XPos", Frm_tprc_Main.list_g_tInsSub[k].XPos);
                            //        sqlParameter4.Add("YPos", Frm_tprc_Main.list_g_tInsSub[k].YPos);

                            //        sqlParameter4.Add("InspectDate", Frm_tprc_Main.list_g_tInsSub[k].InspectDate);
                            //        sqlParameter4.Add("InspectTime", Frm_tprc_Main.list_g_tInsSub[k].InspectTime);
                            //        sqlParameter4.Add("PersonID", list_TWkResult[0].CreateUserID);

                            //        // 둘리
                            //        // 일괄 스캔일 경우 선스캔 라벨에                                    
                            //        if (lstLabelList.Count > 0)
                            //        {
                            //            sqlParameter4.Add("JobID", list_TWkResult[0].JobID);
                            //        }
                            //        else  // 나머지는 마지막 jobid로 세팅
                            //        {
                            //            sqlParameter4.Add("JobID", list_TWkResult[list_TWkResult.Count - 1].JobID);
                            //        }

                            //        sqlParameter4.Add("CreateUserID", list_TWkResult[0].CreateUserID);

                            //        WizCommon.Procedure pro5 = new WizCommon.Procedure();
                            //        pro5.Name = "xp_wkResult_iInspect";
                            //        pro5.OutputUseYN = "N";
                            //        pro5.OutputName = "JobID";
                            //        pro5.OutputLength = "20";

                            //        Prolist.Add(pro5);
                            //        ListParameter.Add(sqlParameter4);
                            //        //ProcedureInfo = null;
                            //    }
                            //}

                            foreach (string Key in dicDefect.Keys)
                            {
                                var Defect = dicDefect[Key] as frm_tprc_Work_Defect_U_CodeView;
                                if (Defect != null)
                                {
                                    Dictionary<string, object> sqlParameter4 = new Dictionary<string, object>();

                                    sqlParameter4.Add("DefectID", Key.Trim());
                                    sqlParameter4.Add("DefectQty", ConvertDouble(Defect.DefectQty));
                                    sqlParameter4.Add("XPos", ConvertInt(Defect.XPos));
                                    sqlParameter4.Add("YPos", ConvertInt(Defect.YPos));
                                    sqlParameter4.Add("JobID", list_TWkResult[0].JobID);
                                    sqlParameter4.Add("CreateUserID", list_TWkResult[0].CreateUserID);

                                    WizCommon.Procedure pro4 = new WizCommon.Procedure();
                                    pro4.Name = "xp_prdWork_iWorkDefect";
                                    pro4.OutputUseYN = "N";
                                    pro4.OutputName = "JobID";
                                    pro4.OutputLength = "20";

                                    Prolist.Add(pro4);
                                    ListParameter.Add(sqlParameter4);
                                }
                            }
                        }
                                                

                        //'************************************************************************************************
                        //'                            생산제품 재고 생성 및 하품 자재 출고 처리  //xp_wkResult_iWkResultStuffInOut
                        //'************************************************************************************************
                        //if (m_ProcessID != "2101" || (m_ProcessID == "2101" && blSHExit))
                        ////성형공정이 아니거나 또는 , 성형공정이면서 작업종료 시점일때만 입력
                        //{

                        Dictionary<string, object> sqlParameter5 = new Dictionary<string, object>();

                        if (i == 0)
                        {
                            sqlParameter5.Add("JobID", list_TWkResult[i].JobID);
                        }
                        else
                        {
                            sqlParameter5.Add("JobID", 0);
                        }

                        sqlParameter5.Add("CreateUserID", list_TWkResult[i].CreateUserID);
                        sqlParameter5.Add("sRtnMsg", "");
                        

                        WizCommon.Procedure pro6 = new WizCommon.Procedure();
                        pro6.Name = "xp_wkResult_iWkResultStuffInOut";     // xp_wkResult_iWkResultStuffInOut_20210526_TEST
                        pro6.OutputUseYN = "N";
                        pro6.OutputName = "JobID";
                        pro6.OutputLength = "20";

                        Prolist.Add(pro6);
                        ListParameter.Add(sqlParameter5);

                        //}
                    }
                }
                //'************************************************************************************************
                //'                           사용 금형 등록
                //'************************************************************************************************ 
                if (list_TMold != null)
                {
                    if (m_ProcessID != "2101" || (m_ProcessID == "2101" && blSHExit))
                    //성형공정이 아니거나 또는 , 성형공정이면서 작업종료 시점일때만 입력
                    {
                        //if (list_TMold.Count > 0)
                        //{
                        //    for (int j = 0; j < Frm_tprc_Main.list_tMold.Count; j++)
                        //    {
                        //        Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                        //        sqlParameter.Add("JobID", list_TWkResult[j].JobID);
                        //        sqlParameter.Add("MoldID", list_TMold[j].sMoldID/*Frm_tprc_Main.list_tMold[j].sMoldID*/);
                        //        sqlParameter.Add("RealCavity", list_TMold[j].sRealCavity/*Frm_tprc_Main.list_tMold[j].sRealCavity*/);
                        //        sqlParameter.Add("HitCount", list_TMold[j].sHitCount/*list_TWkResult[i].WorkQty*/);
                        //        sqlParameter.Add("CreateUserID", list_TWkResult[j].CreateUserID);

                        //        WizCommon.Procedure pro_2 = new WizCommon.Procedure();
                        //        pro_2.Name = "xp_wkResult_iwkResultMold";
                        //        pro_2.OutputUseYN = "N";
                        //        pro_2.OutputName = "JobID";
                        //        pro_2.OutputLength = "20";

                        //        Prolist.Add(pro_2);
                        //        ListParameter.Add(sqlParameter);
                        //    }
                        //}
                    }

                }

                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputToCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {
                    list_Result.RemoveAt(0);

                    //int rsCount = list_Result.Count / 2;//2 = Output갯수(JobID, LabelID)
                    int a = 0;
                    for (int i = 0; i < list_Result.Count; i++)
                    {
                        KeyValue kv = list_Result[i];
                        if (kv.key == "LabelID")
                        {
                            list_TWkLabelPrint[a++].sLabelID = kv.value;
                            //list_TWkLabelPrint[i].sLabelID = kv.value;
                        }
                        //else if (kv.key == "JobID")
                        //{
                        //    //list_TWkResult[i / 2].JobID = float.Parse(kv.value);
                        //    list_TWkResult[i].JobID = float.Parse(kv.value);
                        //}
                    }
                    return true;
                }
                else
                {
                    foreach (KeyValue kv in list_Result)
                    {
                        if (kv.key.ToLower() == "failure")
                        {
                            throw new Exception(kv.value.ToString());
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
                return false;
            }

        }



        private bool Main_TSplit_ConnectionEvent()
        {
            try
            {
                // 저장하려 하는데, Main_TSplit 카운트가 있다.
                // 즉, 앞선 작업자의 데이터를 더해 쓰려고 한다.
                // 그때, 먼저 이루어져야 하는 작업 리스트 모음입니다.
                // 2020.03.10  허윤구.. (난 이게 한계야...미안해.ㅠㅠ)

                //1. 가져와서 쓰려 하는 리스트의 남은 잔량만큼 같은 JobID로 한번 더 Insert.
                //  Split의 JobID로 Groupby 하면 생산박스 수량만큼 두명의 작업자가 뜨게끔.

                list_TWkResult_SplitAdd = new List<Sub_TWkResult_SplitAdd>();

                for (int i = 0; i < Frm_tprc_Main.list_g_tsplit.Count; i++)
                {
                    list_TWkResult_SplitAdd.Add(new Sub_TWkResult_SplitAdd());
                    list_TWkResult_SplitAdd[i].JobID = Frm_tprc_Main.list_g_tsplit[i].JobID;
                    list_TWkResult_SplitAdd[i].SplitSeq = 2;
                    list_TWkResult_SplitAdd[i].WorkPersonID = Frm_tprc_Main.g_tBase.PersonID;
                    list_TWkResult_SplitAdd[i].WorkQty = Lib.GetDouble(txtLotProdQty.Text) - Frm_tprc_Main.list_g_tsplit[i].Qty;
                    //list_TWkResult_SplitAdd[i].WorkQty = Lib.GetDouble(txtLotProdQty.Text) - Lib.GetDouble(txtWorkQty.Text);
                    list_TWkResult_SplitAdd[i].ScanDate = "";

                    list_TWkResult_SplitAdd[i].ScanTime = "";
                    list_TWkResult_SplitAdd[i].WorkStartDate = mtb_From.Text.Replace("-", "");
                    list_TWkResult_SplitAdd[i].WorkEndDate = mtb_To.Text.Replace("-", "");
                    list_TWkResult_SplitAdd[i].WorkStartTime = dtStartTime.Value.ToString("HHmmss");
                    list_TWkResult_SplitAdd[i].WorkEndTime = dtEndTime.Value.ToString("HHmmss");

                    list_TWkResult_SplitAdd[i].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;

                    //////////////////////////////////////////////////////////////////////////////////////
                    ///

                }

                List<WizCommon.Procedure> Prolist = new List<WizCommon.Procedure>();
                List<List<string>> ListProcedureName = new List<List<string>>();
                List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

                for (int i = 0; i < list_TWkResult_SplitAdd.Count; i++)
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                    string InstID = txtInstID.Text;
                    int InstDetSeq = 0;
                    int.TryParse(Lib.GetDouble(txtInstDetSeq.Text).ToString(), out InstDetSeq);

                    sqlParameter.Add("JobID", list_TWkResult_SplitAdd[i].JobID);
                    sqlParameter.Add("SplitSeq", list_TWkResult_SplitAdd[i].SplitSeq);
                    sqlParameter.Add("WorkPersonID", list_TWkResult_SplitAdd[i].WorkPersonID);
                    sqlParameter.Add("WorkQty", list_TWkResult_SplitAdd[i].WorkQty);
                    sqlParameter.Add("ScanDate", list_TWkResult_SplitAdd[i].ScanDate);

                    sqlParameter.Add("ScanTime", list_TWkResult_SplitAdd[i].ScanTime);
                    sqlParameter.Add("WorkStartDate", list_TWkResult_SplitAdd[i].WorkStartDate);
                    sqlParameter.Add("WorkEndDate", list_TWkResult_SplitAdd[i].WorkEndDate);
                    sqlParameter.Add("WorkStartTime", list_TWkResult_SplitAdd[i].WorkStartTime);
                    sqlParameter.Add("WorkEndTime", list_TWkResult_SplitAdd[i].WorkEndTime);

                    sqlParameter.Add("DayOrNightID", Wh_Ar_DayOrNightID);
                    sqlParameter.Add("MachineID", Frm_tprc_Main.g_tBase.MachineID);
                    sqlParameter.Add("CycleTime", Lib.GetDouble(txtCycleTime.Text));
                    sqlParameter.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                    sqlParameter.Add("InstDetSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);

                    sqlParameter.Add("CreateUserID", list_TWkResult_SplitAdd[i].CreateUserID);

                    WizCommon.Procedure pro = new WizCommon.Procedure();
                    pro.Name = "xp_wkResult_iWkResult_Split";
                    pro.OutputUseYN = "N";
                    pro.OutputName = "JobID";
                    pro.OutputLength = "20";

                    Prolist.Add(pro);
                    ListParameter.Add(sqlParameter);


                    // 2. TSPLIT 리스트의 JobID를 기준으로,
                    //  기존에 넣어 둔 부족불량의 WK_RESULT와 LABEL의 QTY를 Update 쳐야 합니다. (+) split의 useclss = *
                    //  생산박스 수량과 동일해야 하는데, 이론상으로는 지금 넣는 splitqty + 기 투입량을 하면, prod_qtyperbox.
                    Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();
                    sqlParameter2.Add("JobID", list_TWkResult_SplitAdd[i].JobID);
                    sqlParameter2.Add("WorkQty", list_TWkResult_SplitAdd[i].WorkQty);

                    WizCommon.Procedure pro2 = new WizCommon.Procedure();
                    pro2.Name = "xp_WizWork_uSplit_ConnectionQty";
                    pro2.OutputUseYN = "N";
                    pro2.OutputName = "JobID";
                    pro2.OutputLength = "20";

                    Prolist.Add(pro2);
                    ListParameter.Add(sqlParameter2);



                    // 3. 기존에 적은분량으로 들어간 JobID의 StuffinOut 기록을 제거하고,
                    Dictionary<string, object> sqlParameter3 = new Dictionary<string, object>();
                    sqlParameter3.Add("JobID", list_TWkResult_SplitAdd[i].JobID);

                    WizCommon.Procedure pro3 = new WizCommon.Procedure();
                    pro3.Name = "xp_WizWork_dStuffinOut_SplitBasicData";
                    pro3.OutputUseYN = "N";
                    pro3.OutputName = "JobID";
                    pro3.OutputLength = "20";

                    Prolist.Add(pro3);
                    ListParameter.Add(sqlParameter3);



                    //  4. 다시 stuffinout 타기.
                    // prod_qtyperbox의 수가 나오도록 업데이트 쳤으니까, 그 수에 맞는 stuffinout으로 다시타기.
                    Dictionary<string, object> sqlParameter4 = new Dictionary<string, object>();

                    sqlParameter4.Add("JobID", list_TWkResult_SplitAdd[i].JobID);
                    sqlParameter4.Add("CreateUserID", list_TWkResult_SplitAdd[i].CreateUserID);
                    sqlParameter4.Add("sRtnMsg", "");

                    WizCommon.Procedure pro4 = new WizCommon.Procedure();
                    pro4.Name = "xp_wkResult_iWkResultStuffInOut";
                    pro4.OutputUseYN = "N";
                    pro4.OutputName = "JobID";
                    pro4.OutputLength = "20";

                    Prolist.Add(pro4);
                    ListParameter.Add(sqlParameter4);


                }

                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputToCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {
                    list_Result.RemoveAt(0);
                    return true;
                }
                else
                {
                    foreach (KeyValue kv in list_Result)
                    {
                        if (kv.key.ToLower() == "failure")
                        {
                            throw new Exception(kv.value.ToString());
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
                return false;
            }                  
        }






        private void GetMtrChileLotRemainQty(string strBoxID, string strProcessID, string ProdQty)
        {
            DataSet ds = null;
            DataRow dr = null;
            int iChildQty = 0;
            string[] ChildArticleID = null;
            string[] ChildLotID = null;
            string[] StuffinRemainQty = null;
            string[] ChildRnk = null;
            string[] UnitClss = null;
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Add("sParentLotID", strBoxID);//상위품ID
                sqlParameter.Add("ProcessID", strProcessID);
                if (m_ProcessID == "0405")//마지막 스캔한 바코드 기준의 UnitClss
                {
                    double douProdQty = 0;
                    double.TryParse(ProdQty, out douProdQty);
                    douProdQty = douProdQty / 1000;
                    sqlParameter.Add("nWorkQty", douProdQty);
                    sqlParameter.Add("UnitClss", 1);
                }
                else
                {
                    sqlParameter.Add("nWorkQty", ProdQty);
                    sqlParameter.Add("UnitClss", 2);
                }

                ds = DataStore.Instance.ProcedureToDataSet("[xp_WizWork_ChkChildLotQty]", sqlParameter, false);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    int Count = ds.Tables[0].Rows.Count;

                    ChildArticleID = new string[Count];
                    ChildLotID = new string[Count];
                    StuffinRemainQty = new string[Count];
                    ChildRnk = new string[Count];
                    UnitClss = new string[Count];
                    double douStuffinRemainQty = 0;
                    double douReqQty = 0;
                    double douProdQty = 0;

                    for (int i = 0; i < Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];

                        iChildQty++;
                        ChildArticleID[i] = dr["ChildArticleID"].ToString();
                        ChildLotID[i] = dr["ChildLotID"].ToString();
                        StuffinRemainQty[i] = dr["StuffinRemainQty"].ToString();
                        ChildRnk[i] = dr["Rnk"].ToString();
                        UnitClss[i] = dr["UnitClss"].ToString();
                    }

                    for (int i = 0; i < GridData2.Rows.Count; i++)
                    {
                        if (GridData2.Rows[i].Cells["ScanExceptYN1"].Value.ToString() == "N")
                        {
                            for (int j = 0; j < iChildQty - 1; j++)
                            {
                                if (GridData2.Rows[i].Cells["ChildArticleID"].Value.ToString() == ChildArticleID[j])
                                {
                                    if (ChildRnk[j] == "1" && GridData2.Rows[i].Cells["BarCode"].Value.ToString().Trim().ToUpper() == ChildLotID[j].Trim().ToUpper())
                                    {
                                        double.TryParse(StuffinRemainQty[j], out douStuffinRemainQty);
                                        double.TryParse(GridData2.Rows[i].Cells["ReqQty"].Value.ToString(), out douReqQty);
                                        double.TryParse(ProdQty, out douProdQty);

                                        if (GridData2.Rows[i].Cells["UnitClss"].Value.ToString() == UnitClss[j])//단위가 같을때
                                        {
                                            if (douStuffinRemainQty < douProdQty * douReqQty)
                                            {
                                                throw new Exception("생산수량이 현투입 자재량을 초과합니다.");
                                            }
                                        }
                                        else//단위가 다를때
                                        {
                                            if (UnitClss[j] == "1")//재고 단위 g
                                            {
                                                if (GridData2.Rows[i].Cells["UnitClss"].Value.ToString() == "2")//하위품 단위 kg
                                                {
                                                    //재고량에 나누기 1000을 해서 kg으로 고쳐서 계산한다.
                                                    if (douStuffinRemainQty / 1000 < douProdQty * douReqQty)
                                                    {
                                                        throw new Exception("생산수량이 현투입 자재량을 초과합니다.");
                                                    }
                                                }
                                                else//하위품의 단위가 kg, g이 아닐때
                                                {
                                                    throw new Exception("선입선출에 위배 되었습니다.");
                                                }
                                            }
                                            else if (UnitClss[j] == "2")//재고 단위 kg
                                            {
                                                if (GridData2.Rows[i].Cells["UnitClss"].Value.ToString() == "1")//하위품 단위 kg
                                                {
                                                    //하위품에 나누기 1000을 해서 kg으로 고쳐서 계산한다.
                                                    if (douStuffinRemainQty < douProdQty * douReqQty / 1000)
                                                    {
                                                        throw new Exception("생산수량이 현투입 자재량을 초과합니다.");
                                                    }
                                                }
                                                else//하위품의 단위가 kg, g이 아닐때
                                                {
                                                    throw new Exception("선입선출에 위배 되었습니다.");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GridData2.Rows[i].Cells["BarCode"].Value.ToString().Trim().ToUpper() != ChildLotID[j].Trim().ToUpper())
                                        {
                                            throw new Exception("선입선출에 위배 되었습니다. \r\n 해당부품의 출고대상은 LOT ID는 " + ChildLotID[j] + "입니다.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의<GetMtrChileLotRemainQty>\r\n{0}", excpt.Message), "[오류]", 0, 1);
                return;
            }
        }
        /// <summary>
        /// '공정이동전표의 정보 가져오기
        /// </summary>
        /// <param name="strLotID"></param>
        /// <param name="strProcessID"></param>
        /// <param name="strMachineID"></param>
        /// <param name="strMoldIDList"></param>
        private void GetWorkLotInfo(string strLotID, string strProcessID, string strMachineID, string strMoldIDList)
        {

            DataSet ds = null;
            DataRow dr = null;
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Add("PLotID", strLotID);
                sqlParameter.Add("ProcessID", strProcessID);
                sqlParameter.Add("MachineID", strMachineID);
                sqlParameter.Add("MoldIDList", strMoldIDList);

                ds = DataStore.Instance.ProcedureToDataSet("xp_wkresult_sWorkLotID", sqlParameter, false);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    dr = ds.Tables[0].Rows[0];
                    double InstQty = 0;
                    double WorkQty = 0;
                    double InstRemainQty = 0;
                    double.TryParse(Lib.GetDouble(dr["InstQty"].ToString()).ToString(), out InstQty);
                    double.TryParse(Lib.GetDouble(dr["WorkQty"].ToString()).ToString(), out WorkQty);
                    InstRemainQty = InstQty - WorkQty;
                    sTdGbn = Lib.CheckNull(dr["TdGbn"].ToString());
                    txtlInstQty.Text = string.Format("{0:n3}", InstQty);
                    txtInstRemainQty.Text = string.Format("{0:n3}", InstRemainQty);

                    if (Frm_tprc_Main.list_tMold.Count > 0)
                    {
                        string strMoldIDCheck = "";
                        strMoldIDCheck = dr["MoldIDCheck"].ToString();
                        if (!(int.Parse(Lib.CheckNum(strMoldIDCheck)) == Frm_tprc_Main.list_tMold.Count))
                        {
                            Message[0] = "[금형 오류]";
                            Message[1] = "선택된 금형은 이 품목의 금형이 아닙니다.";
                            WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                            return;
                        }
                        // 'AFT 는 금형 Cavity 가 1 이므로 쓰고 다른 업체일 경우 변경 필요 >> ?? 이해안감..
                        double douHitCount = 0;
                        double douRealCavity = 0;
                        double douWorkQty = 0;
                        double douSafeHitCount = 0;
                        for (int i = 0; i < Frm_tprc_Main.list_tMold.Count; i++)
                        {
                            double.TryParse(Lib.GetDouble(Frm_tprc_Main.list_tMold[i].sHitCount.ToString()).ToString(), out douHitCount);
                            double.TryParse(Lib.GetDouble(Frm_tprc_Main.list_tMold[i].sRealCavity.ToString()).ToString(), out douRealCavity);                            
                            double.TryParse(Lib.GetDouble(Frm_tprc_Main.list_tMold[i].sSafeHitCount.ToString()).ToString(), out douSafeHitCount);

                            if (douHitCount /*+ douWorkQty / douRealCavity */> douSafeHitCount)
                            {
                                Message[0] = "[금형 오류]";
                                Message[1] = "선택된 금형 중 생산 진행 시 타발수가 한계수명을 넘어가는 금형이 있습니다. (" + Frm_tprc_Main.list_tMold[i].sLotNo + ")";
                                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의<GetWorkLotInfo>\r\n{0}", excpt.Message), "[오류]", 0, 1);
            }
        }

        private bool CheckID(string strBoxID)
        {
            try
            {
                if (!BarCodeCheck(strBoxID))
                {
                    throw new Exception();
                }
                if (GridData2.Rows.Count > 0)
                {
                    foreach (DataGridViewRow dgvr in GridData2.Rows)
                    {
                        if (dgvr.Cells["BarCode"].Value.ToString().Trim() == "")
                        {
                            Message[0] = "[하위품 체크 오류]";
                            Message[1] = dgvr.Cells["Article"].Value.ToString() + "\r\n" + "하위품이 선택되지 않았습니다. 하위품을 스캔해주십시오.";
                            throw new Exception();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                return false;
            }
        }

        private string GetItemText(int inti)
        {
            string sText = "";
            int nLen = 0;
            string sChar = "";
            int iIdx = 0;
            string sTempText = "";
            string sTempChar = "";
            if (list_m_tItem.Count > 0)
            {
                //list_m_tItem[inti]
                sText = m_sData[list_m_tItem[inti].nRelation].Trim();
                nLen = Strings.Len(Strings.StrConv(sText, VbStrConv.Narrow, 0));

                if (list_m_tItem[inti].nSpace == 0)
                {
                    sChar = " ";
                }
                else if (list_m_tItem[inti].nSpace == 1)
                {
                    sChar = "0";
                }
                if (list_m_tItem[inti].nAlign == 0)
                {
                    if (nLen > list_m_tItem[inti].nLength)
                    {
                        iIdx = 1;
                        sTempText = "";
                        for (int i = 1; i < Math.Abs(list_m_tItem[inti].nLength); i++)
                        {
                            sTempChar = Strings.Mid(sText, iIdx, 1);
                            iIdx++;
                            sTempText = sTempText + sTempChar;
                            if (!IsHangul(sTempChar))
                            {
                                break;
                            }
                        }
                        sText = sTempText;
                    }
                    else if (nLen < list_m_tItem[inti].nLength)
                    {
                        for (int i = nLen; i < list_m_tItem[inti].nLength - 1; i++)

                        //for (int i = nLen; i < list_m_tItem[inti].nLength; i++)
                        {
                            sText = sText + sChar;
                        }
                    }
                }
                else
                {
                    if (nLen > list_m_tItem[inti].nLength)
                    {
                        iIdx = Strings.Len(sText);
                        sTempText = "";
                        for (int i = 1; i < Math.Abs(list_m_tItem[inti].nLength); i++)
                        {
                            sTempChar = Strings.Mid(sText, iIdx, 1);
                            iIdx--;
                            sTempText = sTempChar + sTempText;
                            if (!IsHangul(sTempChar))
                            {
                                break;
                            }
                        }
                        sText = sTempText;
                    }
                    else if (nLen < list_m_tItem[inti].nLength)
                    {
                        for (int i = nLen; i < list_m_tItem[inti].nLength - 1; i++)
                        {
                            sText = sChar + sText;
                        }
                    }
                }
            }
            return sText;

        }

        private bool IsHangul(string sText)
        {
            return Strings.Len(sText) == Strings.Len(Strings.StrConv(sText, VbStrConv.Narrow, 0)) ? true : false;
        }

        private string GetBarCodeItemText(int inti)
        {
            string _GetBarCodeItemText = "";
            bool bFound = false;
            _GetBarCodeItemText = GetItemText(inti);
            for (int i = 0; i < list_m_tItem.Count; i++)
            {
                if (list_m_tItem[i].nPrevItem - 1 == inti)
                {
                    _GetBarCodeItemText = _GetBarCodeItemText + GetItemText(i);
                    bFound = true;
                    break;
                }
            }
            return _GetBarCodeItemText;
        }


        public bool SendWindowDllCommand(List<string> vData, string sTagID, int nPrintCount, int nDefectCnt)
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("TagID", sTagID);
                DataTable dt = DataStore.Instance.ProcedureToDataTable("[xp_WizWork_sMtTag]", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    Sub_m_tTag.sTagID = Lib.CheckNull(dr["TagID"].ToString());
                    Sub_m_tTag.sTag = Lib.CheckNull(dr["Tag"].ToString());
                    Sub_m_tTag.nWidth = int.Parse(dr["Width"].ToString());
                    Sub_m_tTag.nHeight = int.Parse(dr["Height"].ToString());
                    //Sub_m_tTag.sUse_YN = dr["clss"].ToString();

                    Sub_m_tTag.nDefHeight = int.Parse(dr["DefHeight"].ToString());
                    Sub_m_tTag.nDefBaseY = int.Parse(dr["DefBaseY"].ToString());
                    Sub_m_tTag.nDefBaseX1 = int.Parse(dr["DefBaseX1"].ToString());
                    Sub_m_tTag.nDefBaseX2 = int.Parse(dr["DefBaseX2"].ToString());
                    Sub_m_tTag.nDefBaseX3 = int.Parse(dr["DefBaseX3"].ToString());

                    Sub_m_tTag.nDefGapY = int.Parse(dr["DefGapY"].ToString());
                    Sub_m_tTag.nDefGapX1 = int.Parse(dr["DefGapX1"].ToString());
                    Sub_m_tTag.nDefGapX2 = int.Parse(dr["DefGapX2"].ToString());
                    Sub_m_tTag.nDefLength = int.Parse(dr["DefLength"].ToString());
                    Sub_m_tTag.nDefHCount = int.Parse(dr["DefHCount"].ToString());

                    Sub_m_tTag.nDefBarClss = int.Parse(dr["DefBarClss"].ToString());
                    Sub_m_tTag.nGap = int.Parse(dr["Gap"].ToString());
                    Sub_m_tTag.sDirect = dr["Direct"].ToString();
                }

                dt = null;
                Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();
                sqlParameter2.Add("TagID", sTagID);
                dt = DataStore.Instance.ProcedureToDataTable("[xp_WizWork_sMtTagSub]", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];

                        list_m_tItem.Add(new TTagSub());

                        //list_m_tItem[i]' .sTag_ID = int.Parse(dr["TagID"].ToString());
                        //list_m_tItem[i]' .sTag_Seq = 	int.Parse(dr["TagSeq"].ToString());
                        list_m_tItem[i].sName = dr["Name"].ToString();
                        list_m_tItem[i].nType = int.Parse(dr["Type"].ToString());
                        list_m_tItem[i].nAlign = int.Parse(dr["Align"].ToString());
                        list_m_tItem[i].x = int.Parse(dr["x"].ToString());
                        list_m_tItem[i].y = int.Parse(dr["y"].ToString());
                        list_m_tItem[i].nFont = int.Parse(dr["Font"].ToString());
                        list_m_tItem[i].nLength = int.Parse(dr["Length"].ToString());
                        list_m_tItem[i].nHMulti = int.Parse(dr["HMulti"].ToString());
                        list_m_tItem[i].nVMulti = int.Parse(dr["VMulti"].ToString());
                        list_m_tItem[i].nRelation = int.Parse(dr["Relation"].ToString());
                        list_m_tItem[i].nRotation = int.Parse(dr["Rotation"].ToString());
                        list_m_tItem[i].nSpace = int.Parse(dr["Space"].ToString());

                        list_m_tItem[i].nPrevItem = int.Parse(dr["PrevItem"].ToString());
                        list_m_tItem[i].nBarType = int.Parse(dr["BarType"].ToString());
                        list_m_tItem[i].nBarHeight = int.Parse(dr["BarHeight"].ToString());
                        list_m_tItem[i].nFigureWidth = int.Parse(dr["FigureWidth"].ToString());
                        list_m_tItem[i].nFigureHeight = int.Parse(dr["FigureHeight"].ToString());
                        list_m_tItem[i].nThickness = int.Parse(dr["Thickness"].ToString());
                        list_m_tItem[i].sImageFile = dr["ImageFile"].ToString();
                        list_m_tItem[i].nWidth = int.Parse(dr["Width"].ToString());
                        list_m_tItem[i].nHeight = int.Parse(dr["Height"].ToString());
                        list_m_tItem[i].nVisible = int.Parse(dr["Visible"].ToString());

                        list_m_tItem[i].sFontName = dr["FontName"].ToString();
                        list_m_tItem[i].sFontStyle = dr["FontStyle"].ToString();
                        list_m_tItem[i].sFontUnderLine = dr["FontUnderLine"].ToString();


                        //int a = 0;
                        //foreach (string str in lData)
                        //{
                        //    Console.WriteLine(a++.ToString() + "/////" + str + "///////");
                        //}

                        //20171011 김종영 수정 type 변경
                        //if (list_m_tItem[i].nType == 1 && list_m_tItem[i].sName.Substring(0, 1).ToUpper() == "D")
                        if (list_m_tItem[i].nType < 2 && list_m_tItem[i].sName.Substring(0, 1).ToUpper() == "D")
                        {
                            if (list_m_tItem[i].nRelation == 0 && list_m_tItem[i].nType == 1)//바코드
                            {
                                list_m_tItem[i].sText = vData[0];
                            }

                            else if (list_m_tItem[i].nRelation > 0 && list_m_tItem[i].nType == 0)
                            {
                                if (vData.Count > list_m_tItem[i].nRelation)
                                {
                                    list_m_tItem[i].sText = vData[list_m_tItem[i].nRelation];
                                }
                                else
                                {
                                    list_m_tItem[i].sText = "";
                                }
                            }
                        }
                        else
                        {
                            list_m_tItem[i].sText = Lib.CheckNull(dr["Text"].ToString());
                        }
                    }
                }

                double strWidth = 0;
                double strHeight = 0;
                try
                {
                    if (Lib.CheckNum(Sub_m_tTag.nWidth.ToString()) != "0")
                    {
                        strWidth = (Sub_m_tTag.nWidth / 10F);
                    }
                    if (Lib.CheckNum(Sub_m_tTag.nHeight.ToString()) != "0")
                    {
                        strHeight = (Sub_m_tTag.nHeight / 10F);
                    }
                }
                catch
                {
                    strWidth = 0;
                    strHeight = 0;
                }


                // setup

                //TSCLIB_DLL.setup(stringFormatN1(strWidth), stringFormatN1(strHeight), "8", "15", "0", "3", "0");//기존소스
                TSCLIB_DLL.setup(stringFormatN1(strWidth), stringFormatN1(strHeight), "4", "15", "1", "4", "0"); // GLS Black Mark Setting
                //TSCLIB_DLL.setup(stringFormatN1(strWidth), stringFormatN1(strHeight), "8", "15", "0", "0", "0");//감열지 테스트용
                TSCLIB_DLL.sendcommand("DIRECTION " + Sub_m_tTag.sDirect);

                TSCLIB_DLL.clearbuffer();
                string sText = "";
                string[] sBarType = new string[2];

                for (int i = 0; i < list_m_tItem.Count; i++)
                {
                    if (list_m_tItem[i].nVisible > 0)//출력여부
                    {
                        //'바코드
                        if (list_m_tItem[i].nType == EnumItem.IO_BARCODE)
                        {
                            if (list_m_tItem[i].nPrevItem == 0)
                            {
                                if (list_m_tItem[i].nBarType == 0)// 1:1 Code
                                {
                                    sBarType[0] = "1";
                                    sBarType[1] = "1";
                                }
                                else                            // 2:5 Code
                                {
                                    sBarType[0] = "2";
                                    sBarType[1] = "5";
                                }

                                string ReadAble = "0"; // 1 : 자동 바코드 출력 / 0 : 안보임

                                TSCLIB_DLL.barcode(list_m_tItem[i].x.ToString(), // x
                                                   list_m_tItem[i].y.ToString(), // y
                                                   "39", // type
                                                   list_m_tItem[i].nBarHeight.ToString(), // height
                                                   ReadAble, // ReadAble
                                                   list_m_tItem[i].nRotation.ToString(), // Rotation
                                                   sBarType[0], // Narrow
                                                   sBarType[1], // Wide
                                                   list_m_tItem[0].sText 
                                                   );

                                if (ReadAble.Equals("0"))
                                {
                                    // 바코드 글자 세팅
                                    int intx = list_m_tItem[i].x;
                                    int inty = list_m_tItem[i].y + 46;
                                    int fontheight = 50;
                                    int rotation = 0;
                                    int fontstyle = 0;
                                    int fontunderline = 0;
                                    string FaceName = "맑은 고딕";
                                    string content = Lib.CheckNull(list_m_tItem[i].sText).Trim();

                                    TSCLIB_DLL.windowsfont(intx, inty, fontheight, rotation, fontstyle, fontunderline, FaceName, content);
                                }
                            }
                        }
                        //데이터 OR 문자
                        else if (list_m_tItem[i].nType == EnumItem.IO_DATA || list_m_tItem[i].nType == EnumItem.IO_TEXT)
                        {
                            sText = Lib.CheckNull(list_m_tItem[i].sText);
                            int intx = list_m_tItem[i].x;
                            int inty = list_m_tItem[i].y;
                            int fontheight = int.Parse((list_m_tItem[i].nFont).ToString());
                            int rotation = list_m_tItem[i].nRotation;
                            int fontstyle = int.Parse(Lib.CheckNum(list_m_tItem[i].sFontStyle));
                            int fontunderline = int.Parse(Lib.CheckNum(list_m_tItem[i].sFontUnderLine));
                            string szFaceName = list_m_tItem[i].sFontName;
                            string content = sText.Trim();

                            TSCLIB_DLL.windowsfont(intx, inty, fontheight, rotation, fontstyle, fontunderline, szFaceName, content);
                        }
                        //'선(Line)-5이하
                        else if (list_m_tItem[i].nType == EnumItem.IO_LINE)// && (list_m_tItem[i].nFigureHeight <= 5 || list_m_tItem[i].nFigureWidth <= 5))
                        {
                            int x1 = 0;
                            int x2 = 0;
                            int y1 = 0;
                            int y2 = 0;
                            int.TryParse(list_m_tItem[i].x.ToString(), out x1);
                            int.TryParse(list_m_tItem[i].y.ToString(), out y1);
                            int.TryParse(list_m_tItem[i].nFigureWidth.ToString(), out x2);
                            int.TryParse(list_m_tItem[i].nFigureHeight.ToString(), out y2);

                            string IsDllStr = "BAR " + x1.ToString() + ", " + y1.ToString() + ", " + x2.ToString() + ", " + y2.ToString();

                            TSCLIB_DLL.sendcommand(IsDllStr);
                        }
                        else if (list_m_tItem[i].nType == EnumItem.IO_BOX)
                        {
                            int x1 = 0;
                            int x2 = 0;
                            int y1 = 0;
                            int y2 = 0;
                            int nTh = 0;
                            int.TryParse(list_m_tItem[i].x.ToString(), out x1);
                            int.TryParse(list_m_tItem[i].y.ToString(), out y1);
                            int.TryParse(list_m_tItem[i].nFigureWidth.ToString(), out x2);
                            int.TryParse(list_m_tItem[i].nFigureHeight.ToString(), out y2);
                            int.TryParse(list_m_tItem[i].nThickness.ToString(), out nTh);

                            string IsDllStr = "BOX " + x1.ToString() + ", " + y1.ToString() + ", " + x2.ToString() + ", " + y2.ToString() + ", " + nTh.ToString();

                            TSCLIB_DLL.sendcommand(IsDllStr);
                        }

                    }
                }
                if (m_ProcessID == "0405")
                {
                    nPrintCount = 2;
                }

                TSCLIB_DLL.printlabel("1", nPrintCount.ToString());

                list_m_tItem = new List<TTagSub>();
                vData = new List<string>();
                return true;
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의<SendWindowDllCommand>\r\n{0}", excpt.Message), "[오류]", 0, 1);
                return false;
            }
        }


        private void PrintWorkCard(int intPrintCount)
        {
            string g_sPrinterName = Lib.GetDefaultPrinter();
            try
            {
                int R = 0;      // Rotation R.
                IsTagID = Frm_tprc_Main.g_tBase.TagID;
                List<string> list_Data = null;
                //Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                //sqlParameter.Add("InstID", list_TWkLabelPrint[0].sInstID);
                //DataTable dt = DataStore.Instance.ProcedureToDataTable("[xp_WorkCard_sWorkCard]", sqlParameter, false);

                for (int i = 0; i < intPrintCount; i++)
                {                    
                    list_Data = new List<string>();
                    Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();

                    if (Frm_tprc_Main.list_g_tsplit.Count > i)
                    {
                        sqlParameter2.Add("InstID", Frm_tprc_Main.list_g_tsplit[i].InstID);
                        sqlParameter2.Add("CardID", Frm_tprc_Main.list_g_tsplit[i].LabelID);
                        R++;
                    }
                    else
                    {
                        sqlParameter2.Add("InstID", list_TWkLabelPrint[i - R].sInstID);
                        if (LabelPrintYN == "Y")
                        {
                            sqlParameter2.Add("CardID", list_TWkLabelPrint[i - R].sLabelID);
                        }
                        else
                        {
                            sqlParameter2.Add("CardID", list_TWkResult[i - R].LabelID);
                        }
                    }
                    
                    DataTable dt2 = DataStore.Instance.ProcedureToDataTable("[xp_WorkCard_sWorkCardPrint]", sqlParameter2, false);
                    lData = new List<string>();
                    string strProcessID = "";
                    int a = 0;
                    int count = 0;
                    double douworkqty = 0;
                    double doudefectqty = 0;
                    string effectdate = "";
                    foreach (DataRow dr in dt2.Rows)
                    {
                        strProcessID = dr["ProcessID"].ToString();
                        if (dr["ProcSeq"].ToString() == "1") // 첫 공정일 때,
                        {
                            double.TryParse(dr["WorkQty"].ToString(), out douworkqty);
                            double.TryParse(dr["wk_defectQty"].ToString(), out doudefectqty);

                            list_Data.Add(Lib.CheckNull(dr["wk_CardID"].ToString())); //라벨번호(공정전표)

                            list_Data.Add(Lib.CheckNull(dr["Article"].ToString())); // 품명
                            list_Data.Add(Lib.MakeDate(WizWorkLib.DateTimeClss.DF_FD, Lib.CheckNull(dr["wk_ResultDate"].ToString())));//D_생산일자
                            list_Data.Add(Lib.CheckNull(dr["BuyerArticleNo"].ToString()));// 품번
                            list_Data.Add(Lib.CheckNull(dr["Model"].ToString()));// 차종
                            list_Data.Add(Lib.CheckNull(dr["Process"].ToString()));// 공정명
                            list_Data.Add((string.Format("{0:n0}", (int)douworkqty)));// _수량
                            list_Data.Add((string.Format("{0:n0}", (int)doudefectqty)));// _불량수량
                            list_Data.Add(Lib.CheckNull(dr["wk_Name"].ToString()));// 작업자
                        }

                        if (dr["ProcSeq"].ToString() != "1" && list_Data.Count > 7)
                        {
                            list_Data.Add(Lib.CheckNull(dr["Process"].ToString())); // 다음(순차) 공정의  품명
                        }
                    }

                    g_sPrinterName = Lib.GetDefaultPrinter();
                    TSCLIB_DLL.openport(g_sPrinterName);
                    if (SendWindowDllCommand(list_Data, IsTagID, 1, 0))
                    {
                        Message[0] = "[라벨발행중]";
                        Message[1] = "라벨 발행중입니다. 잠시만 기다려주세요.";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 2, 2);
                    }
                    else
                    {
                        Message[0] = "[라벨발행 실패]";
                        Message[1] = "라벨 발행에 실패했습니다. 관리자에게 문의하여주세요.\r\n<SendWindowDllCommand>";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 2, 2);
                    }
                    TSCLIB_DLL.clearbuffer();
                    TSCLIB_DLL.closeport();
                }
            }
            catch (Exception excpt)
            {
                Message[0] = "[오류]";
                Message[1] = string.Format("오류!관리자에게 문의\r\n{0}", excpt.Message);
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
            }
        }


        private void cmdBoxList_Click(object sender, EventArgs e)
        {

        }

        private void cmdWorkDefect_Click(object sender, EventArgs e)
        {
            //frm_tprc_Work_Defect_U my4mPop = new frm_tprc_Work_Defect_U(m_OrderID, m_OrderSeq, this.txtBoxID.Text, m_OrderNO, this.txtArticle.Text, "0", this.txtProdQty.Text, m_UnitClss); // vb g_tBase.OrderQty에 값 넣는 부분이 없음
            //my4mPop.Owner = this;
            //my4mPop.WriteTextEvent += My4mPop_WriteTextEvent;
            //my4mPop.ShowDialog();
            ////if (myBoxPop.ShowDialog() == DialogResult.OK)
            ////{
            ////    this.txtBoxQty.Text = numkeypad3.tbInputText.Text;
            ////}
            ////this.cmdBoxQty.Checked = false;
            ///
            frm_tprc_Work_Defect_U defect = new frm_tprc_Work_Defect_U(updateJobID, dicDefect);
            defect.Owner = this;
            defect.ShowDialog();
            if (defect.DialogResult == DialogResult.OK)
            {
                this.dicDefect = defect.dicDefect;
                this.txtDefectQty.Text = defect.returnTotalQty;
            }
            return;
        }

        private void My4mPop_WriteTextEvent(string SumDefectQty)
        {
            txtDefectQty.Text = string.Format("{0:n0}", SumDefectQty);
        }       


        private void SetGrid1RowClear()
        {
            while (GridData1.Rows.Count > 0)
            {
                GridData1.Rows.RemoveAt(0);
            }
        }
        private void SetGrid12owClear()
        {
            while (GridData2.Rows.Count > 0)
            {
                GridData2.Rows.RemoveAt(0);
            }
        }


        private void SetFormDataClear()
        {
            

            this.txtArticle.Text = "";
            txtBuyerArticleNo.Text = "";            

            //this.txtSpec.Text = "";

            SetGrid1RowClear();
            SetGrid12owClear();

            this.txtRemark.Text = "";
            this.txtDailyInstWorkQty.Text = "";
            this.txtDefectQty.Text = "";
            this.txtOrderQty.Text = "";
            this.txtOrderWorkQty.Text = "";
            this.txtOrderRemainQty.Text = "";
            this.txtlInstQty.Text = "";
            this.txtInstRemainQty.Text = "";
            txtInRmUnitClss.Text = "";
            txtInUnitClss.Text = "";

            txtProdQty.Text = "";
            txtInstWorkQty.Text = "";
            

            SetDateTimePicker();

            txtBoxID.Text = "";            
        }

        private void SetDateTimePicker()
        {
            mtb_From.Text = DateTime.Today.ToString("yyyyMMdd");
            mtb_To.Text = DateTime.Today.ToString("yyyyMMdd");
            dtStartTime.Format = DateTimePickerFormat.Custom;
            dtStartTime.CustomFormat = "HH:mm:ss";
            dtEndTime.Format = DateTimePickerFormat.Custom;
            dtEndTime.CustomFormat = "HH:mm:ss";
        }


        private void Form_Activate()
        {
            try
            {
                ////앞공정의 실적을 체크한다. 없을 시 close한다. ex)2차가류에 데이터가 없는데, 
                //Dictionary<string, object> sqlParameters = new Dictionary<string, object>();
                //sqlParameters.Add("@InstID", Frm_tprc_Main.g_tBase.sInstID);
                //sqlParameters.Add("@InstSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);
                //DataTable dtb = DataStore.Instance.ProcedureToDataTable("[xp_WizWork_sWorkQtyByInstIDProcessID]", sqlParameters, false);
                //foreach (DataRow dr in dtb.Rows)
                //{
                //    if (dr["WorkQty"].ToString() == "0")
                //    {
                //        string pro = dr["Process"].ToString();
                //        string nowpro = Frm_tprc_Main.g_tBase.Process;
                //        Message[0] = "[실적오류]";
                //        Message[1] = pro + " 공정에 실적이 입력되지 않았기때문에.\r\n" + nowpro + " 공정의 작업실적을 입력해주세요.";
                //        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                //        blpldClose = true;
                //        return;
                //    }
                //}

                //하위품 있다 : Y , 없다 : N
                string[] CheckYN = new string[2];
                string Query = "select ChildCheckYN from mt_Process where ProcessID = '" + Frm_tprc_Main.g_tBase.ProcessID + "'";
                CheckYN = DataStore.Instance.ExecuteQuery(Query, false);
                ChildCheckYN = CheckYN[1];


                CheckLabelID(Frm_tprc_Main.g_tBase.sLotID);//pl_inputdet의 LotID를 
                
                FillGridData1();//당일 해당공정의 생산실적 조회           
                Frm_tprc_Main.g_tBase.OrderID = m_OrderID;//생산으로 넘어올 시 글로벌 오더ID 변경

                /////
                ///
                m_LabelID = lstStartLabel[0].ToString(); //2021-09-29 StartLabel 가져오기
                txtPreInsertLabelBarCode.Text = m_LabelID;
                BarcodeEnter();  // 여기서 이제 선 기입한 바코드 값 자동기입.
                /////

                // 시작일자와 시작시간을 scandate로 맞출 것.
                DateTime dateTime = new DateTime();
                dateTime = DateTime.ParseExact(m_WorkStartDate, "yyyyMMdd", null);
                mtb_From.Text = dateTime.ToString("yyyy-MM-dd");

                DateTime dt = DateTime.Now;
                dt = DateTime.ParseExact(m_WorkStartTime, "HHmmss", null);
                dtStartTime.Value = dt;

                // 현재 작업자 표시.
                txtNowWorker.Text = Frm_tprc_Main.g_tBase.Person;

                // WorkLog에 데이터 수집을 하는 공정이라면, 
                // 수집데이터를 가져와서 자동으로 뿌려주는 작업을 진행해야 겠지. ㅇㅇ.
                Find_Collect_WorkLogData();

                SetCombox(); //2021-09-28 거래처 콤보박스 생성


                // GLS.
                // 지금이 첫 공정이라면, 저장은 물론, 공정이동전표도 발행해야 하고,
                // 첫공정이 아니라면, 그냥 저장만.
                if (Frm_tprc_Main.g_tBase.sInstDetSeq == "1")
                {
                    // Mt_Article의 LabelPrintYN 여부에 따라서
                    // 첫 공정이더라도 라벨발행 없이 그냥 저장만 될 수도 있어야 한다. (Wh_Ar_LabelPrintYN)                    
                    // GLS. _ 허윤구. _20_0414.
                    if (Wh_Ar_LabelPrintYN == "N")
                    {
                        // 첫공정이지만, 라벨발행 하고싶지 않다.  // (라벨은 만들되, 발행만 안되도록)
                        cmdSave.Text = "저 장\r\n(전표미발행)";
                        LabelPrintYN = "Y";
                        Frm_tprc_Main.g_tBase.TagID = "008"; //GLS 공정이동전표
                        if (cmdSave.Enabled == true)
                        {
                            btnBringSplitData.Enabled = true;
                        }
                       
                    }
                    else
                    {
                        cmdSave.Text = "전표발행";
                        LabelPrintYN = "Y";
                        Frm_tprc_Main.g_tBase.TagID = "008"; //GLS 공정이동전표
                        if (cmdSave.Enabled == true)
                        {
                            btnBringSplitData.Enabled = true;
                        }
                    }                    
                }
                else
                {
                    cmdSave.Text = "저 장";
                    LabelPrintYN = "N";
                    btnBringSplitData.Text = "일괄스캔";
                    if (cmdSave.Enabled == true)
                    {
                        btnBringSplitData.Enabled = true;
                    }

                    lblRemainAdd.Text = "일괄 수량";

                }

                // 수량 / CycleTime 값 0 세팅.
                if (txtFacilityCollectQty.Text == string.Empty)
                {
                    txtFacilityCollectQty.Text = "0";       // 설비수집 수량
                }
                
                txtWorkQty.Text = "0";                  // (내가 한) 작업수량
                txtRemainAdd.Text = "0";                // (남이 한) 잔량수량
                txtLotProdQty.Text = "0";               // 생산박스 당 수량
                txtTotalLabelQty.Text = "0";            // 총 수량( 라벨발행의 기준)
                txtCycleTime.Text = "0";                // Cycle Time

                // 생산박스 당 수량 값 가져오기.
                if (this.txtArticleID.Text != string.Empty)
                {
                    // ArticleID가 어쨌건 무언가 있다는 거니까.
                    BringProdLotQty(this.txtArticleID.Text);
                }
            }

            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
            }
        }

        #region 생산박스 당 수량 값 가져오기(BringProdLotQty)
        private void BringProdLotQty(string ArticleID)
        {
            string[] ProdLotQty = new string[2];
            string Query = "select ProdQtyPerBox from mt_Article where ArticleID = '" + ArticleID + "'";
            ProdLotQty = DataStore.Instance.ExecuteQuery(Query, false);
            txtLotProdQty.Text = Lib.CheckNull(ProdLotQty[1]);
        }

        #endregion

        private void ShowYbox(bool blViewYn)
        {
            if (blViewYn == true)
            {                

                pnlFrame2.Visible = false;
            }
            else
            {

                pnlFrame2.Visible = true;
            }
        }
        private void ShowMoveStatement(bool blViewYn)
        {
            if (blViewYn == true)
            {
                pnlMoveStatement.Visible = true;
                //this.pnlMoveStatement.Location = new System.Drawing.Point(214, 258);
                this.pnlMoveStatement.Location = new System.Drawing.Point(2, 112);

            }
            else
            {
                pnlMoveStatement.Visible = false;
                this.pnlMoveStatement.Location = new System.Drawing.Point(2, 788);

            }
        }

        private void ShowBoxList(bool blViewYn)
        {            
        }

        //pl_inputdet의 LotID를 
        private string CheckLabelID(string strBarCode)
        {
            string strInstID = "";
            try
            {
                string sMoldID = "";
                if (Frm_tprc_Main.list_tMold.Count > 0)
                {
                    sMoldID = Frm_tprc_Main.list_tMold[0].sMoldID;
                }

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PLotID", strBarCode);
                sqlParameter.Add("ProcessID", m_ProcessID); //SearchProcessID());                
                sqlParameter.Add("MoldID", sMoldID);

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_Chkworklotid", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    m_MtrExceptYN = Lib.CheckNull(dr["MtrExceptYN"].ToString());//PLotID가 라벨일때 pl_input의 MtrExceptYN
                    m_OutwareExceptYN = Lib.CheckNull(dr["OutwareExceptYN"].ToString());//PLotID가 라벨일때 pl_input의 OutwareExceptYN
                    strInstID = Lib.CheckNull(dr["InstID"].ToString());//PLotID가 라벨일때 pl_input의 InstID
                    this.txtInstID.Text = Lib.CheckNull(dr["InstID"].ToString());//PLotID가 라벨일때 pl_input의 InstID
                    this.txtInstDetSeq.Text = Lib.CheckNull(dr["InstDetSeq"].ToString());//PLotID가 라벨일때 pl_inputdet의 Instdetseq
                    this.txtLabelGubun.Text = Lib.CheckNull(dr["LabelGubun"].ToString());//PLotID가 라벨일때 LabelGubun = 0                    
                    this.txtArticleID.Text = Lib.CheckNull(dr["ArticleID"].ToString());//PLotID가 라벨일때 pl_inputdet의 ArticleID
                    this.txtArticle.Text = Lib.CheckNull(dr["pldArticle"].ToString());
                    this.txtBuyerArticleNo.Text = Lib.CheckNull(dr["BuyerArticleNo"].ToString());

                    // 라벨발행여부 YN (전역변수 기입)
                    Wh_Ar_LabelPrintYN = Lib.CheckNull(dr["LabelPrintYN"].ToString());


                    // 차종을 조회하는 대신, 그 자리에 진행중인 호기정보를 표시해 주세요.
                    // 2020.03.25 여영애 과장님
                    //this.txtCarModel.Text = Lib.CheckNull(dr["Model"].ToString());   // 차종



                    double InstQty = 0;
                    double ProdQtyPerBox = 0;
                    double InstWorkQty = 0;
                    double InstRemainQty = 0;
                    double.TryParse(dr["InstQty"].ToString(), out InstQty);
                    //pIdProdQtyPerBox 임시 18.06.18 계속확인할것
                    double.TryParse(dr["ProdQtyPerBox"].ToString(), out ProdQtyPerBox);
                    //if (m_ProcessID == "0405" || m_ProcessID == "1101" || m_ProcessID == "2101")
                    //{
                    //    double.TryParse(dr["pIdProdQtyPerBox"].ToString(), out ProdQtyPerBox);
                    //}
                    //else
                    //{

                    //}
                    //pIdProdQtyPerBox 임시 18.06.18 계속확인할것
                    double.TryParse(dr["InstWorkQty"].ToString(), out InstWorkQty);
                    InstRemainQty = InstQty - InstWorkQty;

                    txtlInstQty.Text = string.Format("{0:n0}", (int)InstQty);//지시량
                    txtInstRemainQty.Text = string.Format("{0:n0}", (int)InstRemainQty);//남은지시수량 = 지시수량 - 지시누계량

                    txtInUnitClss.Text = Lib.CheckNull(dr["UnitClssName"].ToString());
                    txtInRmUnitClss.Text = Lib.CheckNull(dr["UnitClssName"].ToString());

                    // 
                    if (!strBarCode.ToUpper().Contains("PL"))
                    {
                        txtProdQty.Text = string.Format("{0:n0}", (int)ProdQtyPerBox);//mt_article의 qtyperbox박스당수량
                    }
                    txtInstWorkQty.Text = string.Format("{0:n0}", (int)InstWorkQty);//지시누계량 = wk_result 생산수량의 합 
                                                                                    //

                    txtErrMsg.Text = Lib.CheckNull(dr["Msg"].ToString());//에러메세지

                    if (Lib.CheckNull(dr["OrderArticleID"].ToString()) ==
                        Lib.CheckNull(dr["ArticleID"].ToString())) //Y : 완제품, N : 완제품X
                    {
                        m_LastArticleYN = "Y";//마지막Article이니? Y 완제품이니?
                    }
                    else
                    {
                        m_LastArticleYN = "N";//마지막Article이니? N
                    }
                    txtRemark.Text = Lib.CheckNull(dr["Remark"].ToString());//pl_inputdet Remark
                    Frm_tprc_Main.g_tBase.Article = Lib.CheckNull(dr["Article"].ToString());//mt_article
                    Frm_tprc_Main.g_tBase.OrderID = Lib.CheckNull(dr["OrderID"].ToString());//pl_input
                    Frm_tprc_Main.g_tBase.OrderNO = Lib.CheckNull(dr["OrderNO"].ToString());//order

                    ///////////////
                    int WorkQty = 0;
                    int OrderSeq = 0;
                    int.TryParse(dr["ProdQtyPerBox"].ToString(), out WorkQty);
                    int.TryParse(dr["OrderSeq"].ToString(), out OrderSeq);
                    Frm_tprc_Main.g_tBase.WorkQty = WorkQty;//wk_labelprint의 수량
                    //전역변수 WorkQty(생산량)에 박스당 수량을 집어넣는다? 왜?? 수정이 필요해보임
                    Frm_tprc_Main.g_tBase.OrderUnit = Lib.CheckNull(dr["UnitClss"].ToString());//order
                    Frm_tprc_Main.g_tBase.OrderSeq = OrderSeq;
                    //////////
                    m_OrderID = Lib.CheckNull(dr["OrderID"].ToString());//pl_input의 OrderID
                    m_ProdAutoInspectYN = Lib.CheckNull(dr["ProductAutoInspectYN"].ToString());//Order의 ProductAutoInspectYN
                    m_OrderSeq = OrderSeq;
                    /////////////
                    Frm_tprc_Main.g_tBase.Basis = "";
                    Frm_tprc_Main.g_tBase.BasisID = 0;

                    m_OrderNO = Lib.CheckNull(dr["OrderNO"].ToString());//수주번호
                    m_UnitClss = Lib.CheckNull(dr["UnitClss"].ToString());//pl_inputdet articleid의 UnitClss

                                        
                    
                    /////////////////////////
                    double OrderQty = 0;
                    double OrderWorkQty = 0;
                    double OrderRemainQty = 0;
                    double DefectQty = 0;
                    double DailyInstWorkQty = 0;
                    double.TryParse(dr["OrderQty"].ToString(), out OrderQty);
                    double.TryParse(dr["OrderWorkQty"].ToString(), out OrderWorkQty);
                    double.TryParse(dr["DefectQty"].ToString(), out DefectQty);
                    double.TryParse(dr["DailyInstWorkQty"].ToString(), out DailyInstWorkQty);
                    OrderRemainQty = OrderQty - OrderWorkQty;
                    txtOrderQty.Text = string.Format("{0:n0}", OrderQty);//오더전체수주량//전체오더량
                    txtOrderWorkQty.Text = string.Format("{0:n0}", OrderWorkQty);//오더생산량
                    txtOrderRemainQty.Text = string.Format("{0:n0}", OrderRemainQty);//오더잔량 = 전체오더량 - 오더에 따른 생산량(wk_result)
                    //txtDefectQty.Text = string.Format("{0:n0}", DefectQty); // 오더번호 기준 기존 불량수량
                    txtDailyInstWorkQty.Text = string.Format("{0:n0}", DailyInstWorkQty);//당일 지시 누계량
                    
                    //2018.06.17 추가

                    m_OrderArticleID = dr["OrderArticleID"].ToString().Trim();//오더ArticleID

                    ////수정여지가 있음..원인분석필요 .. 프로시저 수정?? 
                    //if (Frm_tprc_Main.g_tBase.ProcessID == "1101" || Frm_tprc_Main.g_tBase.ProcessID == "1105")//준비공정에서만 ....
                    //{
                    //    m_OrderArticleID = Lib.CheckNull(dr["ArticleID"].ToString());
                    //}
                    //2018.06.18 주석
                    //사출공정일때 박스당 수량 텍스트박스에 박스당수량 자동으로 입력해준다.
                    //if (Frm_tprc_Main.g_tBase.ProcessID == "2101")// || Frm_tprc_Main.g_tBase.ProcessID == "2101") // '0401:재단, 2101:성형 제외시킴
                    //{
                    //    txtQtyPerBox.Text = Lib.CheckNull(dr["ProdQtyPerBox"].ToString());
                    //    UpdatepnlBoxQty("W");
                    //}
                    //2018.06.18 주석
                    
                    UpdatepnlBoxQty("W");

                    //전역변수 LotID Frm_tprc_Main.g_tBase.sLotID = strBarCode
                    //strbarcode가 라벨id가 아니라 pl_inputdet의 lotid 일때
                    //하위품 그리드를  채워넣는다.
                    if (strBarCode.Contains("PL"))
                    {
                        FillGridData2(strInstID, Frm_tprc_Main.g_tBase.ProcessID);
                    }
                }
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", excpt.Message), "[오류]", 0, 1);
                return "";
            }

            return strInstID;

        }

        private void SetGridData2RowClear()
        {
            while (GridData2.Rows.Count > 0)
            {
                GridData2.Rows.RemoveAt(0);
            }
        }

        

        /// <summary>
        /// LotID에 해당하는 ArticleID 가져오기
        /// 하위품 스캔 체크
        /// </summary>
        /// <param name="strBarCode"></param>
        private bool BarCodeCheck(string strBarCode)
        {
            DataRow dr = null;
            try
            {
                YLabelOK = false;
                //Detail ProcessYN 체크 / 세부공정인지 확인
                string DetailProcessYN = "";
                string[] DetailProcYN = new string[2];
                string sql = "select DetailProcessYN from mt_Process where ProcessID = '" + Frm_tprc_Main.g_tBase.ProcessID + "'"; //2021-02-26 DetailProcessYN 칼럼이 없음
                DetailProcYN = DataStore.Instance.ExecuteQuery(sql, false);
                DetailProcessYN = DetailProcYN[1];

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("LotID", strBarCode);
                if (DetailProcessYN == "Y")
                {
                    string strTopProcess = Frm_tprc_Main.g_tBase.ProcessID.Substring(0, 2) + "01";
                    sqlParameter.Add("ProcessID", strTopProcess);
                }
                else
                {
                    sqlParameter.Add("ProcessID", m_ProcessID);
                }
                sqlParameter.Add("MachineID", m_MachineID);
                sqlParameter.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                sqlParameter.Add("InstDetSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);
                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_sLotInfoByLotID_PrdMove", sqlParameter, false); //2021-09-28

                if (dt != null && dt.Rows.Count > 0)
                {
                    dr = dt.Rows[0];                   

                    m_ArticleID = dr["ArticleID"].ToString().Trim();
                    m_LabelGubun = dr["LabelGubun"].ToString().Trim();                    //라벨구분 1은 원자재 
                    this.txtLabelGubun.Text = m_LabelGubun;
                    m_Inspector = dr["Inspector"].ToString().Trim();                      //검사자
                    double.TryParse(dr["Stuffin01_Qty"].ToString(), out m_RemainQty);         //전체재고량
                    double.TryParse(dr["LocRemainQty"].ToString(), out m_LocRemainQty);   //해당창고재고량
                    m_UnitClss = dr["UnitClss"].ToString();
                    m_UnitClssName = dr["UnitClssName"].ToString();                         //투입되는 원자재의 재고단위
                    m_EffectDate = Lib.MakeDateTime("yyyyMMdd", dr["EffectDate"].ToString());

                    m_ArticleIDMove = dr["ArticleID"].ToString().Trim();    //2021-09-29
                    m_LabelGubunMove = dr["LabelGubun"].ToString().Trim();  //2021-09-29
                    m_UnitClssMove = dr["UnitClss"].ToString();             //2021-09-29

                    if (m_MtrExceptYN == "N")//예외처리YN : 예외처리 아닐때
                    {
                        
                    }
                    return true;
                }
                else
                {
                    //2020.04.03 허윤구
                    // DT가 NULL이 될 수 있는 케이스.
                    // 지속적으로 찾아서 개별 케이스별로 메시지 UPDATE 해야 합니다.

                    //1. 하위품이 사라진경우. > 즉, 알수없는 (여러) 이유로 삭제된 케이스.                    
                    
                    string[] DeleteBarcodeYN = new string[2];
                    string sql_2 = "select cnt = COUNT(*) from wk_result where labelid = '" + strBarCode + "'";
                    DeleteBarcodeYN = DataStore.Instance.ExecuteQuery(sql_2, false);
                    if (DeleteBarcodeYN[1] == "0")
                    {
                        Message[0] = "[하위품 소실]";
                        Message[1] = "해당 하위품( " + strBarCode + " )은 승인되지 않은 품목이거나 삭제처리된 Lot입니다\r\n(작업 취소 후 재시작 해주세요.).";
                        throw new Exception();
                    }
                    else
                    {
                        Message[0] = "[입고승인]";
                        Message[1] = "해당 품목은 승인되지 않은 품목이거나 입고내역이 없는 품목이므로 사용할 수 없습니다.\r\n(작업 취소 후 재시작 해주세요.)";
                        throw new Exception();
                    }
                }
            }
            catch (Exception excpt)
            {
                m_ArticleID = "";
                m_LabelGubun = "";
                m_Inspector = "";
                m_RemainQty = 0;
                m_LocRemainQty = 0;
                m_UnitClss = "";
                m_UnitClssName = "";
                m_EffectDate = "";

                // 2020.09.23 아이고 세상에...
                // 오류 발생시 저장이 안되도록 해야 됨!!!!!
                cmdSave.Enabled = false;
                btnBringSplitData.Enabled = false;
                cmdWorkDefect.Enabled = false;

                return false;
            }
        }



        private void FillGridData2(string strInstID, string strProcessID)
        {
            double dulReqQty = 0;
            SetGridData2RowClear();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Add("InstID", strInstID);
                sqlParameter.Add("ProcessID", strProcessID);

                DataTable dt = null;
                dt = DataStore.Instance.ProcedureToDataTable("xp_wklabelprint_sGetworkchild", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int a = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        double.TryParse(dr["ReqQty"].ToString(), out dulReqQty);
                        GridData2.Rows.Add(++a
                                           , dr["InstID"].ToString().Trim()
                                           , dr["DetSeq"].ToString().Trim()
                                           , dr["ChildSeq"].ToString().Trim()
                                           , dr["ChildArticleID"].ToString().Trim()
                                           , dr["Article"].ToString().Trim()
                                           , dr["BuyerArticleNo"].ToString().Trim()
                                           , ""
                                           , dr["ScanExceptYN"].ToString().Trim()
                                           , ""
                                           , dr["Flag"].ToString().Trim()
                                           , dr["ScanExceptYN"].ToString().Trim()
                                           , ""
                                           , ""
                                           , dr["UnitClss"].ToString().Trim()
                                           , ""
                                           , dulReqQty.ToString()
                                           , ""
                                           , m_UnitClss//""
                                           , "" 
                                           , ""
                                          );
                        if (dr["ScanExceptYN"].ToString() == "Y")
                        {
                            GridData2.Rows[a - 1].DefaultCellStyle.BackColor = Color.FromArgb(238, 108, 128);
                        }                        
                    }
                }
            }
            catch (Exception excpt)
            {
                Message[0] = "[오류]";
                Message[1] = string.Format("오류!관리자에게 문의\r\n{0}", excpt.Message);
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
            }
        }

        /// <summary>
        /// BoxQty Update (생산수량 , 박스당 수량 변동으로 인한)
        /// </summary>
        /// <param name="strGbn"></param>
        private void UpdatepnlBoxQty(string strGbn)
        {
            

        }

        /// <summary>
        /// 스켄 ID  기본 정상여부 확인
        /// </summary>
        /// <returns></returns>
        private bool LF_Check_ScanData(string strBarcode)
        {
            bool blResult = true;
            if (strBarcode != "")
            {
                if (strBarcode.ToUpper().Contains("PL"))
                {
                    //'지시 LotID 15, 16자리
                    if (!(strBarcode.Trim().Length == 15 || strBarcode.Trim().Length == 16))
                    {
                        WizCommon.Popup.MyMessageBox.ShowBox("코드가 잘못되었습니다.", "[바코드 길이오류]", 0, 1);
                        blResult = false;
                    }
                    return blResult;
                }
                //공정이동전표 , 길이 변경 2017.02.09 ,   13 --> 9 자리로 ,  외에는 13자리
                else if (strBarcode.ToUpper().Contains("C") //성형이동전표
                    || strBarcode.ToUpper().Contains("I")   //원자재이동전표
                    || strBarcode.ToUpper().Contains("M")   //혼련이동전표
                    || strBarcode.ToUpper().Contains("T")   //재단이동전표
                    || strBarcode.ToUpper().Contains("B"))  //박스이동전표?
                {
                    //'지시 LotID 15자리
                    if ((strBarcode.Trim().Length != 10))
                    {
                        Message[0] = "[길이 오류]";
                        Message[1] = "코드가 잘못되었습니다.";
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);

                        blResult = false;

                    }
                    return blResult;
                }
            }
            return blResult;
        }

        private bool SetProcessID(string strBarcode)
        {
            bool blResult = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("ProcessID", strBarcode);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Code_sProcess", sqlParameter, false);
                DataTable dt = null;

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                    Frm_tprc_Main.g_tBase.ProcessID = dt.Rows[0]["ProcessID"].ToString();
                    Frm_tprc_Main.g_tBase.Process = dt.Rows[0]["Process"].ToString();
                    //m_ProcessID = dt.Rows[0]["ProcessID"].ToString();
                    //m_Process = dt.Rows[0]["Process"].ToString();
                    blResult = true;
                }
                else
                {
                    blResult = false;
                    Frm_tprc_Main.g_tBase.ProcessID = "";
                    Frm_tprc_Main.g_tBase.Process = "";
                }

            }
            catch (Exception excpt)
            {
                blResult = false;
                Frm_tprc_Main.g_tBase.ProcessID = "";
                Frm_tprc_Main.g_tBase.Process = "";
                MessageBox.Show(string.Format("오류! 관리자에게 문의\r\n{0}", excpt.Message));
            }
            return blResult;

        }

        private bool SetPersonID(string strBarcode)
        {
            bool blResult = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("UserID", strBarcode);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_PlanInput_sPersonID", sqlParameter, false);
                DataTable dt = null;

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];

                    //m_PersonID =    dt.Rows[0]["PersonID"].ToString();
                    //m_Person =      dt.Rows[0]["Name"].ToString();
                    //m_TeamID =      dt.Rows[0]["TeamID"].ToString();
                    //m_Team =        dt.Rows[0]["Team"].ToString();

                    Frm_tprc_Main.g_tBase.PersonID = dt.Rows[0]["PersonID"].ToString();
                    Frm_tprc_Main.g_tBase.Person = dt.Rows[0]["Name"].ToString();
                    Frm_tprc_Main.g_tBase.TeamID = dt.Rows[0]["TeamID"].ToString();
                    Frm_tprc_Main.g_tBase.Team = dt.Rows[0]["Team"].ToString();


                    blResult = true;
                }
                else
                {
                    blResult = false;
                    Frm_tprc_Main.g_tBase.PersonID = "";
                    Frm_tprc_Main.g_tBase.Person = "";
                    Frm_tprc_Main.g_tBase.TeamID = "";
                    Frm_tprc_Main.g_tBase.Team = "";
                }

            }
            catch (Exception excpt)
            {
                blResult = false;
                Frm_tprc_Main.g_tBase.PersonID = "";
                Frm_tprc_Main.g_tBase.Person = "";
                Frm_tprc_Main.g_tBase.TeamID = "";
                Frm_tprc_Main.g_tBase.Team = "";
                MessageBox.Show(string.Format("오류! 관리자에게 문의\r\n{0}", excpt.Message));
            }


            return blResult;
        }

        private void SetGridData1RowClear()
        {
            while (GridData1.Rows.Count > 0)
            {
                GridData1.Rows.RemoveAt(0);
            }
        }


        //당일 해당공정의 생산실적 조회
        private void FillGridData1()
        {
            DataRow dr = null;
            DataGridViewRow row = null;

            SetGridData1RowClear();
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("sDate", DateTime.Now.ToString("yyyyMMdd"));
                sqlParameter.Add("ProcessID", Frm_tprc_Main.g_tBase.ProcessID);
                sqlParameter.Add("MachineID", Frm_tprc_Main.g_tBase.MachineID);
                DataSet ds = null;
                ds = DataStore.Instance.ProcedureToDataSet("xp_Work_sResultListbyProcessID", sqlParameter, false);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        GridData1.Rows.Add(i + 1
                                           , dr["LabelID"].ToString().Trim()
                                          );
                        row = GridData1.Rows[i];
                        row.Height = 30;
                    }
                    GridData1.ClearSelection();
                    txtGrd1Count.Text = ds.Tables[0].Rows.Count.ToString() + "건";
                }
                else
                {

                }
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의<FillGridData1>\r\n{0}", excpt.Message), "[오류]", 0, 1);
            }
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            SetGridData2RowClear();
            SetFormDataClear();
        }

        private void FillGridBoxList()
        {
            DataGridViewRow row = null;
            double WorkQty = 0;
            double QtyPerBox = 0;
            double BoxQty = 0;
            double tBoxQty = 0;
            double TempQty = 0;
            bool nMod = false;

            
        }
        /// <summary>
        ///  박스 입력 창 확인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBoxListClose_Click(object sender, EventArgs e)
        {
            UpdatepnlWorkQty();
            ShowBoxList(false);
            return;
        }
        /// <summary>
        ///  박스수량 입력창 확인 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBoxQtyInput_Click(object sender, EventArgs e)
        {
            string strBoxQty = "";
            

        }

        private void UpdatepnlWorkQty()
        {
            int SumQty = 0;
            
        }
        
        private void cmdUpBurnTemper1_Click(object sender, EventArgs e)
        {


            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
            POPUP.Frm_CMNumericKeypad.g_Name = "생산수량";
            return;

        }
        private void cmdDownBurnTemper1_Click(object sender, EventArgs e)
        {

            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            

        }
        private void cmdFormaTime_Click(object sender, EventArgs e)
        {

            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }
        private void cmdUpBurnTemper2_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }

        private void cmdDownBurnTemper2_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }
        private void cmdSetUpBurnTemper_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }
        private void cmdSetDownBurnTemper_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }


        private void cmdSetFormaTime_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMNumericKeypad numkeypad = new POPUP.Frm_CMNumericKeypad();            
        }






        private void txtBarCodeScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
        /// <summary>
        /// ////////////////////
        /// </summary>
        // 바코드 (자동) 스캔.
        private void BarcodeEnter()
        {

            string sInstID = "";
            string sGridArticleID = "";
            string Barcode = txtPreInsertLabelBarCode.Text.Trim(); 

            try
            {               
                // '바코드에 해당하는 Article, LabelGubun을 전역변수에 저장
                if (!BarCodeCheck(Barcode))
                {
                    throw new Exception();
                }
                        
                if (GridData2.RowCount > 0)
                {
                    for (int i = 0; i < GridData2.RowCount; i++)
                    {
                        sGridArticleID = GridData2.Rows[i].Cells["ChildArticleID"].Value.ToString();
                        if (m_ArticleID == sGridArticleID)
                        {
                            //Grid의 단위와 불러온 입고품의 단위를 Grid의 단위에 맞게 맞추기
                            if (GridData2.Rows[i].Cells["UnitClss"].Value.ToString() != m_UnitClss)
                            {
                                string GridUnitClss = GridData2.Rows[i].Cells["UnitClss"].Value.ToString();
                                if (GridUnitClss == "1" && m_UnitClss == "2")//g
                                {
                                    m_LocRemainQty = m_LocRemainQty * 1000;
                                }
                                else if (GridUnitClss == "2" && m_UnitClss == "1")//kg
                                {
                                    m_LocRemainQty = m_LocRemainQty / 1000;
                                }
                            }
                            //if (GridData2.Rows[i].Cells["BarCode"].Value.ToString() == txtPreInsertLabelBarCode.Text.Trim())
                            //{
                            //    GridData2.Rows[i].Cells["ScanExceptYN"].Value = "N";
                            //    GridData2.Rows[i].Cells["BarCode"].Value = "";
                            //    GridData2.Rows[i].Cells["LabelGubun"].Value = "";
                            //    GridData2["BuyerArticle", i].Selected = true;
                            //    GridData2.Rows[i].Cells["RemainQty"].Value = 0;
                            //    GridData2.Rows[i].Cells["LocRemainQty"].Value = 0;
                            //    GridData2.Rows[i].Cells["ProdCapa"].Value = 0;
                            //    GridData2.Rows[i].Cells["EffectDate"].Value = "";
                            //}
                            //else
                            //{
                                GridData2.Rows[i].Cells["ScanExceptYN"].Value = "Y";
                                GridData2.Rows[i].Cells["BarCode"].Value = this.txtPreInsertLabelBarCode.Text.Trim();
                                GridData2.Rows[i].Cells["LabelGubun"].Value = m_LabelGubun;
                                GridData2["BuyerArticle", i].Selected = true;        
                                GridData2.Rows[i].Cells["RemainQty"].Value = string.Format("{0:n0}", m_RemainQty);//전체잔량
                                GridData2.Rows[i].Cells["LocRemainQty"].Value = string.Format("{0:n0}", m_LocRemainQty);//창고잔량
                                double.TryParse(GridData2.Rows[i].Cells["ReqQty"].Value.ToString(), out m_douReqQty);
                                GridData2.Rows[i].Cells["EffectDate"].Value = m_EffectDate;
                                GridData2.Rows[i].Cells["UnitClssName"].Value = m_UnitClssName;         // 하위품의 재고단위
                                GridData2.Rows[i].Cells["ProdUnitClssName"].Value = txtInUnitClss.Text; // 생산되는 생산품의 재고단위

                         
                                m_douProdCapa = m_LocRemainQty / m_douReqQty;
                                if (m_douProdCapa.ToString().Contains("."))
                                {
                                    string[] sProdCapa = m_douProdCapa.ToString().Split('.');
                                    double.TryParse(sProdCapa[0].ToString(), out m_douProdCapa);//소수점 버림
                                }
                                GridData2.Rows[i].Cells["ProdCapa"].Value = string.Format("{0:n0}", m_douProdCapa);
                            //}
                            m_ArticleID = "";
                            m_LabelGubun = "";
                            m_LocRemainQty = 0;
                            m_RemainQty = 0;
                            m_ParentArticleID = "";
                            m_EffectDate = "";
                            m_UnitClssName = "";
                        }
                    }                        
                }                    
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1].Length == 0 ? ex.Message : Message[1], Message[0], 0, 1);
                return;
            }

        }
        //2021-04-05 잔량 불러오기 후 LOT이동처리하기 위해 추가
        private void LotMoveBarcodeEnter()
        {

            string sInstID = "";
            string sGridArticleID = "";
            string Barcode = m_StartSaveLabelID.Trim();

            try
            {
                // '바코드에 해당하는 Article, LabelGubun을 전역변수에 저장
                if (!BarCodeCheck(Barcode))
                {
                    throw new Exception();
                }

                if (GridData2.RowCount > 0)
                {
                    for (int i = 0; i < GridData2.RowCount; i++)
                    {
                        sGridArticleID = GridData2.Rows[i].Cells["ChildArticleID"].Value.ToString();
                        if (m_ArticleID == sGridArticleID)
                        {
                            //Grid의 단위와 불러온 입고품의 단위를 Grid의 단위에 맞게 맞추기
                            if (GridData2.Rows[i].Cells["UnitClss"].Value.ToString() != m_UnitClss)
                            {
                                string GridUnitClss = GridData2.Rows[i].Cells["UnitClss"].Value.ToString();
                                if (GridUnitClss == "1" && m_UnitClss == "2")//g
                                {
                                    m_LocRemainQty = m_LocRemainQty * 1000;
                                }
                                else if (GridUnitClss == "2" && m_UnitClss == "1")//kg
                                {
                                    m_LocRemainQty = m_LocRemainQty / 1000;
                                }
                            }
                            //if (GridData2.Rows[i].Cells["BarCode"].Value.ToString() == txtPreInsertLabelBarCode.Text.Trim())
                            //{
                            //    GridData2.Rows[i].Cells["ScanExceptYN"].Value = "N";
                            //    GridData2.Rows[i].Cells["BarCode"].Value = "";
                            //    GridData2.Rows[i].Cells["LabelGubun"].Value = "";
                            //    GridData2["BuyerArticle", i].Selected = true;
                            //    GridData2.Rows[i].Cells["RemainQty"].Value = 0;
                            //    GridData2.Rows[i].Cells["LocRemainQty"].Value = 0;
                            //    GridData2.Rows[i].Cells["ProdCapa"].Value = 0;
                            //    GridData2.Rows[i].Cells["EffectDate"].Value = "";
                            //}
                            //else
                            //{
                            GridData2.Rows[i].Cells["ScanExceptYN"].Value = "Y";
                            GridData2.Rows[i].Cells["BarCode"].Value = m_StartSaveLabelID.Trim();
                            GridData2.Rows[i].Cells["LabelGubun"].Value = m_LabelGubun;
                            GridData2["BuyerArticle", i].Selected = true;
                            GridData2.Rows[i].Cells["RemainQty"].Value = string.Format("{0:n0}", m_RemainQty);//전체잔량
                            GridData2.Rows[i].Cells["LocRemainQty"].Value = string.Format("{0:n0}", m_LocRemainQty);//창고잔량
                            double.TryParse(GridData2.Rows[i].Cells["ReqQty"].Value.ToString(), out m_douReqQty);
                            GridData2.Rows[i].Cells["EffectDate"].Value = m_EffectDate;
                            GridData2.Rows[i].Cells["UnitClssName"].Value = m_UnitClssName;         // 하위품의 재고단위
                            GridData2.Rows[i].Cells["ProdUnitClssName"].Value = txtInUnitClss.Text; // 생산되는 생산품의 재고단위


                            m_douProdCapa = m_LocRemainQty / m_douReqQty;
                            if (m_douProdCapa.ToString().Contains("."))
                            {
                                string[] sProdCapa = m_douProdCapa.ToString().Split('.');
                                double.TryParse(sProdCapa[0].ToString(), out m_douProdCapa);//소수점 버림
                            }
                            GridData2.Rows[i].Cells["ProdCapa"].Value = string.Format("{0:n0}", m_douProdCapa);
                            //}
                            m_ArticleID = "";
                            m_LabelGubun = "";
                            m_LocRemainQty = 0;
                            m_RemainQty = 0;
                            m_ParentArticleID = "";
                            m_EffectDate = "";
                            m_UnitClssName = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1].Length == 0 ? ex.Message : Message[1], Message[0], 0, 1);
                return;
            }

        }

        // workLog 가져올거 있으면 가져오고 뿌리고 해야 함.
        private void Find_Collect_WorkLogData()
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WorkLog_sCollectWorkLogData", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 공정과 호기정보가 일치한다면,
                        if (m_ProcessID == dr["ProcessID"].ToString() &&
                            Frm_tprc_Main.g_tBase.MachineID == dr["MachineID"].ToString())
                        {
                            txtFacilityCollectQty.Text = Lib.CheckNull(dr["WorkQty"].ToString().Trim());
                            break;
                        }
                    }

                    //if (txtFacilityCollectQty.Text != string.Empty)
                    //{
                        // worklog에서 값을 가져왔다면,
                        // 지금 가져온 값이 오늘하루의 총 작업수량이 될 테니까,

                    //}
                }


            }
            catch (Exception)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                return;
            }

        }








        private void btnStartTime_Click(object sender, EventArgs e)
        {
            TimeCheck("시작시간");
        }

        private void btnEndTime_Click(object sender, EventArgs e)
        {
            TimeCheck("종료시간");
        }

        private void TimeCheck(string strTime)
        {
            POPUP.Frm_CMNumericKeypad FK = new POPUP.Frm_CMNumericKeypad(strTime);
            FK.Owner = this;
            string sTime = "";
            DateTime dt = DateTime.Now;
            if (FK.ShowDialog() == DialogResult.OK)
            {

                sTime = FK.InputTextValue;
                if (sTime != "")
                {
                    dt = DateTime.ParseExact(sTime, "HHmmss", null);
                }
            }

            if (strTime == "시작시간")
            {
                dtStartTime.Value = dt;
            }
            else if (strTime == "종료시간")
            {
                dtEndTime.Value = dt;
            }
        }

        private void txtWorkQty_TextChanged(object sender, EventArgs e)
        {            

            int intInstQty = 0;
            int intWorkQty = 0;
            int intInstRemainQty = 0;
            int.TryParse(Lib.GetDouble(txtlInstQty.Text).ToString(), out intInstQty);            

            intInstRemainQty = intInstQty - intWorkQty;
            //this.txtInstRemainQty.Text = string.Format("{0:n0}", intInstRemainQty);
        }

        private void txtProdQty_TextChanged(object sender, EventArgs e)
        {
            ProdQty = Lib.CheckNum(txtProdQty.Text).Replace(",", "");
        }


        private void txtWorkQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            WizWorkLib.TypingOnlyNumber(sender, e, true, false);
        }

        private void txtQtyPerBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            WizWorkLib.TypingOnlyNumber(sender, e, true, false);
        }

        private void txtBoxQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            WizWorkLib.TypingOnlyNumber(sender, e, true, false);
        }
        //boolean 값을 받기위해 private -> public 으로 수정 18.01.15
        public bool LF_ChkMachineCheck()
        {
            // '***************************************************************
            // '0:공정작업입력 시 설비 점검(하루1회이상) 및 자주검사 수행(작업지시별 1회이상) check
            // '***************************************************************
            bool blResult = false;
            bool bFirst = false;

            string strMachine = "";
            string[] MachineTemp = null;

            DataSet ds = null;
            string strMessage = "";
            string strMessageInspect = "";

            int intResult = 0;
            int intNoWorkTime = 0;
            int inAutoInspect = 0;

            Tools.INI_GS gs = new Tools.INI_GS();

            strMachine =Frm_tprc_Main.gs.GetValue("Work", "Machine", "");

            if (strMachine != "")
            {
                MachineTemp = strMachine.Split('|');//머신
                foreach (string str in MachineTemp)
                {
                    if (str == m_ProcessID + Frm_tprc_Main.g_tBase.MachineID)
                    {
                        Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                        sqlParameter.Add("ProcessID", m_ProcessID);
                        sqlParameter.Add("MachineID", m_MachineID);
                        sqlParameter.Add("PLotID", m_LotID);

                        DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_sToDayMcRegularInspectAutoYN", sqlParameter, false);
                        if (dt != null && dt.Rows.Count == 1)
                        {

                            DataRow dr = null;
                            dr = ds.Tables[0].Rows[0];

                            int.TryParse(dr["result"].ToString(), out intResult);

                            if (intResult < 0)
                            {
                                MessageBox.Show(string.Format("처리할 공정 및 호기를 설정하세요.", "공정및호기 설정오류"));
                                return blResult;
                            }

                            int.TryParse(dr["NoWorkTime"].ToString(), out intNoWorkTime);//'계획정지시간 이 없는 건만 Check

                            if (intNoWorkTime == 0)
                            {
                                if (intResult == 0)
                                {
                                    if (bFirst == true)
                                    {
                                        strMessage = dr["McName"].ToString().Trim();
                                    }
                                    else
                                    {
                                        if (strMessage == "")
                                        {
                                            strMessage = dr["McName"].ToString().Trim();
                                        }
                                        else
                                        {
                                            strMessage = strMessage + ",  " + dr["McName"].ToString().Trim();
                                        }
                                    }

                                }
                                int.TryParse(dr["AutoInspect"].ToString(), out inAutoInspect);
                                if (inAutoInspect == 0)
                                {
                                    if (bFirst == true)
                                    {
                                        strMessageInspect = dr["McName"].ToString().Trim();

                                    }
                                    else
                                    {
                                        if (strMessageInspect == "")
                                        {
                                            strMessageInspect = dr["McName"].ToString().Trim();
                                        }
                                        else
                                        {
                                            strMessageInspect = strMessageInspect + ",  " + dr["McName"].ToString().Trim();
                                        }
                                    }
                                    if (strMessage != "")
                                    {
                                        Message[0] = "[설비점검 오류]";
                                        Message[1] = strMessage + "의 설비점검을 하셔야합니다.";
                                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                                        //timer1.Stop();
                                        return false;
                                    }
                                    if (strMessageInspect != "")
                                    {
                                        Message[0] = "[자주검사 오류]";
                                        Message[1] = strMessageInspect + "의 자주검사를 하셔야합니다.";
                                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                                        //timer1.Stop();
                                        return false;
                                    }
                                }
                            }

                        }
                        else
                        {
                            Message[0] = "[공정및호기 설정오류]";
                            Message[1] = "처리할 공정 및 호기를 설정하세요.";
                            WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void save2101()
        {
            list_TWkResult = new List<Sub_TWkResult>();
            int InstDetSeq = 0;
            int i = 0;
            float sLogID = 0;
            float WorkQty = 0;
            //성형 금형정보 
            float UpBurnPlateTemper1 = 0;
            float DownBurnPlateTemper1 = 0;
            float UpBurnPlateTemper2 = 0;
            float DownBurnPlateTemper2 = 0;
            float SetUpBurnPlateTemper = 0;
            float SetDownBurnPlateTemper = 0;
            float FormaTime = 0;
            float SetFormaTime = 0;
            float SetFormaOpenTime = 0;
            float FormaOpenTime = 0;

            list_TWkResult.Add(new Sub_TWkResult());
            list_TWkResult[i].JobID = 0;
            list_TWkResult[i].InstID = txtInstID.Text;
            int.TryParse(Lib.GetDouble(txtInstDetSeq.Text).ToString(), out InstDetSeq);
            list_TWkResult[i].InstDetSeq = InstDetSeq;
            list_TWkResult[i].LabelID = txtBoxID.Text;
            list_TWkResult[i].LabelGubun = txtLabelGubun.Text;
            list_TWkResult[i].ProcessID = Frm_tprc_Main.g_tBase.ProcessID;
            list_TWkResult[i].MachineID = Frm_tprc_Main.g_tBase.MachineID;
            list_TWkResult[i].ArticleID = txtArticleID.Text;
            list_TWkResult[i].WorkQty = WorkQty;
            list_TWkResult[i].sLastArticleYN = m_LastArticleYN;
            list_TWkResult[i].ProdAutoInspectYN = m_ProdAutoInspectYN;
            list_TWkResult[i].sOrderID = m_OrderID;
            list_TWkResult[i].nOrderSeq = m_OrderSeq;

            list_TWkResult[i].WorkStartDate = mtb_From.Text.Replace("-", "");
            list_TWkResult[i].WorkStartTime = dtStartTime.Value.ToString("HHmmss");
            list_TWkResult[i].WorkEndDate = mtb_To.Text.Replace("-", "");
            list_TWkResult[i].WorkEndTime = dtEndTime.Value.ToString("HHmmss");
            list_TWkResult[i].ScanDate = list_TWkResult[i].WorkEndDate;
            list_TWkResult[i].ScanTime = list_TWkResult[i].WorkEndTime;
            list_TWkResult[i].JobGbn = "1";

            //'------------------------------------------------------------------------------------------



            //'------------------------------------------------------------------------------------------

            list_TWkResult[i].Comments = "";
            list_TWkResult[i].ReworkOldYN = "";
            list_TWkResult[i].ReworkLinkProdID = "";
            list_TWkResult[i].CreateUserID = Frm_tprc_Main.g_tBase.PersonID;
            list_TWkResult[i].WDNO = "";
            list_TWkResult[i].WDID = "";
            list_TWkResult[i].WDQty = 0;
            list_TWkResult[i].s4MID = "";
            float.TryParse(m_LogID, out sLogID);
            list_TWkResult[i].sLogID = sLogID;


        }

        #region 달력 From값 입력 // 달력 창 띄우기
        private void mtb_From_Click(object sender, EventArgs e)
        {
            WizCommon.Popup.Frm_TLP_Calendar calendar = new WizCommon.Popup.Frm_TLP_Calendar(mtb_From.Text.Replace("-", ""), mtb_From.Name);
            calendar.WriteDateTextEvent += new WizCommon.Popup.Frm_TLP_Calendar.TextEventHandler(GetDate);
            calendar.Owner = this;
            calendar.ShowDialog();
        }
        #endregion
        #region 달력 To값 입력 // 달력 창 띄우기
        private void mtb_To_Click(object sender, EventArgs e)
        {
            WizCommon.Popup.Frm_TLP_Calendar calendar = new WizCommon.Popup.Frm_TLP_Calendar(mtb_To.Text.Replace("-", ""), mtb_To.Name);
            calendar.WriteDateTextEvent += new WizCommon.Popup.Frm_TLP_Calendar.TextEventHandler(GetDate);
            calendar.Owner = this;
            calendar.ShowDialog();
        }
        #endregion
        #region Calendar.Value -> mtbBox.Text 달력창으로부터 텍스트로 값을 옮겨주는 메소드
        private void GetDate(string strDate, string btnName)
        {
            DateTime dateTime = new DateTime();
            dateTime = DateTime.ParseExact(strDate, "yyyyMMdd", null);
            if (btnName == mtb_From.Name)
            {
                mtb_From.Text = dateTime.ToString("yyyy-MM-dd");
            }
            else if (btnName == mtb_To.Name)
            {
                mtb_To.Text = dateTime.ToString("yyyy-MM-dd");
            }

        }
        #endregion

        private void pnlBarcode_Paint(object sender, PaintEventArgs e)
        {

        }


        #region 수량 / CycleTime 기입용 숫자 키패드 팝업창 모음.

        // 작업수량 기입 팝업창 생성
        private void chkWorkQty_Click(object sender, EventArgs e)
        {
            double DOU_WorkQty = 0;

            txtWorkQty.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("수량입력", "수량");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtWorkQty.Text = keypad.tbInputText.Text;
                if (txtWorkQty.Text == "" || Convert.ToInt32(txtWorkQty.Text) == 0)
                {
                    txtWorkQty.Text = "0";
                }
                Double.TryParse(txtWorkQty.Text, out DOU_WorkQty);
                txtWorkQty.Text = string.Format("{0:n0}", (int)DOU_WorkQty);
            }
            Get_TotalQty();
        }
        // 작업수량 기입 팝업창 생성
        private void txtWorkQty_Click(object sender, EventArgs e)
        {
            double DOU_WorkQty = 0;

            txtWorkQty.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("수량입력", "수량");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtWorkQty.Text = keypad.tbInputText.Text;
                if (txtWorkQty.Text == "" || Convert.ToInt32(txtWorkQty.Text) == 0)
                {
                    txtWorkQty.Text = "0";
                }
                Double.TryParse(txtWorkQty.Text, out DOU_WorkQty);
                txtWorkQty.Text = string.Format("{0:n0}", (int)DOU_WorkQty);
            }
            Get_TotalQty();
        }

        // 생산박스 당 수량 기입 팝업창 생성
        private void chkLotProdQty_Click(object sender, EventArgs e)
        {
            double DOU_LotProdQty = 0;

            txtLotProdQty.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("생산수량", "생산수량");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtLotProdQty.Text = keypad.tbInputText.Text;
                if (txtLotProdQty.Text == "" || Convert.ToInt32(txtLotProdQty.Text) == 0)
                {
                    txtLotProdQty.Text = "0";
                }
                Double.TryParse(txtLotProdQty.Text, out DOU_LotProdQty);
                txtLotProdQty.Text = string.Format("{0:n0}", (int)DOU_LotProdQty);
            }
        }
        // 생산박스 당 수량 기입 팝업창 생성
        private void txtLotProdQty_Click(object sender, EventArgs e)
        {
            double DOU_LotProdQty = 0;

            txtLotProdQty.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("생산수량", "생산수량");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtLotProdQty.Text = keypad.tbInputText.Text;
                if (txtLotProdQty.Text == "" || Convert.ToInt32(txtLotProdQty.Text) == 0)
                {
                    txtLotProdQty.Text = "0";
                }
                Double.TryParse(txtLotProdQty.Text, out DOU_LotProdQty);
                txtLotProdQty.Text = string.Format("{0:n0}", (int)DOU_LotProdQty);
            }
        }


        // CycleTime 기입 팝업창 생성
        private void chkCycleTime_Click(object sender, EventArgs e)
        {
            double DOU_CycleTime = 0;

            txtCycleTime.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("C-T", "C-T");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtCycleTime.Text = keypad.tbInputText.Text;
                if (txtCycleTime.Text == "" || Convert.ToInt32(txtCycleTime.Text) == 0)
                {
                    txtCycleTime.Text = "0";
                }
                Double.TryParse(txtCycleTime.Text, out DOU_CycleTime);
                txtCycleTime.Text = string.Format("{0:n0}", (int)DOU_CycleTime);
            }
        }
        // CycleTime 기입 팝업창 생성
        private void txtCycleTime_Click(object sender, EventArgs e)
        {
            double DOU_CycleTime = 0;

            txtCycleTime.Text = "";
            POPUP.Frm_CMNumericKeypad keypad = new POPUP.Frm_CMNumericKeypad("C-T", "C-T");

            keypad.Owner = this;
            if (keypad.ShowDialog() == DialogResult.OK)
            {
                txtCycleTime.Text = keypad.tbInputText.Text;
                if (txtCycleTime.Text == "" || Convert.ToInt32(txtCycleTime.Text) == 0)
                {
                    txtCycleTime.Text = "0";
                }
                Double.TryParse(txtCycleTime.Text, out DOU_CycleTime);
                txtCycleTime.Text = string.Format("{0:n0}", (int)DOU_CycleTime);
            }
        }

        #endregion



        // 합계수량 구해내기.
        private void Get_TotalQty()
        {
            double My_WorkQty = 0;
            double Your_RemainQty = 0;
            double TotalQty = 0;

            double.TryParse(txtWorkQty.Text, out My_WorkQty);
            double.TryParse(txtRemainAdd.Text, out Your_RemainQty);

            TotalQty = My_WorkQty + Your_RemainQty;
            txtTotalLabelQty.Text = TotalQty.ToString();
        }



        // 오른쪽 작업(취소)(삭제)(파기)??? 버튼 신규생성.
        private void btnWorkingDestory_Click(object sender, EventArgs e)
        {
            Message[0] = "[작업삭제]";
            Message[1] = "생산작업을 진행중이였습니다. 작업을 삭제하시겠습니까?";
            if (WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 0) == DialogResult.OK)
            {
                //1. 현재아이 정보.
                float NowJobID = float.Parse(updateJobID);
                string YLabelID = m_LabelID;

                try
                {
                    //2. 삭제 프로시저.
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Add("JobID", NowJobID);
                    sqlParameter.Add("YLabelID", YLabelID);
                    DataStore.Instance.ProcedureToDataSet("[xp_WizWork_dWkResult_YellowIng]", sqlParameter, true);
                    DataStore.Instance.CloseConnection(); //2021-09-23 DB 커넥트 연결 해제
                    //3. 다시 목록으로 복귀.
                    cmdExit_Click(null, null);
                }
                catch (Exception EX)
                {
                    Message[0] = "[작업삭제]";
                    Message[1] = "작업삭제에 실패하였습니다. JobID: " + NowJobID + ",라벨: " + YLabelID + "\r\n" +
                        EX.Message.ToString();
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                    return;
                }
            }
        }


        // 오른쪽 잔량 불러오기 버튼 클릭.
        private void btnBringSplitData_Click(object sender, EventArgs e)
        {
            // 잔량 불러오기 버튼 기능	2021-07-27 안씀
            if (btnBringSplitData.Text.Trim().Replace(" ", "").Contains("잔량"))
            {
                //string BringArticleID = this.txtArticleID.Text;
                //string InstID = this.txtInstID.Text; //2021-07-01

                //frm_PopUp_BringSplitData2 BringSplitData = new frm_PopUp_BringSplitData2(BringArticleID, txtBuyerArticleNo.Text, InstID); //2021-07-01
                //BringSplitData.Owner = this;
                //BringSplitData.WriteTextEvent += BringSplitData_WriteTextEvent;

                //void BringSplitData_WriteTextEvent(string Sum_SplitUsingQty)
                //{
                //    double D_Sum_SplitUsingQty = 0;
                //    double.TryParse(Sum_SplitUsingQty, out D_Sum_SplitUsingQty);
                //    txtRemainAdd.Text = D_Sum_SplitUsingQty.ToString();
                //    Get_TotalQty();
                //}

                //BringSplitData.ShowDialog();

                //FillGridData2(Frm_tprc_Main.list_g_tsplit[0].InstID, Frm_tprc_Main.g_tBase.ProcessID); //2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가
                //string StartSaveLabelIDSQL = "select StartSaveLabelID from wk_result where LabelID =  '" + Frm_tprc_Main.list_g_tsplit[0].LabelID + "'"; //2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가
                //string[] StartSaveLabelID = new string[2];//2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가
                //StartSaveLabelID = DataStore.Instance.ExecuteQuery(StartSaveLabelIDSQL, false);//2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가
                //m_StartSaveLabelID = StartSaveLabelID[1];//2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가

                //LotMoveBarcodeEnter();//2021-04-05 잔량 이동처리 후 LOT이동처리를 위해 추가

                return;
            } // 일괄 스캔 기능	
            else if (btnBringSplitData.Text.Trim().Replace(" ", "").Contains("일괄"))
            {
                frm_PopUp_PreScanWork2 FPPSW = new frm_PopUp_PreScanWork2(Frm_tprc_Main.g_tBase.ProcessID, Frm_tprc_Main.g_tBase.MachineID, txtPreInsertLabelBarCode.Text
                    , lstLabelList, lstQty,lstLabelListMove, lstQtyMove, Frm_tprc_Main.g_tBase.Process);
                FPPSW.StartPosition = FormStartPosition.CenterScreen;
                FPPSW.BringToFront();
                FPPSW.TopMost = true;
                if (FPPSW.ShowDialog() == DialogResult.OK)
                {
                    this.lstQty = FPPSW.lstQty;
                    this.lstLabelList = FPPSW.lstLabelList;
                    //2021-10-07 외주생산이동에서 일괄스캔 시 사내로 이동 시키기 위해 저장
                    this.lstLabelListMove = FPPSW.lstLabelListMove;
                    this.lstQtyMove = FPPSW.lstQtyMove;

                    this.AnotherQty = FPPSW.AnotherQty;
                    txtRemainAdd.Text = stringFormatN0(AnotherQty);
                    btnBringSplitData.Text = "일괄 스캔(" + lstLabelList.Count + ")";
                    Get_TotalQty();
                }
                return;
            }
        }




        // (실제는 체크, 모양은 버튼) 체크형 버튼들 > 체크하더라도 본 색깔 유형 그대로 유지하도록.
        private void checkBox_CheckedPrevent(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Checked = false;
            }
            else
            {
                ((CheckBox)sender).Checked = false;
            }
        }

        #region 기타 메서드 모음

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        private string stringFormatN1(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // 천마리 콤마, 소수점 두자리
        private string stringFormatN2(object obj)
        {
            return string.Format("{0:N2}", obj);
        }

        // 데이터피커 포맷으로 변경
        private string DatePickerFormat(string str)
        {
            string result = "";

            if (str.Length == 8)
            {
                if (!str.Trim().Equals(""))
                {
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                }
            }

            return result;
        }

        // 시간 형식 6글자라면! 11:11:11
        private string DateTimeFormat(string str)
        {
            str = str.Replace(":", "").Trim();

            if (str.Length == 6)
            {
                string Hour = str.Substring(0, 2);
                string Min = str.Substring(2, 2);
                string Sec = str.Substring(4, 2);

                str = Hour + ":" + Min + ":" + Sec;
            }
            else if (str.Length == 4)
            {
                string Hour = str.Substring(0, 2);
                string Min = str.Substring(2, 2);

                str = Hour + ":" + Min;
            }

            return str;
        }

        // Int로 변환
        private int ConvertInt(string str)
        {
            int result = 0;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    result = Int32.Parse(str);
                }
            }

            return result;
        }

        // 소수로 변환 가능한지 체크 이벤트
        private bool CheckConvertDouble(string str)
        {
            bool flag = false;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                if (Double.TryParse(str, out chkDouble) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 숫자로 변환 가능한지 체크 이벤트
        private bool CheckConvertInt(string str)
        {
            bool flag = false;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Trim().Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 소수로 변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Double.TryParse(str, out chkDouble) == true)
                {
                    result = Double.Parse(str);
                }
            }

            return result;
        }

        #endregion

        
    }
}