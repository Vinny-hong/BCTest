﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KwBarcode;
using GenCode128;

namespace NuCloverBarcode
{
    public partial class NuClover : Form
    {
        private Bitmap mImage = null;
        private Image mBarcodeImage = null;
        private int
            mYear,
            mMonth,
            mDate,
            mCco,
            mTco,            
            mLb,
            mTb,
            mTw,
            mTh,
            mTi;
        private float
            mSp,
            mIc;
        private string mTn;

        private long
            lVer = 0,
            lEqu = 0,
            lYear,
            lMonth,
            lDate,
            lCco,
            lTco,
            lTn,
            lSp,
            lIc,
            lLb,
            lTw,
            lTh,
            lTi,
            lTb;

        private int[] bits = {
            2,
            2,
            4,
            4,
            5,
            5,
            5,
            24,
            32,
            32,
            8,
            6,
            7,
            7,
            8
        };

        public NuClover()
        {
            InitializeComponent();
            String theVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = "NuClover Barcode Generator (v." + theVersion + ")";

            TxTb.TextChanged += onTargetParamChanged;
            TxLb.TextChanged += onTargetParamChanged;
            TxTw.TextChanged += onTargetParamChanged;
            TxTh.TextChanged += onTargetParamChanged;
            TxTi.TextChanged += onTargetParamChanged;

            generateBarcodeImage(this, new EventArgs());

        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mImage = new Bitmap(openFileDialog1.FileName);
                pictureBox2.Image = drawTargetZoneOnImage(mImage);

            }
        }

        private Bitmap drawTargetZoneOnImage(Bitmap image)
        {            
            if (image == null)
                return null;

            Bitmap retImage = (Bitmap)image.Clone();
            if (gatherTargetDate())
            {
                Rectangle[] rect = getTargetRect();
                Graphics g1 = Graphics.FromImage(retImage);
                Pen p = new Pen(Color.Red);
                p.Width = 3;
                g1.DrawRectangles(p, rect);
            }

            return retImage;
        }

        private bool gatherAllDate()
        {
            if (!gatherInfoDate())
                return false;
            if (!gatherTargetDate())
                return false;
            return true;
        }

        private bool gatherInfoDate()
        {
            try
            {
                DateTime dateTime = dateTimePicker1.Value;
                mYear = dateTime.Year - 2012;
                mMonth = dateTime.Month;
                mDate = dateTime.Day;
                mCco = int.Parse(TxCco.Text);
                mTco = int.Parse(TxTco.Text);
                mTn = TxTn.Text;
                mSp = float.Parse(TxSp.Text);
                mIc = float.Parse(TxIc.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private bool gatherTargetDate()
        {
            try
            {
                mLb = int.Parse(TxLb.Text);
                mTb = int.Parse(TxTb.Text);
                mTw = int.Parse(TxTw.Text);
                mTh = int.Parse(TxTh.Text);
                mTi = int.Parse(TxTi.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private Rectangle[] getTargetRect()
        {
            Rectangle[] rect = new Rectangle[2];
            rect[0] = new Rectangle(mLb, mTb, mTw, mTh);
            rect[1] = new Rectangle(mLb, mTb + mTi, mTw, mTh);
            return rect;
        }

        private void onTargetParamChanged(object sender, EventArgs e)
        {
            pictureBox2.Image = drawTargetZoneOnImage(mImage);
        }

        private void generateBarcodeImage(object sender, EventArgs e)
        {
            if (gatherAllDate())
            {
                convertAllDataToLong();
                long[] values = {
                    lVer,
                    lEqu,
                    lYear,
                    lMonth,
                    lDate,
                    lCco,
                    lTco,
                    lTn,
                    lSp,
                    lIc,
                    lLb,
                    lTw,
                    lTh,
                    lTi,
                    lTb
                };
                List<string> barcodes = BarcodeCore.BarcodeEncoder(bits, values);
                string BC = "";
                if (barcodes != null)
                {
                    foreach (string barcode in barcodes)
                    {
                        BC += barcode;
                    }
                    mBarcodeImage = Code128Rendering.MakeBarcodeImage(BC, 5, true);
                }
                else
                {
                    mBarcodeImage = null;
                }
            }
            else
            {
                mBarcodeImage = null;
            }
            pictureBox1.Image = mBarcodeImage;
        }

        private void convertAllDataToLong()
        {
            lYear = (long)mYear;
            lMonth = (long)mMonth;
            lDate = (long)mDate;
            lCco = (long)mCco;
            lTco = (long)mTco;
            lTn = BarcodeCore.TextToLong(mTn);
            lSp = BarcodeCore.FloatToLong(mSp);
            lIc = BarcodeCore.FloatToLong(mIc);
            lLb = (long)mLb;
            lTw = (long)mTw;
            lTh = (long)mTh;
            lTi = (long)mTi;
            lTb = (long)mTb;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mBarcodeImage != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mBarcodeImage.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                    MessageBox.Show("Complete", "Message");
                }
            }
        }
    }
}