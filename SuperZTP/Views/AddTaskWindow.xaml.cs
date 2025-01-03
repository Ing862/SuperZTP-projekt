﻿using SuperZTP.Builder;
using SuperZTP.Command;
using SuperZTP.Model;
using SuperZTP.Composite;
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
using System.Windows.Shapes;

namespace SuperZTP.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddTask.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private List<SuperZTP.Model.Task> tasks;
        private CommandInvoker invoker = new CommandInvoker();
        private TaskBuilder taskBuilder = new TaskBuilder();
        private FileHandler fileHandler;

        public AddTaskWindow(List<SuperZTP.Model.Task> tasks, FileHandler fileHandler, ICategory categories, ITag tags)
        {
            InitializeComponent();
            this.tasks = tasks;
            this.fileHandler = fileHandler;
            LoadCategoriesToComboBox(categories);
            LoadTagsToComboBox(tags);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            int taskId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            var selectedCategoryItem = (ComboBoxItem)CategoryComboBox.SelectedItem;
            var selectedCategory = selectedCategoryItem?.Tag as Category;
            var selectedTagItem = (ComboBoxItem)TagComboBox.SelectedItem;
            var selectedTag = selectedTagItem?.Tag as Tag;
            string priority = ((ComboBoxItem)PriorityComboBox.SelectedItem)?.Content.ToString() ?? "Niski";
            DateTime selectedDate = TaskDatePicker.SelectedDate ?? DateTime.Now; // Domyślnie bieżąca data, jeśli brak wyboru
            bool isCompleted = IsCompletedCheckBox.IsChecked ?? false;

            var zadanie = taskBuilder
                .setTitle(title)
                .setDescription(description)
                .setTag(selectedTag ?? new Tag("Inna"))
                .setCategory(selectedCategory ?? new Category("Inna"))
                .build();
            zadanie.Id = GetNextTaskId(tasks);
            zadanie.SetDeadline(selectedDate);
            zadanie.SetPriority(priority);
            if (isCompleted)
            {
                zadanie.MarkAsDone();
            }
            invoker.AddCommand(new AddTask(tasks, zadanie));
            invoker.Execute();
            fileHandler.SaveTasksToFile("tasks.txt");
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public static int GetNextTaskId(List<Model.Task> tasks)
        {
            return tasks.Any() ?tasks.Max(t => t.Id) + 1 : 1;
        }

        // Odczytywanie kategorii:
        private void LoadCategoriesToComboBox(ICategory rootCategory)
        {
            CategoryComboBox.Items.Clear();
            if (rootCategory is Category cat)
            {
                foreach (var subCategory in cat.categories)
                {
                    AddCategoriesToComboBox(subCategory, "");
                }
            }
        }

        private void AddCategoriesToComboBox(ICategory category, string parentPath)
        {
            string fullPath = string.IsNullOrEmpty(parentPath)
                ? category.Name
                : $"{parentPath} > {category.Name}";

            if (!string.IsNullOrEmpty(parentPath))
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = fullPath,
                    Tag = category
                };
                CategoryComboBox.Items.Add(item);
            }

            if (category is Category cat)
            {
                foreach (var subCategory in cat.categories)
                {
                    AddCategoriesToComboBox(subCategory, fullPath);
                }
            }
        }

        // Odczytywanie tagów:
        private void LoadTagsToComboBox(ITag rootTag)
        {
            TagComboBox.Items.Clear();
            if (rootTag is Tag t)
            {
                foreach (var subTag in t.tags)
                {
                    AddTagsToComboBox(subTag, "");
                }
            }
        }

        private void AddTagsToComboBox(ITag tag, string parentPath)
        {
            string fullPath = string.IsNullOrEmpty(parentPath)
                ? tag.Name
                : $"{parentPath} > {tag.Name}";

            if (!string.IsNullOrEmpty(parentPath))
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = fullPath,
                    Tag = tag
                };
                TagComboBox.Items.Add(item);
            }

            if (tag is Tag t)
            {
                foreach (var subTag in t.tags)
                {
                    AddTagsToComboBox(subTag, fullPath);
                }
            }
        }
    }
}
