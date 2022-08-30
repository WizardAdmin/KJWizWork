﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WizCommon;

//*******************************************************************************
//프로그램명    frm_PopUp_PreScanWork.cs
//메뉴ID        
//설명          frm_PopUp_PreScanWork 메인소스입니다.
//작성일        2019.07.29
//개발자        허윤구
//*******************************************************************************
// 변경일자     변경자      요청자      요구사항ID          요청 및 작업내용
//*******************************************************************************
//  19_0729     허윤구  * 성형 하나에 재단 2개가 필요한 케이스에 따라 수정보완.
//                          (InsertX)
//  2019.08.01 > 허윤구    FMB가 어떤 이유로 이미 재단창고에 가 있을 케이스에 대한 로직추가.
//*******************************************************************************

namespace WizWork
{
    public partial class frm_PopUp_PreScanWork : Form
    {

        private string m_ProcessID = "";        //공정id
        private string m_MachineID = "";        //머신id
        private string m_ArticleID = "";        //품명id   

        private string m_LabelGubun = "";       //라벨구분  
        private string m_Inspector = "";        //검사자
        private string m_MoldID = "";           //몰드. 금형ID

        private string m_UnitClss = "";         // 단위
        private string m_UnitClssName = "";
        private string m_EffectDate = "";       // 유효일? 효과일?    (스캔 후 초기화)     
        private string m_MtrExceptYN = "";      // 예외처리 체크용도

        private double m_RemainQty = 0;         //      입고수량  (스캔 후 초기화)
        private double m_LocRemainQty = 0;      //    '자품목 현 재고량  (스캔 후 초기화)
        //2019.06.14 허윤구 추가.
        private string WH_AR_m_LocID = "";      //     현재창고 위치 체크. > 에러구문 상세화 목적.

        //2020.01.07 허윤구 추가.
        private string Wh_Ar_ChildArticleID = "";  // 내가 스캔한 제품의 부모ArticleID >> 즉, 작지의 ArticleID

        //2020.01.07 허윤구 추가.
        private string Wh_Ar_InstID = "";       // PL_Input 작지의 해당 대상 InstID.
        private string Wh_Ar_InstID_Seq = "";   // PL_Input 작지의 해당 대상 InstID_Seq.

        private double m_douReqQty = 0;         //현재품목 소요량
        private double m_douProdCapa = 0;


        string[] Message = new string[2];  // 메시지박스 처리용도.
        bool YLabelOK = false;          //바코드 스캔 시 Y라벨로 넣을 것인지에 대한 BOOL값
        int InsertX = 0;                // 바코드 스캔 시 스캔해야 하는 하위품의 갯수 체크용. (X개가 들어갔는지)

        string WhAr_MoveArticle_I = string.Empty;    // I_FMB_재단 자동이동시의 Article.(Whole-Area)
        List<WizCommon.Procedure> Prolist = null;
        List<Dictionary<string, object>> ListParameter = null;

        WizWorkLib Lib = new WizWorkLib();


        public frm_PopUp_PreScanWork()
        {
            InitializeComponent();
            SetScreen();  //TLP 사이즈 조정
        }
        public frm_PopUp_PreScanWork(string strProcessID, string strMachineID, string strMoldID)
        {
            InitializeComponent();
            m_ProcessID = strProcessID;
            m_MachineID = strMachineID;
            m_MoldID = strMoldID;
            SetScreen();  //TLP 사이즈 조정
        }


