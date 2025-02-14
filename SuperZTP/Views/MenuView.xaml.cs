﻿using SuperZTP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperZTP.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MenuViewModel;
            if (viewModel != null)
            {
                viewModel.GenerateSelectedReport();
            }
        }

        private void GenerateSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MenuViewModel;
            if (viewModel != null)
            {
                viewModel.GenerateSelectedSummary();
            }
        }
    }
}
