using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Calc_Duy
{
    public partial class Form1 : Form
    {
        private List<string> history = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        //------------------ 2. Nhập được số nguyên dương ------------
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnNumber0_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "0";
        }

        private void btnNumber1_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "1";
        }

        private void btnNumber2_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "2";
        }

        private void btnNumber3_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "3";
        }

        private void btnNumber4_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "4";
        }

        private void btnNumber5_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "5";
        }

        private void btnNumber6_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "6";
        }

        private void btnNumber7_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "7";
        }

        private void btnNumber8_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "8";
        }

        private void btnNumber9_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "9";
        }

        // 3. Nhập số thập phân, định đạng đúng(vd: 1234567,89 -> 1.234.567,89)
        private string FormatNumberForDisplay(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string[] parts = input.Split(new char[] { '+', '-', '*', '/', '%' }, StringSplitOptions.RemoveEmptyEntries);
            char[] operators = input.Where(c => "+-*/%".Contains(c)).ToArray();

            if (parts.Length == 0)
            {
                return input;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                string cleanInput = parts[i].Replace(".", "").Replace(",", "");

                if (double.TryParse(cleanInput, out double number))
                {
                    var customFormat = new NumberFormatInfo
                    {
                        NumberDecimalSeparator = ",",
                        NumberGroupSeparator = ".",
                        NumberGroupSizes = new[] { 3 }
                    };

                    if (number % 1 == 0)
                    {
                        parts[i] = number.ToString("#,##0", customFormat);
                    }
                    else
                    {
                        parts[i] = number.ToString("#,##0.##", customFormat);
                    }
                }
            }

            string result = parts[0];
            for (int i = 0; i < operators.Length; i++)
            {
                if (i + 1 < parts.Length)
                {
                    result += operators[i] + parts[i + 1];
                }
                else
                {
                    result += operators[i];
                }
            }

            return result;
        }
        private void TxtMainDisplay_TextChanged(object sender, EventArgs e)
        {
            int cursorPosition = TxtMainDisplay.SelectionStart;

            TxtMainDisplay.Text = FormatNumberForDisplay(TxtMainDisplay.Text);

            TxtMainDisplay.SelectionStart = cursorPosition;
        }

        // 4. Thực hiện chức năng nút C
        private void btnClear_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text = "";
            TxtSubDisplay.Text = "";
        }

        // 5. Xóa lùi biểu thức (xóa từng ký tự từ phải sang trái)
        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (TxtMainDisplay.Text.Length > 0)
            {
                TxtMainDisplay.Text = TxtMainDisplay.Text.Substring(0, TxtMainDisplay.Text.Length - 1);
            }
        }

        // 6. Thực hiện tính toán đúng : +,-,*,/, %
        private void btnPlus_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "+";
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "-";
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "*";
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "/";
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += "%";
        }

        private void btnDot_Click(object sender, EventArgs e)
        {
            TxtMainDisplay.Text += ",";
        }

        // 7. Thực hiện đúng khi nhấn nhiều lần nút dấu bằng
        private void btnEqual_Click(object sender, EventArgs e)
        {
            try
            {
                var result = new DataTable().Compute(TxtMainDisplay.Text, null);
                string resultString = result.ToString();
                TxtSubDisplay.Text = resultString;

                string calculation = $"{TxtMainDisplay.Text} = {resultString}";
                history.Add(calculation);

                TxtMainDisplay.Text = resultString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        // 8. Nhận số âm khi nhấn nút ± (có dấu - đằng trước)
        private void btnPlusMinus_Click(object sender, EventArgs e)
        {
            string currentText = TxtMainDisplay.Text;

            if (double.TryParse(currentText, out double number))
            {
                number = -number;
                TxtMainDisplay.Text = number.ToString(CultureInfo.InvariantCulture);
            }
        }


        // 9. Thực hiện được chức năng nút mở đóng ngoặc()
        private bool isOpenBracket = true;
        private void btnParenthesis_Click(object sender, EventArgs e)
        {
            if (isOpenBracket)
            {
                TxtMainDisplay.Text += "(";
            }
            else
            {
                TxtMainDisplay.Text += ")";
            }
            isOpenBracket = !isOpenBracket;
        }

        
        // 10. Lưu nhật ký tính toán(biểu tượng đồng hồ trên giao diện)
        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (rtbHistory != null)
            {
                rtbHistory.Clear();

                foreach (var item in history)
                {
                    string[] parts = item.Split('=');
                    if (parts.Length == 2)
                    {
                        string expression = parts[0].Trim();
                        string result = parts[1].Trim();

                        rtbHistory.SelectionAlignment = HorizontalAlignment.Right;
                        rtbHistory.AppendText(expression + Environment.NewLine);

                        rtbHistory.SelectionAlignment = HorizontalAlignment.Right;
                        rtbHistory.AppendText("= " + result + Environment.NewLine);

                        rtbHistory.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        rtbHistory.SelectionAlignment = HorizontalAlignment.Right;
                        rtbHistory.AppendText(item + Environment.NewLine);

                        rtbHistory.AppendText(Environment.NewLine);
                    }
                }

                rtbHistory.ScrollToCaret();
                rtbHistory.Visible = !rtbHistory.Visible;
            }
            else
            {
                MessageBox.Show("Không tìm thấy lịch sử!");
            }
        }

    }
}
