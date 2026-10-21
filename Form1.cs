using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Avtomoika
{
    public partial class Form1 : Form
    {
        // 1. Хранилище заказов в памяти приложения
        private List<Order> orderHistory = new List<Order>();

        // 2. Справочники тарифов и коэффициентов кузова
        private Dictionary<string, double> carTypes = new Dictionary<string, double>()
        {
            { "Седан", 1.0 },
            { "Внедорожник / Кроссовер", 1.4 },
            { "Микроавтобус", 1.8 }
        };

        private Dictionary<string, decimal> washServices = new Dictionary<string, decimal>()
        {
            { "Экспресс (Кузов)", 400m },
            { "Комплекс (Кузов + Салон)", 800m },
            { "Химчистка салона", 3000m },
            { "Покрытие воском", 500m }
        };

        public Form1()
        {
            InitializeComponent();
            InitializeWashApp();
        }

        private void InitializeWashApp()
        {
            // Привязка данных к выпадающим спискам
            cmbCarType.DataSource = new BindingSource(carTypes, null);
            cmbCarType.DisplayMember = "Key";
            cmbCarType.ValueMember = "Value";

            cmbService.DataSource = new BindingSource(washServices, null);
            cmbService.DisplayMember = "Key";
            cmbService.ValueMember = "Value";

            UpdateHistoryGrid();
        }

        // КНОПКА: Добавление и расчет заказа
        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCarNumber.Text))
            {
                MessageBox.Show("Введите госномер автомобиля!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string carNumber = txtCarNumber.Text.Trim().ToUpper();
            double coefficient = (double)cmbCarType.SelectedValue;
            decimal basePrice = (decimal)cmbService.SelectedValue;

            // Расчет стоимости: Базовая цена услуги * Коэффициент габаритов машины
            decimal finalPrice = basePrice * (decimal)coefficient;

            Order newOrder = new Order
            {
                Id = orderHistory.Count + 1,
                CarNumber = carNumber,
                CarType = cmbCarType.Text,
                Service = cmbService.Text,
                Price = finalPrice,
                DateTime = DateTime.Now
            };

            orderHistory.Add(newOrder);
            UpdateHistoryGrid();
            txtCarNumber.Clear();

            MessageBox.Show($"Заказ №{newOrder.Id} успешно добавлен!\nСтоимость: {finalPrice:F2} руб.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateHistoryGrid()
        {
            dgvOrders.DataSource = null;
            dgvOrders.DataSource = orderHistory.Select(o => new
            {
                № = o.Id,
                Госномер = o.CarNumber,
                Тип = o.CarType,
                Услуга = o.Service,
                Цена = o.Price.ToString("F2") + " руб.",
                Время = o.DateTime.ToString("HH:mm:ss")
            }).ToList();

            decimal totalRevenue = orderHistory.Sum(o => o.Price);
            lblTotalRevenue.Text = $"Общая выручка: {totalRevenue:F2} руб.";
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }
        public string Service { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
                // КНОПКА: Экспорт статистики в файл
        private void btnExportReport_Click(object sender, EventArgs e)
        {
            if (orderHistory.Count == 0)
            {
                MessageBox.Show("История заказов пуста. Нечего сохранять.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fileName = $"Отчет_Смена_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("=============================================");
                    writer.WriteLine($"ОТЧЕТ АВТОМОЙКИ ЗА СМЕНУ ОТ {DateTime.Now:dd.MM.yyyy HH:mm}");
                    writer.WriteLine("=============================================");
                    writer.WriteLine($"{"№",-4} | {"Госномер",-10} | {"Тип кузова",-15} | {"Тип услуги",-22} | {"Итого",-10}");
                    writer.WriteLine("-------------------------------------------------------------------------");

                    foreach (var order in orderHistory)
                    {
                        writer.WriteLine($"{order.Id,-4} | {order.CarNumber,-10} | {order.CarType,-15} | {order.Service,-22} | {order.Price + "р.",-10}");
                    }

                    writer.WriteLine("-------------------------------------------------------------------------");
                    writer.WriteLine($"Всего обработано автомобилей: {orderHistory.Count}");
                    writer.WriteLine($"Суммарная выручка за смену : {orderHistory.Sum(o => o.Price):F2} руб.");
                    writer.WriteLine("=============================================");
                }

                MessageBox.Show($"Отчет успешно сохранен:\n{Path.GetFullPath(fileName)}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
