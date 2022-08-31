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
    public partial class frm_PopUp_PreScanWork_Grid4 : Form
    {

        private string m_ProcessID = "";        //공정id
        private string m_MachineID = "";        //머신id
        private string m_ArticleID = "";        //품명id   

        private string m_LabelGubun = "";       //라벨구분  
        private string m_Inspector = "";        //검사자
        private string m_LabelID = "";           //몰드. 금형ID

        private string m_UnitClss = "";         // 단위
        private string m_UnitClssName = "";
        private string m_EffectDate = "";       // 유효일? 효과일?    (스캔 후 초기화)     
        private string m_MtrExceptYN = "";      // 예외처리 체크용도

        private string m_ChildArticleID = "";   //2021-05-12
        private string m_ChildArticle = "";     //2021-05-12
        private string m_ChildUnitClss = "";    //2021-05-12
        private string m_Article = "";          //2021-05-12
        private string m_StartSaveLabelID = ""; //2021-05-12

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

        private string m_NewProductYN = ""; //2022-02-14 신제품, 재연마 구분

        int SumQty = 0; //2021-11-16 합계용 변수 추가

        string[] Message = new string[2];  // 메시지박스 처리용도.
        bool YLabelOK = false;          //바코드 스캔 시 Y라벨로 넣을 것인지에 대한 BOOL값
        int InsertX = 0;                // 바코드 스캔 시 스캔해야 하는 하위품의 갯수 체크용. (X개가 들어갔는지)

        string WhAr_MoveArticle_I = string.Empty;    // I_FMB_재단 자동이동시의 Article.(Whole-Area)
        List<WizCommon.Procedure> Prolist = null;
        List<Dictionary<string, object>> ListParameter = null;

        WizWorkLib Lib = new WizWorkLib();

        public List<string> lstLabelList = new List<string>();
        public List<float> lstQty = new List<float>();
        public double AnotherQty = 0;
        private string m_PCMtrExceptYN = "";      // 2022-06-08 공정별 예외처리 체크용도

        public frm_PopUp_PreScanWork_Grid4()
        {
            InitializeComponent();
            SetScreen();  //TLP 사이즈 조정
        }
        public frm_PopUp_PreScanWork_Grid4(string strProcessID, string strMachineID, string strLabelID, List<string> lstLabelList, List<float> lstQty)
        {
            this.lstLabelList = lstLabelList;
            this.lstQty = lstQty;

            InitializeComponent();
            m_ProcessID = strProcessID;
            m_MachineID = strMachineID;
            m_LabelID = strLabelID;
            SetScreen();  //TLP 사이즈 조정
        }

        public frm_PopUp_PreScanWork_Grid4(string strProcessID, string strMachineID, string strLabelID) //2021-05-12
        {

            InitializeComponent();
            m_ProcessID = strProcessID;
            m_MachineID = strMachineID;
            m_LabelID = strLabelID;
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
            //tlpBottomChoice.ColumnStyles[1].Width = 0;
            //tlpBottomChoice.ColumnStyles[2].Width = 0;

            //2022-06-08 mtrExceptYN 빈값인지 아닌지 판단
            //ProcessMtrExceptYN();

            // 시작작업 처리
            Form_Activate();

            //칼럼 만들어지면 수정 예정 2022-06-09
            //(m_MtrExceptYN.Equals("N") && m_PCMtrExceptYN.Equals("")) || m_PCMtrExceptYN.Equals("N")
            if (m_MtrExceptYN.Equals("N"))
            {
                // 선 스캔 항목 정보 등록
                setPreScanLabel();
                setBtnOKEnabled(); //2021-12-11 라벨 없을 경우 ok 비활성화
            }

            
        }

        #region 선 스캔 항목 정보 등록

        private void setPreScanLabel()
        {
            try
            {
                //2021-05-12
                int index = 1;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PLotID", Frm_tprc_Main.g_tBase.sLotID);

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_PlanInput_sPlanInputDetArticle_Child", sqlParameter, false);

                foreach (DataRow dr in dt.Rows)
                {
                    m_ChildArticle = dr["BuyerArticleNo"].ToString();
                    m_ChildArticleID = dr["ChildArticleID"].ToString();
                    m_ChildUnitClss = dr["UnitClss"].ToString();


                    GridData2.Rows.Add(index.ToString() // 순번
                                               , ""
                                               , m_ChildArticle
                                               , ""
                                               , "선스캔라벨"
                                               , ""
                                               , m_ChildArticleID
                                               , m_ChildUnitClss
                                               , "" //2022-02-14
                                              );
                    index++;
                    //InsertX++; //2021-05-12 articlechild 에 임시 임서트 하기 위해 추가
                }

                #region 주석
                //if (this.lstLabelList.Count > 0)
                //{
                //    for (int i = 0; i < lstLabelList.Count; i++)
                //    {
                //        GridData2.Rows.Add((GridData2.Rows.Count + 1).ToString()
                //                               , lstLabelList[i]
                //                               , txtBuyerArticleNo.Text
                //                               , lstQty[i].ToString()
                //                               , "X"
                //                               , ""
                //                              );
                //    }
                //}
                #endregion

            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", ex.Message), "[오류 - setPreScanLabel()]", 0, 1);
            }
        }

        #endregion


        #region 공정별로 하위품 투입 예외 관리가 가능하여 공정별로 다를 경우 처리하기 위해 생성
        private void ProcessMtrExceptYN()
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Add("InstID", Frm_tprc_Main.g_tBase.sInstID);
                sqlParameter.Add("InstDetSeq", ConvertInt(Frm_tprc_Main.g_tBase.sInstDetSeq));

                DataTable dt = DataStore.Instance.ProcedureToDataTable("xp_PlanInput_sPlanInputDetArticle_ChildMtrExceptYN", sqlParameter, false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    m_PCMtrExceptYN = dr["MtrExceptYN"].ToString();
                }
            }
            catch (Exception excpt)
            {
                WizCommon.Popup.MyMessageBox.ShowBox(string.Format("오류! 관리자에게 문의\r\n{0}", excpt.Message), "[오류]", 0, 1);
            }
        }
        #endregion


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
            //btnOK.Enabled = false;
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

                    //txtArticle.Text = Lib.CheckNull(dr["BuyerArticleNo"].ToString()); //2021-11-16 덕우테크 품명
                    //txtBuyerArticleNo.Text =  Lib.CheckNull(dr["Article"].ToString()); //2021-11-16 덕우테크 품번
                    txtArticle.Text = Lib.CheckNull(dr["pldArticle"].ToString()); 
                    txtBuyerArticleNo.Text = Lib.CheckNull(dr["pldBuyerArticle"].ToString());

                    //txtBuyerArticleNo.Text = Lib.CheckNull(dr["OrderArticleID"].ToString()); //2021-05-11

                    //칼럼 만들어지면 수정 예정 2022-06-09
                    //(m_MtrExceptYN.Equals("Y") && m_PCMtrExceptYN.Equals("")) || m_PCMtrExceptYN.Equals("Y")
                    if (m_MtrExceptYN.Equals("Y"))
                    {
                        setPreScanLabel(); //2021-05-07 Grid 생성 없이 바로 넘어가면 checkdata 에서 오류 생겨서 Y도 Grid생성 되게 추가
                        btnOK_Click(null, null);
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

                    #region 주석
                    //2021-11-16 덕우테크 I라벨이 B라벨로 생성 I라벨로 시작하는 라벨이 아닌 라벨은 막기
                    //if (txtBarCodePreScan.Text.ToUpper().Contains("I") != true)
                    //{
                    //    WizCommon.Popup.MyMessageBox.ShowBox("스캔한 라벨을 확인 해주세요.", "[오류 - 스캔 부분]", 0, 1);
                    //    return;
                    //}
                    #endregion

                    if (BarcodeEnter())
                    {
                        //2021-11-16 2번째 그리드 부터 추가하기 위해 조건 추가
                        for (int i = 0; i < GridData2.Rows.Count; i++)
                        {
                            if (m_ArticleID == GridData2.Rows[i].Cells["BuyerArticleNo"].Value.ToString())
                            {
                                GridData2.Rows[i].SetValues((i + 1).ToString()
                                           , txtBarCodePreScan.Text
                                           , m_Article
                                           , stringFormatN0(m_LocRemainQty)
                                           , "선스캔라벨"
                                           , ""
                                           , m_ArticleID
                                           , m_UnitClss);
                            }
                        }


                        #region 주석
                        //2021-11-16 2번째 그리드 부터 추가하기 위해 조건 추가
                        //int GrdRowCount = GridData2.Rows.Count;

                        //if(GrdRowCount == 1 && GridData2.Rows[0].Cells["Label"].Value.ToString() == "")
                        //{
                        //     if (m_ArticleID == GridData2.Rows[0].Cells["BuyerArticleNo"].Value.ToString())
                        //     {
                        //         GridData2.Rows[0].SetValues((0 + 1).ToString()
                        //                    , txtBarCodePreScan.Text
                        //                    , m_Article
                        //                    , stringFormatN0(m_LocRemainQty)
                        //                    , ""
                        //                    , ""
                        //                    , m_ArticleID
                        //                    , m_UnitClss);
                        //     }
                        // }
                        // else
                        // {
                        //     if (m_ArticleID == GridData2.Rows[0].Cells["BuyerArticleNo"].Value.ToString())
                        //     {
                        //         GridData2.Rows[GrdRowCount - 1].SetValues(GrdRowCount
                        //                         , txtBarCodePreScan.Text
                        //                         , m_Article
                        //                         , stringFormatN0(m_LocRemainQty)
                        //                         , ""
                        //                         , ""
                        //                         , m_ArticleID
                        //                         , m_UnitClss);
                        //     }
                        // }

                        // SumQty = 0;

                        // for (int i = 0; i < GridData2.Rows.Count ; i++)
                        // {                           
                        //     SumQty += Convert.ToInt32(GridData2.Rows[i].Cells["Qty"].Value.ToString().Replace(",",""));
                        // }
                        //stringFormatN0(SumQty);

                        //GridData2.Rows.Add("", "", "합계", stringFormatN0(SumQty), "", "", "", "");
                        //if(GridData2.Rows[i].Cells["Label"].Value.ToString() != "")
                        //{
                        //   GridData2.Rows.Add();
                        //}

                        //if(m_ArticleID == GridData2.Rows[i].Cells["BuyerArticleNo"].Value.ToString())
                        //{
                        //  GridData2.Rows[i].SetValues((i + 1).ToString()
                        //             , txtBarCodePreScan.Text
                        //             , m_Article
                        //             , m_LocRemainQty.ToString()
                        //             , ""
                        //             , ""
                        //             , m_ArticleID
                        //             , m_UnitClss); 
                        //}
                        #endregion


                    }
                    txtBarCodePreScan.Text = "";
                    txtBarCodePreScan.Focus();
                    setBtnOKEnabled(); //2021-12-11 스캔 안 했을 경우 ok버튼 비활성화
                }
            }
            catch (Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("관리자에게 문의해주세요.\r\n(Info : " + ex.Message.ToString() + ")", "[오류 - 스캔 부분]", 0, 1);
            }
        }

        #region 하위 그리드 취소 버튼 클릭 > 해당 하위품 취소
        private void GridData2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (GridData2.Columns[e.ColumnIndex].Name == "Cancel")
                {
                    if (GridData2.SelectedRows[0].Cells["Remark"].Value.ToString().Trim().Replace(" ", "").Equals("선스캔라벨") || GridData2.SelectedRows[0].Cells["Article"].Value.ToString().Trim().Equals("합계"))
                    {
                        WizCommon.Popup.MyMessageBox.ShowBox("첫번째 라벨 또는 합계는 삭제가 불가 합니다.", "[투입 취소 불가]", 0, 1);
                        return;
                    }

                    if (WizCommon.Popup.MyMessageBox.ShowBox("해당 라벨을 투입 취소하시겠습니까?", "[취소 전 확인]", 0, 0) == DialogResult.OK)
                    {
                        GridData2.Rows.Remove(GridData2.SelectedRows[0]);

                        SumQty = 0;
                        for (int i = 0; i < GridData2.Rows.Count; i++)
                        {
                            if (GridData2.Rows[i].Cells["Article"].Value.ToString().Trim().Equals("합계"))
                            {
                                GridData2.Rows.Remove(GridData2.Rows[i]);
                            }
                        }
                        for (int i = 0; i < GridData2.Rows.Count; i++)
                        {
                            SumQty += Convert.ToInt32(GridData2.Rows[i].Cells["Qty"].Value.ToString());
                        }
                        stringFormatN0(SumQty);

                        GridData2.Rows.Add("", "", "합계", SumQty.ToString(), "", "", "", "");


                    }
                }
            }
            catch(Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("관리자에게 문의해주세요.\r\n(Info : " + ex.Message.ToString() + ")", "[오류 - 하위품 취소 부분]", 0, 1);
            }
        }
        #endregion

        #region btnOK 버튼 활성화 / 비활성화

        private void setBtnOKEnabled()
        {
            if (GridData2.Rows[0].Cells["Label"].Value.ToString() == "")
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }

            //if (GridData2.Rows.Count > 1)
            //{
            //    btnOK.Enabled = true;
            //}
            //else
            //{
            //    btnOK.Enabled = false;
            //}
        }

        #endregion

        #region 바코드 스캔 후 메인 그리드에 넣기

        private void FillGridData2(string strInstID, string strProcessID)
        {
            double dulReqQty = 0;
            //SetGridData2RowClear();

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

        private void SetGridData2RowClear()
        {
            while (GridData2.Rows.Count > 0)
            {
                GridData2.Rows.RemoveAt(0);
            }
        }

        #endregion

        // 스캔버튼 클릭.
        private void cmdBarCodePreScan_Click(object sender, EventArgs e)
        {
            try
            {
                POPUP.Frm_CMKeypad.g_Name = "바코드 스캔";
                POPUP.Frm_CMKeypad FK = new POPUP.Frm_CMKeypad();
                POPUP.Frm_CMKeypad.KeypadStr = txtBarCodePreScan.Text.Trim();
                if (FK.ShowDialog() == DialogResult.OK)
                {
                    txtBarCodePreScan.Text = FK.tbInputText.Text;

                    KeyPressEventArgs key = new KeyPressEventArgs((char)13);
                    txtBarCodePreScan_KeyPress(null, key);

                    //if (BarcodeEnter())
                    //{
                       
                    //    //btnOK_Click(null, null);
                    //    FillGridData2(Frm_tprc_Main.g_tBase.sInstID, m_ProcessID);
                    //}
                    //txtBarCodePreScan.Focus();
                }
                else
                {
                    txtBarCodePreScan.Text = string.Empty;
                }
            }
            catch(Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("관리자에게 문의해주세요.\r\n(Info : " + ex.Message.ToString() + ")", "[오류 - 스캔버튼 클릭 부분]", 0, 1);
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

                // 1.5 바코드 중복 체크
                bool flag = true;
                for(int i = 0; i < GridData2.Rows.Count; i++)
                {
                    string Label = GridData2.Rows[i].Cells["Label"].Value.ToString();

                    if (txtBarCodePreScan.Text.Trim().Equals(Label))
                    {
                        Message[0] = "[바코드 중복 오류]";
                        Message[1] = "해당 라벨은 이미 스캔하셨습니다.";
                        //WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 2, 2);
                        WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 3, 1);
                        WH_AR_m_LocID = "";
                        return false;
                    }
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

                    m_Article = dr["Article"].ToString().Trim();
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
                        string[] sProTerm = Frm_tprc_Main.gs.GetValue("Work", "ProTerm", "ProTerm").Split('|');//배열에 공정별 조건 넣기
                        foreach (string ProTerm in sProTerm)
                        {
                            if (ProTerm.Length > 4)
                            {
                                if (m_ProcessID == ProTerm.Substring(0, 4))
                                {
                                    if (ProTerm.Substring(4, 1) == "A")//숙성시간
                                    {
                                        //숙성시간YN AgingYN = Y면 숙성시간 24시간 지났으므로 사용가능
                                        if (dr["AgingYN"].ToString() != "Y")
                                        {
                                            Message[0] = "[숙성시간 24시간미만 재료 사용불가]";
                                            Message[1] = "재단 입고 이후 24시간 경과 되지 않은 " + Lib.CheckNull(dr["AgingTime"].ToString()) + "시간 경과된 )  재료 사용 불가능 합니다." +
                                                "숙성시작시간은 " + Lib.CheckNull(dr["AgingStartTime"].ToString()) + " 입니다.";
                                            throw new Exception();
                                        }
                                    }
                                    else if (ProTerm.Substring(4, 1) == "D")//배치검사
                                    {
                                        if (strBarCode.Substring(0, 1) != "I")
                                        {
                                            //배치검사YN DefectYN == N일때 배치검사 통과
                                            if (dr["DefectYN"].ToString() == "")//.Trim() == "")//값이 없으므로, 수행되지 않았음.
                                            {
                                                Message[0] = "[배치검사 오류]";
                                                Message[1] = "배치검사가 수행되지 않았습니다. \r\n  실험실에서 배치검사를 진행하여주십시오.";
                                                throw new Exception();
                                            }
                                            else if (dr["DefectYN"].ToString().ToUpper() == "Y")
                                            {
                                                Message[0] = "[배치검사 오류]";
                                                Message[1] = "배치검사를 통과하지 못했습니다. \r\n  해당 품목은 사용할 수 없습니다.";
                                                throw new Exception();
                                            }
                                        }                                        
                                    }
                                    else if (ProTerm.Substring(4, 1) == "E")//유효기간
                                    {
                                        if (dr["ChkEffect"].ToString() == "Y")//유효기간 체크여부YN
                                        {
                                            if (dr["EffectYN"].ToString() != "Y")//유효기간 EffectYN = Y일때 사용가능
                                            {
                                                if (dr["EffectDate"].ToString().Trim() == "")
                                                {
                                                    Message[0] = "[유효기간 없음]";
                                                    Message[1] = "유효기간이 입력되어있지 않습니다. \r\n 해당 품목은 사용할 수 없습니다. \r\n " +
                                                        "자재입고 담당자에게 유효기간 입력을 요청하세요.";
                                                }
                                                else
                                                {
                                                    Message[0] = "[유효기간 오류]";
                                                    Message[1] = "유효기간이 지났습니다. 해당 품목은 사용할 수 없습니다.";
                                                }

                                                throw new Exception();
                                            }
                                        }
                                    }
                                }
                            }
                        }
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
                    //  1. 입고승인
                    //  2. stuffinsub 의 outwareyn = y 인 케이스.
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

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    Wh_Ar_ChildArticleID = dt.Rows[0]["CHildArticleID"].ToString();
                //}
                foreach(DataRow dr in dt.Rows)
                {
                    if(m_ArticleID == dr["CHildArticleID"].ToString())
                    {
                        Wh_Ar_ChildArticleID = dr["CHildArticleID"].ToString();
                    }
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
            try
            {
                if (StartHandleWorking_Click() == true)
                {
                    // GLS 는 가공업체이기 때문에, 무조건 하위품은 1개라고 가정
                    #region 아래의 구문은 쓰지 않음(위의 이유로)
                    //Frm_tprc_Main.lstStartLabel = new List<string>();
                    //for (int i = 0; i < dgdMain.Rows.Count; i++)
                    //{
                    //    Frm_tprc_Main.lstStartLabel.Add(dgdMain.Rows[i].Cells["Label"].Value.ToString());
                    //}
                    #endregion

                    DialogResult = DialogResult.OK;
                    btnCancel_Click(null, null);
                }
                else
                {
                    Message[0] = "[오류]";
                    Message[1] = string.Format("오류! 관리자에게 문의\r\n");
                    WizCommon.Popup.MyMessageBox.ShowBox(Message[1], Message[0], 0, 1);
                }

                lstLabelList.Clear();
                lstQty.Clear();
                for (int i = 1; i < GridData2.Rows.Count; i++)
                {
                    double Qty = ConvertDouble(GridData2.Rows[i].Cells["Qty"].Value.ToString());
                    lstQty.Add((float)Qty);
                    lstLabelList.Add(GridData2.Rows[i].Cells["Label"].Value.ToString());

                    AnotherQty += Qty;
                }

                // 담은 라벨 리스트 넘기기
                DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                WizCommon.Popup.MyMessageBox.ShowBox("관리자에게 문의해주세요.\r\n(Info : " + ex.Message.ToString() + ")", "[오류 - btnOK_Click()]", 0, 1);
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
                sqlParameter1.Add("StartSaveLabelID", GridData2.Rows[0].Cells["Label"].Value.ToString());

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


                for (int k = 0; k < GridData2.Rows.Count; k++)
                {
                   Dictionary<string, object> sqlParameter2 = new Dictionary<string, object>();
                   sqlParameter2.Add("JobID", "");
                   sqlParameter2.Add("ChildLabelID", GridData2.Rows[k].Cells["Label"].Value.ToString());//
                   sqlParameter2.Add("ChildLabelGubun", "");
                   sqlParameter2.Add("ChildArticleID", GridData2.Rows[k].Cells["BuyerArticleNo"].Value.ToString());
                   sqlParameter2.Add("ReworkOldYN", "");
                   sqlParameter2.Add("ReworkLinkChildProdID", "");
                   sqlParameter2.Add("CreateUserID", Frm_tprc_Main.g_tBase.PersonID);
                   sqlParameter2.Add("ChildUseQty", 0);
                   //sqlParameter2.Add("ProdQtyOver100YN", ""); 

                   WizCommon.Procedure pro2 = new WizCommon.Procedure();
                   pro2.Name = "xp_wkResult_iWkResultArticleChild";
                   pro2.OutputUseYN = "N";
                   pro2.OutputName = "JobID";
                   pro2.OutputLength = "20";

                   Prolist.Add(pro2);
                   ListParameter.Add(sqlParameter2);
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

        #region 기타 메서드 모음

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // 천마리 콤마, 소수점 두자리
        private string stringFormatN2(object obj)
        {
            return string.Format("{0:N2}", obj);
        }

        // 천마리 콤마, 소수점 두자리
        private string stringFormatNC(object obj, int cnt)
        {
            return string.Format("{0:N" + cnt + "}", obj);
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

        // 소수로 변환
        private float ConvertFloat(string str)
        {
            float result = 0;
            float chkFloat = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (float.TryParse(str, out chkFloat) == true)
                {
                    result = float.Parse(str);
                }
            }

            return result;
        }

        #endregion
    }
}
