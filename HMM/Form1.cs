using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace HMM
{
    public partial class LoginForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        public ChromiumWebBrowser chromeBrowser;
        bool isHidden = true;
        private bool move = false;
        private Point startingPoint = new Point(0, 0);
        private int zoomLevel = 0;
        private string username(string username)
        {
            username = UsernameTB.Text;
            return username;
        }
        public LoginForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            PasswordTB.PasswordChar = '*';
            websitePanel.Visible = false;
            inventoryPanel.Visible = false;
            salesPanel.Visible = false;
            InitializeChrominium();
            inventoryDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            salesDatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //inventoryDataGrid.Dock = DockStyle.Fill;
            inventoryDataGrid.RowHeadersVisible = false;
            salesDatagrid.RowHeadersVisible = false;
            foreach (DataGridViewColumn column in inventoryDataGrid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn column in salesDatagrid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            try
            {
                LoadInventoryData();
            }
            catch (IndexOutOfRangeException)
            {

            };
            try
            {
                LoadSalesData();
            }
            catch (IndexOutOfRangeException)
            {

            };
        }

        private void LoadSalesData()
        {
            try
            {
                string salesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_SalesData.txt");
                foreach (string line in File.ReadAllLines(salesDataPath))
                {
                    string inventoryName = line.Split('&')[0].TrimStart();
                    string inventoryAmount = line.Split('&')[1].TrimStart();
                    string inventoryPrice = line.Split('&')[2].TrimStart();
                    string inventoryID = line.Split('&')[3].TrimStart();
                    string inventoryDate = line.Split('&')[4].TrimStart();
                    string inventoryEdit = line.Split('&')[5].TrimStart();
                    string inventoryDelete = line.Split('&')[6].TrimStart();


                    salesDatagrid.Rows.Add(new object[]{
                    inventoryName, inventoryAmount, inventoryPrice, inventoryID, inventoryDate, inventoryEdit, inventoryDelete, "Edit", "Delete"
                });


                }
            }
            catch (FileNotFoundException)
            {
                string profilesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_SalesData.txt");
                using (TextWriter Tw = new StreamWriter(profilesDataPath))
                {
                    for (int i = 0; i < salesDatagrid.Rows.Count; i++)
                    {
                        for (int j = 0; j < salesDatagrid.Columns.Count; j++)
                        {

                            Tw.Write($"{salesDatagrid.Rows[i].Cells[j].Value.ToString()}" + "& ");

                            if (j == salesDatagrid.Columns.Count - 1)
                            {

                            }
                        }
                        Tw.WriteLine();
                    }
                }
                LoadSalesData();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);       
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (UsernameTB.Text != "" || PasswordTB.Text != "")
            {
                if (UsernameTB.Text == "1" && PasswordTB.Text == "1")
                {
                    label3.Visible = false;
                    label4.Visible = false;
                    UsernameTB.Visible = false;
                    PasswordTB.Visible = false;
                    pictureBox1.Visible = false;
                    button4.Visible = false;
                    this.Height = 800;
                    this.FormBorderStyle = FormBorderStyle.None;
                    Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
                    websitePanel.Visible = true;
                    label2.Visible = false;
                    label2.Text = "HONEYSUCKLE-MEADERY";
                    label2.Location = new Point(448, 7);
                    label2.Visible = true;                    
                    websiteBtn.Visible = true;
                    inventoryBtn.Visible = true;
                    salesBtn.Visible = true;    
                }
                else
                {
                    MessageBox.Show("Invalid credentials, try again!");
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            if(isHidden == true)
            {
                PasswordTB.PasswordChar = '\0';
                isHidden = false;
            }
            else
            {
                PasswordTB.PasswordChar = '*';
                isHidden = true;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            startingPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startingPoint.X, p.Y - this.startingPoint.Y);

            }
        }

        private void InitializeChrominium()
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            this.chromeBrowser = new ChromiumWebBrowser("https://akl032.wixsite.com/my-site");
            websitePanel.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;            
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void inventoryBtn_Click(object sender, EventArgs e)
        {
            websitePanel.Visible = false;
            inventoryPanel.Visible = true;
            salesPanel.Visible = false;
        }

        private void websiteBtn_Click(object sender, EventArgs e)
        {
            inventoryPanel.Visible = false;
            websitePanel.Visible = true;
            salesPanel.Visible = false;
        }

        private void salesBtn_Click(object sender, EventArgs e)
        {
            inventoryPanel.Visible = false;
            websitePanel.Visible = false;
            salesPanel.Visible = true;
        }

        private void inventoryDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            this.inventoryDataGrid.ClearSelection();
        }

        private void inventoryNameTB_Enter(object sender, EventArgs e)
        {
            if(inventoryNameTB.Text == "Name")
            {
                inventoryNameTB.Text = "";
            }
        }

        private void inventoryNameTB_Leave(object sender, EventArgs e)
        {
            if (inventoryNameTB.Text == "")
            {
                inventoryNameTB.Text = "Name";
            }
        }

        private void inventoryDataGrid_DoubleClick(object sender, EventArgs e)
        {
            if(panel2.Visible == false)
            {
                invPnlAddBtn.Enabled = false;
                invPnlAddBtn.BackColor = Color.SlateGray;
                panel2.Visible = true;
            }
            else
            {
                invPnlAddBtn.Enabled = true;
                invPnlAddBtn.BackColor = Color.FromArgb(255, 128, 0);
                panel2.Visible = false;
            }
        }

        private void InventoryAmtTB_Enter(object sender, EventArgs e)
        {
            if (InventoryAmtTB.Text == "Amount")
            {
                InventoryAmtTB.Text = "";
            }
        }

        private void InventoryAmtTB_Leave(object sender, EventArgs e)
        {
            if (InventoryAmtTB.Text == "")
            {
                InventoryAmtTB.Text = "Amount";
            }
        }

        private void InventoryPriceTB_Enter(object sender, EventArgs e)
        {
            if (InventoryPriceTB.Text == "Price")
            {
                InventoryPriceTB.Text = "";
            }
        }

        private void InventoryPriceTB_Leave(object sender, EventArgs e)
        {
            if (InventoryPriceTB.Text == "")
            {
                InventoryPriceTB.Text = "Price";
            }
        }

        private void InventoryIDTB_Enter(object sender, EventArgs e)
        {
            if (InventoryIDTB.Text == "ID")
            {
                InventoryIDTB.Text = "";
            }
        }

        private void InventoryIDTB_Leave(object sender, EventArgs e)
        {
            if (InventoryIDTB.Text == "")
            {
                InventoryIDTB.Text = "ID";
            }
        }

        private void inventoryAddBtn_Click(object sender, EventArgs e)
        {
            string inventoryName = "N/A";
            string inventoryAmount = "N/A";
            string inventoryPrice = "0.00";
            string inventoryID = "N/A";
            DateTime iDate;
            iDate = InventoryDatePicker.Value;
            string inventoryDate = iDate.ToString();
            if (inventoryNameTB.Text != "Name")
            {
                inventoryName = inventoryNameTB.Text;
            }
            if (InventoryAmtTB.Text != "Amount")
            {
                inventoryAmount = InventoryAmtTB.Text;
            }
            if (InventoryPriceTB.Text != "Price")
            {
                inventoryPrice = InventoryPriceTB.Text;
            }
            if (InventoryIDTB.Text != "ID")
            {
                inventoryID = InventoryIDTB.Text;
            }
            this.inventoryDataGrid.Rows.Add(inventoryName, inventoryAmount, $"${inventoryPrice}", inventoryID, inventoryDate, "Edit", "Delete");
        }

        private void label5_Click(object sender, EventArgs e)
        {           
            invPnlAddBtn.BackColor = Color.FromArgb(255, 128, 0);
            invPnlAddBtn.Enabled = true;
            panel2.Visible = false;
        }

        private void invPnlAddBtn_Click(object sender, EventArgs e)
        {
            if(panel2.Visible == false)
            {
                invPnlAddBtn.Enabled = false;
                invPnlAddBtn.BackColor = Color.DarkGray;
                panel2.Visible = true;
            }
        }

        private void inventorySaveBtn_Click(object sender, EventArgs e)
        {
            SaveInventoryData();
        }
        private void SaveInventoryData()
        {
            try
            {
                string message = string.Empty;
                string inventoryDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_InventoryData.txt");
                foreach (DataGridViewRow row in inventoryDataGrid.Rows)
                {
                    message += row.Cells["itemName"].Value.ToString() + "& " + row.Cells["Amount"].Value.ToString() + "& " + row.Cells["Price"].Value.ToString() + "& " + row.Cells["ProductID"].Value.ToString() + "& " + row.Cells["Date"].Value.ToString() + "& " + row.Cells["inventoryEditColumn"].Value.ToString() + "& " + row.Cells["inventoryDeleteColumn"].Value.ToString() + "& ";
                    message += Environment.NewLine;
                }

                using (TextWriter tw = new StreamWriter(inventoryDataPath))
                {
                    tw.Write(message);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error rendering data!");
            }
        }
        private void SaveSalesData()
        {
            try
            {
                string message = string.Empty;
                string salesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_SalesData.txt");
                foreach (DataGridViewRow row in salesDatagrid.Rows)
                {
                    message += row.Cells["salesNameColumn"].Value.ToString() + "& " + row.Cells["salesAmountColumn"].Value.ToString() + "& " + row.Cells["salesPriceColumn"].Value.ToString() + "& " + row.Cells["salesIDColumn"].Value.ToString() + "& " + row.Cells["salesDateColumn"].Value.ToString() + "& " + row.Cells["salesEditColumn"].Value.ToString() + "& " + row.Cells["salesDeleteColumn"].Value.ToString() + "& ";
                    message += Environment.NewLine;
                }

                using (TextWriter tw = new StreamWriter(salesDataPath))
                {
                    tw.Write(message);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error rendering data!");
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                string inventoryDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_InventoryData.txt");
                foreach (string line in File.ReadAllLines(inventoryDataPath))
                {
                    string inventoryName = line.Split('&')[0].TrimStart();
                    string inventoryAmount = line.Split('&')[1].TrimStart();
                    string inventoryPrice = line.Split('&')[2].TrimStart();
                    string inventoryID = line.Split('&')[3].TrimStart();
                    string inventoryDate = line.Split('&')[4].TrimStart();
                    string inventoryEdit = line.Split('&')[5].TrimStart();
                    string inventoryDelete = line.Split('&')[6].TrimStart();


                    inventoryDataGrid.Rows.Add(new object[]{
                    inventoryName, inventoryAmount, inventoryPrice, inventoryID, inventoryDate, inventoryEdit, inventoryDelete, "Edit", "Delete"
                });


                }
            }
            catch (FileNotFoundException)
            {
                string profilesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Honeysuckle_Meadery_InventoryData.txt");
                using (TextWriter Tw = new StreamWriter(profilesDataPath))
                {
                    for (int i = 0; i < inventoryDataGrid.Rows.Count; i++)
                    {
                        for (int j = 0; j < inventoryDataGrid.Columns.Count; j++)
                        {

                            Tw.Write($"{inventoryDataGrid.Rows[i].Cells[j].Value.ToString()}" + "& ");

                            if (j == inventoryDataGrid.Columns.Count - 1)
                            {

                            }
                        }
                        Tw.WriteLine();
                    }
                }
                LoadInventoryData();
            }
        }

        private void inventoryDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = inventoryDataGrid.CurrentCell.RowIndex;
            if (inventoryDataGrid.Columns[e.ColumnIndex].Name == "inventoryDeleteColumn")
            {
                try
                {
                    inventoryDataGrid.Rows.RemoveAt(rowIndex);
                }
                catch (ArgumentOutOfRangeException)
                {

                }
                SaveInventoryData();
            }
        }

        private void zooBtn_Click(object sender, EventArgs e)
        {
            zoomLevel++;
            chromeBrowser.SetZoomLevel(zoomLevel);
        }

        private void zoomOutBtn_Click(object sender, EventArgs e)
        {
            zoomLevel--;
            chromeBrowser.SetZoomLevel(zoomLevel);
        }

        private void salesDatagrid_SelectionChanged(object sender, EventArgs e)
        {
            this.salesDatagrid.ClearSelection();
        }

        private void salesDatagrid_DoubleClick(object sender, EventArgs e)
        {
            if (salesItemForm.Visible == false)
            {
                salesAddBtn.Enabled = false;
                salesAddBtn.BackColor = Color.SlateGray;
                salesItemForm.Visible = true;
            }
            else
            {
                salesAddBtn.Enabled = true;
                salesAddBtn.BackColor = Color.FromArgb(255, 128, 0);
                salesItemForm.Visible = false;
            }
        }

        private void salesAddBtn_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                salesAddBtn.Enabled = false;
                salesAddBtn.BackColor = Color.DarkGray;
                salesItemForm.Visible = true;
            }
        }

        private void salesDatagrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = salesDatagrid.CurrentCell.RowIndex;
            if (salesDatagrid.Columns[e.ColumnIndex].Name == "salesDeleteColumn")
            {
                try
                {
                    salesDatagrid.Rows.RemoveAt(rowIndex);
                }
                catch (ArgumentOutOfRangeException)
                {

                }
                SaveSalesData();
            }
        }

        private void salesSaveBtn_Click(object sender, EventArgs e)
        {
            SaveSalesData();
        }

        private void salesInsertItemBtn_Click(object sender, EventArgs e)
        {
            string inventoryName = "N/A";
            string inventoryAmount = "N/A";
            string inventoryPrice = "0.00";
            string inventoryID = "N/A";
            DateTime iDate;
            iDate = InventoryDatePicker.Value;
            string inventoryDate = iDate.ToString();
            if (salesNameTB.Text != "Name")
            {
                inventoryName = salesNameTB.Text;
            }
            if (salesAmountTB.Text != "Amount")
            {
                inventoryAmount = salesAmountTB.Text;
            }
            if (salesPriceTB.Text != "Price")
            {
                inventoryPrice = salesPriceTB.Text;
            }
            if (salesIDTB.Text != "ID")
            {
                inventoryID = salesIDTB.Text;
            }
            this.salesDatagrid.Rows.Add(inventoryName, inventoryAmount, $"${inventoryPrice}", inventoryID, inventoryDate, "Edit", "Delete");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            salesAddBtn.Enabled = true;
            salesAddBtn.BackColor = Color.FromArgb(255, 128, 0);
            salesItemForm.Visible = false;
        }
    }
}
