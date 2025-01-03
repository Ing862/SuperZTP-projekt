﻿using SuperZTP.Command;
using SuperZTP.Composite;
using SuperZTP.Controller;
using SuperZTP.Model;
using SuperZTP.TemplateMethod;
using SuperZTP.Views;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperZTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Model.Task> tasks = new List<Model.Task>();
        private List<Note> notes = new List<Note>();
        private FileHandler fileHandler;
        private ICategory categories;
        private ITag tags;
        private GroupTasks GroupTasks = new GroupTasks();
        private GroupNotes GroupNotes = new GroupNotes();
        private SortTasks SortTasks = new SortTasks();
        private SortNotes SortNotes = new SortNotes();
        private GenerateTXT txt;
        private GeneratePDF pdf;
        private GenerateDOCX docx;

        public MainWindow()
        {
            InitializeComponent();
            fileHandler = new FileHandler(tasks, notes);
            fileHandler.LoadTasksFromFile("tasks.txt");
            fileHandler.LoadNotesFromFile("notes.txt");
            categories = fileHandler.LoadCategoriesFromFile("categories.txt");
            tags = fileHandler.LoadTagsFromFile("tags.txt");
            DisplayTasks();
            DisplayNotes();

            txt = new GenerateTXT(tasks);
            pdf = new GeneratePDF(tasks);
            docx = new GenerateDOCX(tasks);
        }

        // TASKI
        // Dodawanie taska
        public void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTask = new AddTaskWindow(tasks, fileHandler, categories, tags);
            addTask.ShowDialog();
            DisplayTasks();
        }

        // Edycja zadania
        public void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var taskToEdit = tasks.FirstOrDefault(t => t.Id == (int)button.Tag);

            if (taskToEdit == null)
                return;

            var editTaskWindow = new EditTaskWindow(taskToEdit, fileHandler, categories, tags);
            if (editTaskWindow.ShowDialog() == true)
            {
                var editedTask = editTaskWindow.EditedTask;
                var taskIndex = tasks.FindIndex(t => t.Id == editedTask.Id);
                if (taskIndex >= 0)
                {
                    tasks[taskIndex] = editedTask;
                }
                DisplayTasks();
            }
        }

        // Usuwanie zadania
        public void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var taskIdToDelete = (int)button.Tag;

            var taskToDelete = tasks.FirstOrDefault(t => t.Id == taskIdToDelete);

            if (taskToDelete != null)
            {
                if (MessageBox.Show($"Czy na pewno chcesz usunąć zadanie: {taskToDelete.Title}?", "Usuń zadanie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tasks.Remove(taskToDelete);
                    fileHandler.SaveTasksToFile("tasks.txt");
                    DisplayTasks();
                }
            }
        }

        // Wyświetlanie listy zadań
        private void DisplayTasks()
        {
            string status;
            TasksListBox.Items.Clear();
            foreach (var task in tasks)
            {
                var taskPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                if(task.IsDone)
                {
                    status = "Ukończone";
                }
                else
                {
                    status = "Nie ukończone";
                }

                var taskText = new TextBlock
                {
                    Text = $"{task.Id}. {task.Title} (Priorytet: {task.Priority}, Termin: {task.Deadline:yyyy-MM-dd})\n{task.Description}\n{status}\n",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 270
                };

                // Przyciski
                var editButton = new Button
                {
                    Content = "Edytuj",
                    Tag = task.Id,
                    Margin = new Thickness(5),
                    Width = 75
                };
                editButton.Click += EditTaskButton_Click;

                var deleteButton = new Button
                {
                    Content = "Usuń",
                    Tag = task.Id,
                    Margin = new Thickness(5),
                    Width = 75
                };
                deleteButton.Click += DeleteTaskButton_Click;
                taskPanel.Children.Add(taskText);
                taskPanel.Children.Add(editButton);
                taskPanel.Children.Add(deleteButton);
                TasksListBox.Items.Add(taskPanel);
            }
        }

        // Grupowanie - Zadań
        private void GroupByCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var groupedTasks = GroupTasks.GroupTasksByCategory(tasks);
            DisplayGroupedTasks(groupedTasks);
        }

        private void GroupByTagButton_Click(object sender, RoutedEventArgs e)
        {
            var groupedTasks = GroupTasks.GroupTasksByTag(tasks);
            DisplayGroupedTasks(groupedTasks);
        }

        private void DisplayGroupedTasks(List<IGrouping<string, Model.Task>> groupedTasks)
        {
            TasksListBox.Items.Clear();

            foreach (var group in groupedTasks)
            {
                var groupHeader = new TextBlock
                {
                    Text = $"Grupa: {group.Key}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 10, 5, 5)
                };
                TasksListBox.Items.Add(groupHeader);

                foreach (var task in group)
                {
                    var taskText = new TextBlock
                    {
                        Text = $"{task.Id}. {task.Title} (Priorytet: {task.Priority}, Termin: {task.Deadline:yyyy-MM-dd}, Wykonane: {task.IsDone})",
                        Margin = new Thickness(10, 0, 5, 5)
                    };
                    TasksListBox.Items.Add(taskText);
                }
            }
        }

        // Sortowanie - Zadań
        private void SortTasksByTitleAsc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByTitle(ascending: true);
        }

        private void SortTasksByTitleDesc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByTitle(ascending: false);
        }

        private void SortTasksByPriorityDesc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByPriority(ascending: false);
        }

        private void SortTasksByPriorityAsc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByPriority(ascending: true);
        }

        private void SortTasksByDeadlineAsc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByDeadline(ascending: true);
        }

        private void SortTasksByDeadlineDesc_Click(object sender, RoutedEventArgs e)
        {
            SortTasksByDeadline(ascending: false);
        }

        private void SortTasksByTitle(bool ascending)
        {
            tasks = SortTasks.SortTasksByTitle(tasks, ascending);
            DisplayTasks();
        }

        private void SortTasksByPriority(bool ascending)
        {
            tasks = SortTasks.SortTasksByPriority(tasks, ascending);
            DisplayTasks();
        }

        private void SortTasksByDeadline(bool ascending)
        {
            tasks = SortTasks.SortTasksByDeadline(tasks, ascending);
            DisplayTasks();
        }

        // NOTATKI
        // Dodawanie notatki
        public void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            AddNoteWindow addNote = new AddNoteWindow(notes, fileHandler, categories, tags);
            addNote.ShowDialog();
            DisplayNotes();
        }

        // Edycja notatki
        public void EditNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var noteToEdit = notes.FirstOrDefault(t => t.Id == (int)button.Tag);

            if (noteToEdit == null)
            {
                return;
            }

            var editNoteWindow = new EditNoteWindow(noteToEdit, fileHandler, categories, tags);
            if (editNoteWindow.ShowDialog() == true)
            {
                var editedNote = editNoteWindow.EditedNote;
                var noteIndex = tasks.FindIndex(t => t.Id == editedNote.Id);
                if (noteIndex >= 0)
                {
                    notes[noteIndex] = editedNote;
                }
                DisplayNotes();
            }
        }

        // Usuwanie notatki
        public void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var noteIdToDelete = (int)button.Tag;

            var noteToDelete = notes.FirstOrDefault(t => t.Id == noteIdToDelete);

            if (noteToDelete != null)
            {
                if (MessageBox.Show($"Czy na pewno chcesz usunąć notatkę: {noteToDelete.Title}?", "Usuń notatkę", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    notes.Remove(noteToDelete);
                    fileHandler.SaveNotesToFile("notes.txt");
                    DisplayNotes();
                }
            }
        }

        // Wyświetlanie listy notatek
        private void DisplayNotes()
        {
            NotesListBox.Items.Clear();
            foreach (var note in notes)
            {
                var notePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                var noteText = new TextBlock
                {
                    Text = $"{note.Id}. {note.Title}\n{note.Description}",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 270
                };

                // Przyciski
                var editButton = new Button
                {
                    Content = "Edytuj",
                    Tag = note.Id,
                    Margin = new Thickness(5),
                    Width = 75
                };
                editButton.Click += EditNoteButton_Click;

                var deleteButton = new Button
                {
                    Content = "Usuń",
                    Tag = note.Id,
                    Margin = new Thickness(5),
                    Width = 75
                };
                deleteButton.Click += DeleteNoteButton_Click;
                notePanel.Children.Add(noteText);
                notePanel.Children.Add(editButton);
                notePanel.Children.Add(deleteButton);
                NotesListBox.Items.Add(notePanel);
            }
        }

        // Grupowanie - Notatek
        private void GroupByCategoryNButton_Click(object sender, RoutedEventArgs e)
        {
            var groupedNotes = GroupNotes.GroupNotesByCategory(notes);
            DisplayGroupedNotes(groupedNotes);
        }

        private void GroupByTagNButton_Click(object sender, RoutedEventArgs e)
        {
            var groupedNotes = GroupNotes.GroupNotesByTag(notes);
            DisplayGroupedNotes(groupedNotes);
        }

        private void DisplayGroupedNotes(List<IGrouping<string, Note>> groupedNotes)
        {
            NotesListBox.Items.Clear();

            foreach (var group in groupedNotes)
            {
                var groupHeader = new TextBlock
                {
                    Text = $"Grupa: {group.Key}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 10, 5, 5)
                };
                NotesListBox.Items.Add(groupHeader);

                foreach (var note in group)
                {
                    var noteText = new TextBlock
                    {
                        Text = $"{note.Id}. {note.Title}\n{note.Description}",
                        Margin = new Thickness(10, 0, 5, 5)
                    };
                    NotesListBox.Items.Add(noteText);
                }
            }
        }

        // Sortowanie - Notatek
        private void SortNotesByTitleAsc_Click(object sender, RoutedEventArgs e)
        {
            SortNotesByTitle(ascending: true);
        }

        private void SortNotesByTitleDesc_Click(object sender, RoutedEventArgs e)
        {
            SortNotesByTitle(ascending: false);
        }

        private void SortNotesByTitle(bool ascending)
        {
            notes = SortNotes.SortNotesByTitle(notes, ascending);
            DisplayNotes();
        }


        // RAPORTY I PODSUMOWANIA (obecnie wszystkie rodzaje pod jednym przyciskiem dla testów)
        private void GenerateRaportButton_Click(object sender, RoutedEventArgs e)
        {
            txt.GenerateRaport("raportTXT.txt");
            pdf.GenerateRaport("raportPDF.pdf");
            docx.GenerateRaport("raportDOCX.docx");
            MessageBox.Show("Wygenerowano raport");
        }

        private void GenerateSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            txt.GenerateSummary("podsumowanieTXT.txt");
            pdf.GenerateSummary("podsumowaniePDF.pdf");
            docx.GenerateSummary("podsumowanieDOCX.docx");
            MessageBox.Show("Wygenerowano podsumowanie");
        }
    }
}