        #region 테이블 레이아웃 패널 사이즈 조정
        private void SetScreen()
        {
            tlpMain.Dock = DockStyle.Fill;
            foreach (Control control in tlpMain.Controls)
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(1, 1, 1, 1);
                foreach (Control contro in control.Controls)
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
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion


        //로드.
        private void frm_PopUp_PreScanWork_Load(object sender, EventArgs e)
        {
            // 전체 클리어.
            SetFormDataClear();

            // 시작처리 버튼숨기기.
            tlpBottomChoice.ColumnStyles[1].Width = 0;
            tlpBottomChoice.ColumnStyles[2].Width = 0;

            // 시작작업 처리
            Form_Activate();          
        }

        // After Load
        private void frm_PopUp_PreScanWork_Shown(object sender, EventArgs e)
        {
            // 포커스
            txtBarCodePreScan.Focus();
            txtBarCodePreScan.Select(0, 0);
            txtBarCodePreScan.Focus();
        }


        #region 전체 클리어
        private void SetFormDataClear()
        {
            this.txtBarCodePreScan.Text = string.Empty;
            this.txtArticle.Text = string.Empty;
            this.txtBuyerArticleNo.Text = string.Empty;

            // 스캔체크에 통과할때까지 '시작처리' 버튼은 사용불가. 
            btnOK.Enabled = false;
        }

        #endregion


        #region Form_Activate 묶음

        #region 시작작업
        private void Form_Activate()
        {
            try
            {
                Wh_Ar_InstID = CheckLabelID(Frm_tprc_Main.g_tBase.sLotID);
            }
            catch(Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
            }
            
        }

        #endregion

        #region  시작용 체크 + Textbox 채우기.
        private string CheckLabelID(string strBarCode)
        {
            string strInstID = "";
            string strInstDetSeq = "";

            try
            {
                string sMoldID = "";

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PLotID", strBarCode);
                sqlParameter.Add("ProcessID", m_ProcessID); //SearchProcessID());                
                sqlParameter.Add("MoldID", sMoldID);

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_Chkworklotid", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    m_MtrExceptYN = Lib.CheckNull(dr["MtrExceptYN"].ToString());//PLotID가 라벨일때 pl_input의 MtrExceptYN                    
                    strInstID = Lib.CheckNull(dr["InstID"].ToString());//PLotID가 라벨일때 pl_input의 InstID                                        
                    strInstDetSeq = Lib.CheckNull(dr["InstDetSeq"].ToString());
                    Wh_Ar_InstID_Seq = strInstDetSeq;

                    double InstQty = 0;
                    double InstWorkQty = 0;
                    double InstRemainQty = 0;

                    double.TryParse(dr["InstQty"].ToString(), out InstQty);
                    double.TryParse(dr["InstWorkQty"].ToString(), out InstWorkQty);
                    InstRemainQty = InstQty - InstWorkQty;                    

                    Frm_tprc_Main.g_tBase.sArticleID = Lib.CheckNull(dr["ArticleID"].ToString());//mt_article
                    Frm_tprc_Main.g_tBase.Article = Lib.CheckNull(dr["pldArticle"].ToString());//mt_article
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
                    Frm_tprc_Main.g_tBase.Basis = "";
                    Frm_tprc_Main.g_tBase.BasisID = 0;

                    //m_UnitClss = Lib.CheckNull(dr["UnitClss"].ToString());//pl_inputdet articleid의 UnitClss
                    //m_UnitClssName = Lib.CheckNull(dr["UnitClssName"].ToString());

                    txtArticle.Text = Lib.CheckNull(dr["pldArticle"].ToString());

                    txtBuyerArticleNo.Text = Lib.CheckNull(dr["BuyerArticleNo"].ToString());                   

                }

            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", excpt.Message), "[오류]", 0, 1);
                return "";
            }

            return strInstID;
        }

        #endregion

        #endregion


        #region 스캔 묶음

        #region 스캔

