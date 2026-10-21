using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

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
    }
}
