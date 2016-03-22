﻿using ATP_Lab.Model;
using ATP_Lab.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ATP_Lab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TreeViewModel treeViewModel;
        private ListView dragSource = null;
        private ListView parent = null;
        private Operation childData = null;
        private object firstObjectData = null;
        private object secondObjectData = null;


        public MainWindow()
        {
            InitializeComponent();
            treeViewModel = new TreeViewModel();
            this.DataContext = treeViewModel;
        }


        private void dList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView parent = (ListView)sender;
            dragSource = parent;
            this.parent = parent;
            object data = GetDataFromListView(dragSource, e.GetPosition(parent));
            secondObjectData = data;
            if (parent.Name == "TestList")
            {
                childData = (Operation)data;
            }
            firstObjectData = data;

        }

        private void dList_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            ListView parent = (ListView)sender;
            var data = GetDataFromListView(parent, e.GetPosition(parent));
            //main listview
            if (parent.Name == "TestList")
            {
                treeViewModel.Operation.Remove((Operation)firstObjectData);
                treeViewModel.Operation.Insert(GetIndex(data, treeViewModel.Operation), (Operation)firstObjectData);
            }
            //expander listview
            else if (parent.Name == "dList")
            {
                var operation = (Operation)firstObjectData;
                foreach (var item in treeViewModel.Operation)
                {
                    if (item.Name == parent.Tag.ToString())
                    {
                        item.MoreOperation.Add(new Operation(operation.Name));
                    }
                    if (item.Name == childData.Name.ToString())
                    {
                        item.MoreOperation.Remove(operation);
                    }
                }
            }
        }

        private int GetIndex(object name, IEnumerable<Operation> collection)
        {
            int x = 0;
            foreach (var item in collection)
            {
                if (item == ((Operation)name))
                {
                    break;
                }
                x++;
            }
            return x + 1;
        }

        private static object GetDataFromListView(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                    if (element == source)
                    {
                        return null;
                    }
                }
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }
            return null;
        }

        private void TestList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (secondObjectData != null)
                {
                    DragDrop.DoDragDrop(parent, secondObjectData, DragDropEffects.Move);
                }
            }
        }
    }
}