        // 스캔 텍스트 체크.
        private void txtBarCodePreScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)13)
                {
                    txtBarCodePreScan.Text = txtBarCodePreScan.Text.Trim().ToUpper();
                    if (BarcodeEnter())
                    {
                        btnOK_Click(null, null);
                    }
                    //txtBarCodePreScan.Focus();
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(ex.Message.ToString(), "[오류]", 0, 1);
            }            
        }

        // 스캔버튼 클릭.
        private void cmdBarCodePreScan_Click(object sender, EventArgs e)
        {
            POPUP.Frm_CMKeypad.g_Name = "바코드 스캔";
            POPUP.Frm_CMKeypad FK = new POPUP.Frm_CMKeypad();
            POPUP.Frm_CMKeypad.KeypadStr = txtBarCodePreScan.Text.Trim();
            if (FK.ShowDialog() == DialogResult.OK)
            {
                txtBarCodePreScan.Text = FK.tbInputText.Text;
                if (BarcodeEnter())
                {
                    btnOK_Click(null, null);
                }
                //txtBarCodePreScan.Focus();
            }
            else
            {
                txtBarCodePreScan.Text = string.Empty;
            }
        }

        #endregion

        #region 스캔 바코드 기반 데이터 가져오기

        // 스캔 바코드 기반 조건체크 및 데이터 가져오기.
        private bool BarcodeEnter()
        {
            Wh_Ar_ChildArticleID = string.Empty;

            string sInstID = "";
            string sGridArticleID = "";
            string Barcode = txtBarCodePreScan.Text.Trim();

            try
            { 
                // 1. 스켄 ID  기본 정상여부 확인
                if (!(LF_Check_ScanData(Barcode)))
                {
                    Message[0] = "[바코드오류]";
                    Message[1] = "바코드 형식이 정상적이지 않습니다.";
                    throw new Exception();
                }

                //2. 바코드 체크. 
                if (!(BarCodeCheck(Barcode)))
                {
                    Message[0] = "[바코드체크]";
                    Message[1] = "바코드에 기반한 데이터를 찾지 못했습니다. \r\n" +
                        "바코드 정보와 공정코드를 확인해 주세요.";
                    throw new Exception();
                }
                if (Wh_Ar_ChildArticleID != m_ArticleID)
                {
                    Message[0] = "[스캔오류]";
                    Message[1] = "하위품이 아니거나 잘못된 바코드입니다. \r\n" +
                        Wh_Ar_ChildArticleID + "미일치" + m_ArticleID;
                    throw new Exception();
                }
                if ((m_RemainQty == 0) || (m_RemainQty < 0))
                {
                    Message[0] = "[라벨오류]";
                    Message[1] = "입력된 롯트는 현재 재고량이 0이하입니다.\r\n" +
                                   "재고를 모두 소진하였습니다.";
                    throw new Exception();
                }
                return true;               
            }

            catch (Exception)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                WH_AR_m_LocID = "";
                return false;
            }
        }

        #endregion

        #region LF_Check_ScanData 펑션
        /// <summary>
        /// 스켄 ID  기본 정상여부 확인
        /// </summary>
        /// <returns></returns>
        private bool LF_Check_ScanData(string strBarcode)
        {
            bool blResult = true;
            if (strBarcode != "")
            {
                // 입고 원자재라면, I로 시작하겠지.
                if (strBarcode.ToUpper().Contains("I"))
                {
                    //'입고 Stuffinsub LotID 11자리
                    if (!(strBarcode.Trim().Length == 11))
                    {
                        WizCommon.Popup.MyMessageBox.ShowBox("코드가 잘못되었습니다.", "[바코드 길이오류]", 0, 1);
                        txtBarCodePreScan.Text = "";
                        blResult = false;
                    }
                    return blResult;
                }
                // 첫번째 공정에서 나온 라벨은 뭘로 시작해야 하나요??
                else if (strBarcode.ToUpper().Contains("C"))
                {
                    //'공정 wk_Label LotID 11자리
                    if (!(strBarcode.Trim().Length == 11))
                    {
                        WizCommon.Popup.MyMessageBox.ShowBox("코드가 잘못되었습니다.", "[바코드 길이오류]", 0, 1);
                        txtBarCodePreScan.Text = "";
                        blResult = false;
                    }
                    return blResult;
                }
                else
                { blResult = false; }
            }
            blResult = false;
            return blResult;
        }

        #endregion
        
        #region BarCodeCheck 펑션

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

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("LotID", strBarCode);
                sqlParameter.Add("ProcessID", m_ProcessID);
                sqlParameter.Add("MachineID", m_MachineID);
                sqlParameter.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                sqlParameter.Add("InstDetSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);
                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_WizWork_sLotInfoByLotID", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dr = dt.Rows[0];  

                    m_ArticleID = dr["ArticleID"].ToString().Trim();
                    m_LabelGubun = dr["LabelGubun"].ToString().Trim();                    //라벨구분 1은 원자재 
                    m_Inspector = dr["Inspector"].ToString().Trim();                      //검사자
                    double.TryParse(dr["RemainQty"].ToString(), out m_RemainQty);         //전체재고량
                    double.TryParse(dr["LocRemainQty"].ToString(), out m_LocRemainQty);   //해당창고재고량
                    WH_AR_m_LocID = dr["LocID"].ToString().Trim();                        //현재 위치하고 있는 창고.
                    m_UnitClss = dr["UnitClss"].ToString();
                    m_EffectDate = Lib.MakeDateTime("yyyyMMdd", dr["EffectDate"].ToString());

                    if (m_MtrExceptYN == "N")//예외처리YN : 예외처리 아닐때
                    {                   

                    }
                    if (Find_BOM_Child())
                    {
                        return true;
                    }
                    else
                    {
                        // BOM_Child에 실패할 수 있는 케이스.

                        // 1. 작지번호상 Seq가 있는데, ArticleChild에 동일 DetSeq가 없는 경우.
                        string[] DetCountYN = new string[2];
                        string Query = "select  cnt = COUNT(*)  from pl_InputDetArticleChild  where InstID =  '" + Wh_Ar_InstID + "'  and DetSeq = '" + Wh_Ar_InstID_Seq + "'  ";
                        DetCountYN = DataStore.Instance.ExecuteQuery(Query, false);
                        if (Lib.CheckNull(DetCountYN[1]).ToString() == "0")
                        {
                            Message[0] = "[작업지시 등록오류]";
                            Message[1] = "작업지시 순서와 작업하위품 순서등록이 일치하지 않습니다. \r\n" +
                                "위저드정보시스템으로 문의해 주세요.(pl_InputDetArticleChild) \r\n" +
                                "InstID= " + Wh_Ar_InstID + ", DetSeq= " + Wh_Ar_InstID_Seq;
                            throw new Exception();
                        }
                        else
                        {
                            Message[0] = "[bom 실패]";
                            Message[1] = "해당 품목의 상위 BOM으로 작업지시를 내리지 않았습니다. \r\n" +
                                "BOM 변경 후, 새로운 작업지시를 내려주세요.";
                            throw new Exception();
                        }                        
                    }
                    
                }
                else
                {
                    // dt.rows count가 0 이야.
                    // 근데, 여기 들어오는 케이스가 현재 발견된게 지금 3건.
                    //  1. 입고승인/ 2. stuffinsub 의 outwareyn = y 인 케이스.
                    //  3. 하위품 사라진 경우(DB삭제)
                    //  이것들을 가려내야 한다.   (20_0330_허윤구)

                    //stuffinsub outware y or n
                    string[] CheckYN = new string[2];
                    string Query = "select outwareyn from stuffinsub where lotid = '" + strBarCode + "'";
                    CheckYN = DataStore.Instance.ExecuteQuery(Query, false);
                    string OutCheckYN = CheckYN[1];

                    string[] DeleteBarcodeYN = new string[2];
                    string sql_2 = "select cnt = COUNT(*) from wk_result where labelid = '" + strBarCode + "'";
                    DeleteBarcodeYN = DataStore.Instance.ExecuteQuery(sql_2, false);


                    if (OutCheckYN == "Y")
                    {
                        Message[0] = "[재고소진]";
                        Message[1] = "입력된 자재는 현재 재고량이 0이하입니다.\r\n" +
                                       "재고를 모두 소진하였습니다.";
                        throw new Exception();
                    }
                    else if (DeleteBarcodeYN[1] == "0")
                    {
                        Message[0] = "[하위품 소실]";
                        Message[1] = "해당 하위품( " + strBarCode + " )은 승인되지 않은 품목이거나 삭제처리된 Lot입니다.";
                        throw new Exception();
                    }
                    else
                    {
                        Message[0] = "[입고승인]";
                        Message[1] = "해당 품목은 승인되지 않은 품목이거나 입고내역이 없는 품목이므로 사용할 수 없습니다.";
                        throw new Exception();
                    }
                }          
            }
            catch (Exception)
            {
                m_ArticleID = "";
                m_LabelGubun = "";
                m_Inspector = "";
                m_RemainQty = 0;
                m_LocRemainQty = 0;
                WH_AR_m_LocID = "";
                m_UnitClss = "";
                m_EffectDate = "";
                txtBarCodePreScan.Text = "";
                WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                return false;
            }
        }

        #endregion

        private bool Find_BOM_Child()
        {
            try
            {
                // BOM 풀지말고 작지의 CHILD_ARTICLE_ID 를 가져와서 비교하는 방식으로 해.
                // PL_INPUTDET_CHILDARTICLE
                // 2020.01.07 허윤구
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("sInstID", Wh_Ar_InstID);
                sqlParameter.Add("sInstSeq", Wh_Ar_InstID_Seq);

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_PlanInput_sPlanInputDetArticle", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Wh_Ar_ChildArticleID = dt.Rows[0]["CHildArticleID"].ToString();
                }

                if (Wh_Ar_ChildArticleID == string.Empty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류]", 0, 1);
                return false;
            }                       
        }
        #endregion


        #region 시작, 취소 버튼선택.

        // 시작처리 버튼 선택시.
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_ProcessID == "6501" || m_ProcessID == "6601") //2021-10-06 외주 공정일땐 임시 인서트 않하게 하기 위해 조건 추가
            {
                Frm_tprc_Main.lstStartLabel = new List<string>();
                Frm_tprc_Main.lstStartLabel.Add(txtBarCodePreScan.Text.Trim());
                DialogResult = DialogResult.OK;
                btnCancel_Click(null, null);

            }
            else
            {
                // DB 임시 인서트.
                if (StartHandleWorking_Click() == true)
                {
                    DialogResult = DialogResult.OK;
                    btnCancel_Click(null, null);
                }
                else
                {
                    Message[0] = "[오류]";
                    Message[1] = string.Format("오류! 관리자에게 문의\r\n");
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                }
            }
        }

        // 취소 버튼 선택시.
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        #region DB 임시인서트 펑션

        // DB 임시데이터 인서트 작업.
        private bool StartHandleWorking_Click()
        {
            List<WizCommon.Procedure> Prolist = new List<WizCommon.Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                Dictionary<string, object> sqlParameter1 = new Dictionary<string, object>();
                double WorkQty = 0;
                string YLabelID = "";

                sqlParameter1.Add("JobID", 0);
                sqlParameter1.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                sqlParameter1.Add("InstDetSeq", Frm_tprc_Main.g_tBase.sInstDetSeq);
                sqlParameter1.Add("LabelID", Frm_tprc_Main.g_tBase.sLotID);
                sqlParameter1.Add("StartSaveLabelID", txtBarCodePreScan.Text.Trim());

                sqlParameter1.Add("LabelGubun", m_LabelGubun);
                sqlParameter1.Add("ProcessID", m_ProcessID);
                sqlParameter1.Add("MachineID", m_MachineID);
                // 새벽 작업자 scan date 일자 -1 작업(새벽반의 물량도 전날물량으로 쳐야 하니까.)
                // 2019.09.05 허윤구.
                string Midnight_Worker = DateTime.Now.ToString("HHmmss");
                if ((Convert.ToInt32(Midnight_Worker) >= 000000) && (Convert.ToInt32(Midnight_Worker) <= 073000))
                {
                    sqlParameter1.Add("ScanDate", DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
                }
                else
                {
                    sqlParameter1.Add("ScanDate", DateTime.Now.ToString("yyyyMMdd"));
                }
                sqlParameter1.Add("ScanTime", DateTime.Now.ToString("HHmmss"));

                sqlParameter1.Add("ArticleID", Frm_tprc_Main.g_tBase.sArticleID);
                sqlParameter1.Add("WorkQty", WorkQty);
                sqlParameter1.Add("Comments", m_ProcessID + " " + "공정 시작처리에 의한 기록작업");
                sqlParameter1.Add("ReworkOldYN", "");
                sqlParameter1.Add("ReworkLinkProdID", "");

                sqlParameter1.Add("WorkStartDate", DateTime.Now.ToString("yyyyMMdd"));
                sqlParameter1.Add("WorkStartTime", DateTime.Now.ToString("HHmmss"));
                sqlParameter1.Add("WorkEndDate", "");
                sqlParameter1.Add("WorkEndTime", "");
                sqlParameter1.Add("JobGbn", "1");

                sqlParameter1.Add("NoReworkCode", "");
                sqlParameter1.Add("WDNO", "");
                sqlParameter1.Add("WDID", "");
                sqlParameter1.Add("WDQty", 0);
                sqlParameter1.Add("LogID", 0);

                sqlParameter1.Add("s4MID", "");
                sqlParameter1.Add("DayOrNightID", Frm_tprc_Main.g_tBase.DayOrNightID);
                sqlParameter1.Add("CreateUserID", Frm_tprc_Main.g_tBase.PersonID);
                

                WizCommon.Procedure pro1 = new WizCommon.Procedure();
                pro1.Name = "xp_wkResult_iWkResult";
                pro1.OutputUseYN = "Y";
                pro1.OutputName = "JobID";
                pro1.OutputLength = "20";

                Prolist.Add(pro1);
                ListParameter.Add(sqlParameter1);


                if (InsertX > 0)
                {
                    for (int k = 0; k < InsertX; k++)
                    {
                        Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();
                        sqlParameter2.Add("JobID", "");
                        sqlParameter2.Add("ChildLabelID", "");//
                        sqlParameter2.Add("ChildLabelGubun", "");
                        sqlParameter2.Add("ChildArticleID", "");
                        sqlParameter2.Add("ReworkOldYN", "");
                        sqlParameter2.Add("ReworkLinkChildProdID", "");
                        sqlParameter2.Add("CreateUserID", Frm_tprc_Main.g_tBase.PersonID);
                        sqlParameter2.Add("ProdQtyOver100YN", "");

                        WizCommon.Procedure pro2 = new WizCommon.Procedure();
                        pro2.Name = "xp_wkResult_iWkResultArticleChild";
                        pro2.OutputUseYN = "N";
                        pro2.OutputName = "JobID";
                        pro2.OutputLength = "20";

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter2);
                    }
                }

                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputToCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {
                    list_Result.RemoveAt(0);
                    m_ArticleID = "";
                    m_LabelGubun = "";
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

        #endregion


        #endregion


    }
}
