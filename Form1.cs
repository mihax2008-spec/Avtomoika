using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace avtomoika
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.прайсЛист_УслугTableAdapter.Fill(this.автомойкаDataSet.ПрайсЛист_Услуг);
                this.машиныTableAdapter.Fill(this.автомойкаDataSet.Машины);
                this.клиентыTableAdapter.Fill(this.автомойкаDataSet.Клиенты);
                this.журнал_заказовTableAdapter.Fill(this.автомойкаDataSet.Журнал_заказов);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки базы: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    if (btn.Text.Trim().Equals("Сохранить", StringComparison.OrdinalIgnoreCase)) btn.Click += buttonSave_Click;
                    else if (btn.Text.Trim().Equals("Добавить", StringComparison.OrdinalIgnoreCase)) btn.Click += buttonAdd_Click;
                    else if (btn.Text.Trim().Equals("Удалить", StringComparison.OrdinalIgnoreCase)) btn.Click += buttonDelete_Click;
                    else if (btn.Text.Trim().Equals("Закрыть", StringComparison.OrdinalIgnoreCase)) btn.Click += buttonClose_Click;
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Form popupForm = new Form();
            popupForm.Size = new Size(350, 320);
            popupForm.StartPosition = FormStartPosition.CenterParent;
            popupForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            popupForm.MaximizeBox = false;
            popupForm.MinimizeBox = false;

            Button btnOk = new Button { Text = "ОК", DialogResult = DialogResult.OK, Location = new Point(50, 230), Size = new Size(100, 30) };
            Button btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Location = new Point(180, 230), Size = new Size(100, 30) };
            popupForm.Controls.Add(btnOk);
            popupForm.Controls.Add(btnCancel);
            popupForm.AcceptButton = btnOk;

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    popupForm.Text = "Добавить новый заказ";
                    Label lbl1 = new Label { Text = "Дата (ДД.ММ.ГГГГ ЧЧ:ММ):", Location = new Point(20, 20), AutoSize = true };
                    TextBox txtDate = new TextBox { Location = new Point(20, 40), Width = 280, Text = DateTime.Now.ToString("g") };
                    Label lbl2 = new Label { Text = "ID Машины:", Location = new Point(20, 70), AutoSize = true };
                    TextBox txtCarId = new TextBox { Location = new Point(20, 90), Width = 280 };
                    Label lbl3 = new Label { Text = "ID Услуги:", Location = new Point(20, 120), AutoSize = true };
                    TextBox txtServiceId = new TextBox { Location = new Point(20, 140), Width = 280 };
                    Label lbl4 = new Label { Text = "Имя сотрудника:", Location = new Point(20, 170), AutoSize = true };
                    TextBox txtEmpName = new TextBox { Location = new Point(20, 190), Width = 280 };
                    Label lbl5 = new Label { Text = "Финальная цена:", Location = new Point(20, 220), AutoSize = true };
                    TextBox txtPrice = new TextBox { Location = new Point(20, 240), Width = 280 };

                    btnOk.Location = new Point(50, 280);
                    btnCancel.Location = new Point(180, 280);
                    popupForm.Size = new Size(350, 370);
                    popupForm.Controls.AddRange(new Control[] { lbl1, txtDate, lbl2, txtCarId, lbl3, txtServiceId, lbl4, txtEmpName, lbl5, txtPrice });

                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var newRow = this.автомойкаDataSet.Журнал_заказов.NewЖурнал_заказовRow();
                            newRow.order_date = Convert.ToDateTime(txtDate.Text);
                            newRow.car_id = Convert.ToInt32(txtCarId.Text);
                            newRow.service_id = Convert.ToInt32(txtServiceId.Text);
                            newRow.employee_name = txtEmpName.Text;
                            newRow.final_price = Convert.ToDecimal(txtPrice.Text);

                            this.автомойкаDataSet.Журнал_заказов.AddЖурнал_заказовRow(newRow);
                            this.журнал_заказовTableAdapter.Update(this.автомойкаDataSet.Журнал_заказов);
                            MessageBox.Show("Заказ успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show($"Ошибка ввода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    break;

                case 1:
                    popupForm.Text = "Добавить клиента";
                    Label lblC1 = new Label { Text = "ФИО Клиента:", Location = new Point(20, 20), AutoSize = true };
                    TextBox txtClientName = new TextBox { Location = new Point(20, 40), Width = 280 };
                    Label lblC2 = new Label { Text = "Телефон:", Location = new Point(20, 80), AutoSize = true };
                    TextBox txtPhone = new TextBox { Location = new Point(20, 100), Width = 280 };
                    popupForm.Controls.AddRange(new Control[] { lblC1, txtClientName, lblC2, txtPhone });

                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var newRow = this.автомойкаDataSet.Клиенты.NewКлиентыRow();
                            newRow.client_name = txtClientName.Text;
                            newRow.Phone = txtPhone.Text;

                            this.автомойкаDataSet.Клиенты.AddКлиентыRow(newRow);
                            this.клиентыTableAdapter.Update(this.автомойкаDataSet.Клиенты);
                            MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show($"Ошибка ввода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    break;
                case 2:
                    popupForm.Text = "Добавить машину";
                    Label lblM1 = new Label { Text = "Гос. номер машины:", Location = new Point(20, 20), AutoSize = true };
                    TextBox txtPlate = new TextBox { Location = new Point(20, 40), Width = 280 };
                    Label lblM2 = new Label { Text = "Марка и модель:", Location = new Point(20, 80), AutoSize = true };
                    TextBox txtBrand = new TextBox { Location = new Point(20, 100), Width = 280 };
                    Label lblM3 = new Label { Text = "ID Владельца (Клиента):", Location = new Point(20, 140), AutoSize = true };
                    TextBox txtClientRef = new TextBox { Location = new Point(20, 160), Width = 280 };
                    popupForm.Controls.AddRange(new Control[] { lblM1, txtPlate, lblM2, txtBrand, lblM3, txtClientRef });

                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var newRow = this.автомойкаDataSet.Машины.NewМашиныRow();
                            newRow.plate_number = txtPlate.Text;
                            newRow.brand_model = txtBrand.Text;
                            newRow.client_id = Convert.ToInt32(txtClientRef.Text);

                            this.автомойкаDataSet.Машины.AddМашиныRow(newRow);
                            this.машиныTableAdapter.Update(this.автомойкаDataSet.Машины);
                            MessageBox.Show("Машина успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show($"Ошибка ввода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    break;

                case 3:
                    popupForm.Text = "Добавить услугу";
                    Label lblS1 = new Label { Text = "Название услуги:", Location = new Point(20, 20), AutoSize = true };
                    TextBox txtServiceName = new TextBox { Location = new Point(20, 40), Width = 280 };
                    Label lblS2 = new Label { Text = "Стоимость:", Location = new Point(20, 80), AutoSize = true };
                    TextBox txtServicePrice = new TextBox { Location = new Point(20, 100), Width = 280 };
                    popupForm.Controls.AddRange(new Control[] { lblS1, txtServiceName, lblS2, txtServicePrice });

                    if (popupForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var newRow = this.автомойкаDataSet.ПрайсЛист_Услуг.NewПрайсЛист_УслугRow();
                            newRow.service_name = txtServiceName.Text;
                            newRow.price = Convert.ToDecimal(txtServicePrice.Text);

                            this.автомойкаDataSet.ПрайсЛист_Услуг.AddПрайсЛист_УслугRow(newRow);
                            this.прайсЛист_УслугTableAdapter.Update(this.автомойкаDataSet.ПрайсЛист_Услуг);
                            MessageBox.Show("Услуга успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show($"Ошибка ввода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    break;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        this.журналЗаказовBindingSource.EndEdit();
                        this.журнал_заказовTableAdapter.Update(this.автомойкаDataSet.Журнал_заказов);
                        break;
                    case 1:
                        this.клиентыBindingSource.EndEdit();
                        this.клиентыTableAdapter.Update(this.автомойкаDataSet.Клиенты);
                        break;
                    case 2:
                        this.машиныBindingSource.EndEdit();
                        this.машиныTableAdapter.Update(this.автомойкаDataSet.Машины);
                        break;
                    case 3:
                        this.прайсЛистУслугBindingSource.EndEdit();
                        this.прайсЛист_УслугTableAdapter.Update(this.автомойкаDataSet.ПрайсЛист_Услуг);
                        break;
                }
                this.автомойкаDataSet.AcceptChanges();
                MessageBox.Show("Все изменения успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            BindingSource activeBindingSource = null;
            switch (tabControl1.SelectedIndex)
            {
                case 0: activeBindingSource = this.журналЗаказовBindingSource; break;
                case 1: activeBindingSource = this.клиентыBindingSource; break;
                case 2: activeBindingSource = this.машиныBindingSource; break;
                case 3: activeBindingSource = this.прайсЛистУслугBindingSource; break;
            }

            if (activeBindingSource?.Current != null)
            {
                if (MessageBox.Show("Удалить выбранную строку?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    activeBindingSource.RemoveCurrent();
                    buttonSave_Click(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Не выбрана строка для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabPage1_Click(object sender, EventArgs e) { }
    }
}
