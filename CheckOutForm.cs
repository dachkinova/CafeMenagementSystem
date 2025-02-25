﻿using CafeManagementSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace HotelMenagementSystem
{
    [Serializable]
    public partial class CheckOutForm : Form
    {
        public CheckOutForm()
        {
            InitializeComponent();
        }

        public List<Product> productsList1;



        public string totalBill1 { get; set; }

        public CheckOutForm(string totalBill, List<Product> productsList, bool isPayed)
        {
            InitializeComponent();

            this.totalBill1 = totalBill;
            totalBillBox.Text = totalBill1;
            this.productsList1 = productsList;


            label6.Text = DateTime.Now.ToLongDateString();
            label5.Text = DateTime.Now.ToLongTimeString();


            GetDublicatedItems();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Would you like to continue?", "Message", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Form1 f = new Form1(null);


                var binFormatter = new BinaryFormatter();
                using (var fileStream = new FileStream(@"D:\productsList.txt",
                   FileMode.Open, FileAccess.Read))
                {
                    productsList1 = (List<Product>)binFormatter.Deserialize(fileStream);
                    foreach (var pr in productsList1)
                    {
                        f.listView1.Items.Add(pr.ToString());
                    }
                }

                int count = 0;
                count++;

                if (count > 1)
                {
                    using (var fileStream = new FileStream(@"D:\productsList.txt",
                    FileMode.Open, FileAccess.Read))
                    {
                        productsList1 = (List<Product>)binFormatter.Deserialize(fileStream);
                        foreach (var pr in productsList1)
                        {
                            f.listView1.Items.Add(pr.ToString());
                        }
                    }
                }
                this.Hide();
                f.ShowDialog();
                this.Close();
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
        }
        private void buttonRemoveProduct_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView2.Items)
            {
                if (item.Selected)
                {
                    listView2.Items.Remove(item);

                    double price = double.Parse(item.SubItems[2].Text);
                    double newPrice = double.Parse(this.totalBillBox.Text) - price;
                    totalBillBox.Text = newPrice.ToString();
                    totalBill1 = newPrice.ToString();
                }
            }
        }
        private void buttonReset_Click(object sender, EventArgs e)
        {
            listView2.Clear();
            totalBillBox.Clear();
        }
       
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSelectedIndex = checkedListBox1.SelectedIndex;
            if (iSelectedIndex == -1)
                return;
            for (int iIndex = 0; iIndex < checkedListBox1.Items.Count; iIndex++)
                checkedListBox1.SetItemCheckState(iIndex, CheckState.Unchecked);
            checkedListBox1.SetItemCheckState(iSelectedIndex, CheckState.Checked);

        }
        private void buttonPay_Click(object sender, EventArgs e)
        {
            totalBill1 = totalBillBox.Text;

            if (checkedListBox1.GetItemCheckState(0) == CheckState.Checked)
            {
                PaymentMethod pm = new ByCash(totalBill1);
                pm.ShowMessage();
            }
            else if (checkedListBox1.GetItemCheckState(1) == CheckState.Checked)
            {
                PaymentMethod pm = new WithCard(totalBill1);
                pm.ShowMessage();
            }
            else
            {
                MessageBox.Show("Please pick a payment method and then click OK to pay.");
            }
        }
        public void ShowInformation(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (Product pr in productsList1)
            {
                builder.AppendLine(pr.ToString());
            }
            builder.AppendLine();

        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            printDocument.DefaultPageSettings.PaperSize =
                new PaperSize("", 390, 500);
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.PrintPreviewControl.Zoom = 1.0;
            printPreviewDialog.ShowDialog();
        }
        private void printDocument_PrintPage(object sender,
            System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("RECEIPT", new Font("Times New Roman", 20, FontStyle.Underline),
                new SolidBrush(Color.Blue), new Point(45, 45));

            e.Graphics.DrawString("Cafe management system", new Font("Times New Roman", 12, FontStyle.Italic),
                new SolidBrush(Color.Black), new Point(45, 100));


            e.Graphics.DrawString("Item", new Font("Times New Roman", 11, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(45, 130));
            e.Graphics.DrawString("Quantity", new Font("Times New Roman", 11, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(200, 130));
            e.Graphics.DrawString("Price", new Font("Times New Roman", 11, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(300, 130));

            int x = 150;

            for (int i = 0; i < listView2.Items.Count; i++)
            {
                string name = listView2.Items[i].Text;
                string qty = listView2.Items[i].SubItems[1].Text;
                string price = listView2.Items[i].SubItems[2].Text;


                e.Graphics.DrawString(name, new Font("Times New Roman", 12),
                new SolidBrush(Color.Black), new Point(45, x));
                e.Graphics.DrawString(qty, new Font("Times New Roman", 12),
                new SolidBrush(Color.Black), new Point(200, x));
                e.Graphics.DrawString(price, new Font("Times New Roman", 12),
                new SolidBrush(Color.Black), new Point(300, x));

                x += 17;
            }

            e.Graphics.DrawString("----------------------------",
                new Font("Times New Roman", 11, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(200, x + 20));
            e.Graphics.DrawString("Total bill: " + totalBill1, new Font("Times New Roman", 14, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(210, x + 40));

            var dateTime = DateTime.Now;
            e.Graphics.DrawString(dateTime.ToString(), new Font("Times New Roman", 11, FontStyle.Italic),
            new SolidBrush(Color.Black), new Point(45, x + 100));
        }

        private void printPreview_PrintClick(object sender, System.EventArgs ee)
        {
            try
            {
                this.printPreviewDialog.Document = printDocument;
                if (printPreviewDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, ToString());
            }
        }

        private void GetDublicatedItems()
        {
            var list1 = productsList1
                .GroupBy(x => x.Name)
                .Distinct()
                .Select(g => new
                {
                    Value = g.Key,
                    Count = g.Count(),
                    TotalPrice = string.Format("{0:f2}", g.Sum(s => s.Price * s.quantity))
                })
                .ToList();

            foreach (var x in list1)
            {
                ListViewItem lvi = new ListViewItem(x.Value.ToString());
                lvi.SubItems.Add(x.Count.ToString());
                lvi.SubItems.Add(x.TotalPrice.ToString());
                listView2.Items.Add(lvi);
            }
        }

        private void buttonSaveToFile_Click(object sender, EventArgs e)
        {
            string path = @"D:\SavedOrder-CafeManagemnetSystem\listOfOrderedProducts.txt";
            
            FileSaver.CreateFile(path, productsList1, totalBill1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Plase select currency.");
            }
            else if (comboBox1.SelectedItem.ToString() == "BGN")
            {
                labelSign.Text = "лв";
                double money = 0;

                foreach (Product pr in productsList1)
                {
                    money += pr.price;
                }
                Currency curr = new BGN();
                double convertedSum = curr.ConvertPrice(money);
                totalBillBox.Text = (string.Format("{0:f2}", convertedSum));
            }
            else if (comboBox1.SelectedItem.ToString() == "EUR")
            {
                labelSign.Text = "€";
                double money = 0;
                foreach (Product pr in productsList1)
                {
                    money += pr.price;
                }
                Currency curr = new EUR();
                double convertedSum = curr.ConvertPrice(money);
                totalBillBox.Text = (string.Format("{0:f2}", convertedSum));
            }
            else if (comboBox1.SelectedItem.ToString() == "USD")
            {
                labelSign.Text = "$";
                totalBillBox.Text = totalBill1;
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}




    
