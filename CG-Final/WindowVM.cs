using System;
using System.Timers;
using System.Windows;
using System.Xml.Serialization;
using CG_Final.Commands;
using CG_Final.Util;
using Microsoft.Win32;

namespace CG_Final
{
    public class WindowVM : ModelBase
    {
        private ObjectBase _selectedObject;

        public ObjectBase SelectedObject
        {
            get { return _selectedObject; }
            set { SetProperty(ref _selectedObject, value, SelectedChanged); }
        }

        #region Commands
        public Command NewObjectCommand { get; }
        public Command DeleteObjectCommand { get; }
        public Command SaveSceneCommand { get; }
        public Command LoadSceneCommand { get; }
        public Command TranslateObjectCommand { get; }
        public Command ScaleObjectCommand { get; }
        public Command RotateObjectCommand { get; }

        private IObjectCommand _lastCommand;

        public IObjectCommand LastCommand
        {
            get { return _lastCommand; }
            set
            {
                SetProperty(ref _lastCommand, value);
            }
        }
        #endregion

        public Scene CurrentScene
        {
            get { return Scene.CurrentScene; }
            set
            {
                Scene.CurrentScene = value;
                OnPropertyChanged(nameof(CurrentScene));
            }
        }

        public WindowVM()
        {
            CurrentScene = new Scene();
            NewObjectCommand = new Command(NewObject);
            LoadSceneCommand = new Command(OpenScene);
            SaveSceneCommand = new Command(Save);
            DeleteObjectCommand = new Command(DeleteObject, () => SelectedObject != null);
            TranslateObjectCommand = new Command(TranslateObject, () => SelectedObject != null);
            ScaleObjectCommand = new Command(ScaleObject, () => SelectedObject != null);
            RotateObjectCommand = new Command(RotateObject, () => SelectedObject != null);

            SelectedObject = CurrentScene.CreateNew();

            CurrentScene.Redraw();

            SelectedChanged += OnSelectedChanged;
        }

        private void RotateObject()
        {
            LastCommand?.Deactivate(Application.Current.MainWindow);
            LastCommand = new RotationCommand(Application.Current.MainWindow, SelectedObject);
            LastCommand.OnApply += () =>
            {
                LastCommand.Deactivate(Application.Current.MainWindow);
                LastCommand = null;
            };
        }

        private void ScaleObject()
        {
            LastCommand?.Deactivate(Application.Current.MainWindow);
            LastCommand = new ScaleCommand(Application.Current.MainWindow, SelectedObject);
            LastCommand.OnApply += () =>
            {
                LastCommand.Deactivate(Application.Current.MainWindow);
                LastCommand = null;
            };
        }

        private void OnSelectedChanged(object sender, ValueChangedEventArgs<ObjectBase> valueChangedEventArgs)
        {
            DeleteObjectCommand.RaiseCanExecuteChanged();
            TranslateObjectCommand.RaiseCanExecuteChanged();
            ScaleObjectCommand.RaiseCanExecuteChanged();
            RotateObjectCommand.RaiseCanExecuteChanged();
        }

        private void TranslateObject()
        {
            LastCommand?.Deactivate(Application.Current.MainWindow);
            LastCommand = new TranslateCommand(Application.Current.MainWindow, SelectedObject);
            LastCommand.OnApply += () =>
            {
                LastCommand.Deactivate(Application.Current.MainWindow);
                LastCommand = null;
            };
        }

        private void DeleteObject()
        {
            LastCommand?.Deactivate(Application.Current.MainWindow);
            LastCommand = null;

            var index = CurrentScene.Objects.IndexOf(SelectedObject);
            CurrentScene.Objects.RemoveAt(index);

            if (CurrentScene.Objects.Count > 0)
            {
                SelectedObject = index < CurrentScene.Objects.Count
                    ? CurrentScene.Objects[index]
                    : CurrentScene.Objects[--index];
            }
            else
                SelectedObject = null;

            CurrentScene.Redraw();
        }

        private void NewObject()
        {
            LastCommand?.Deactivate(Application.Current.MainWindow);
            var o = CurrentScene.CreateNew();
            LastCommand = new CreateNewObject(Application.Current.MainWindow, o);
            SelectedObject = o;
            CurrentScene.Redraw();
        }

        private void Save()
        {
            var saveDiag = new SaveFileDialog()
            {
                Filter = "Benda Supreme file format (*.benda)|*.benda",
                Title = "Super Save Dialog (WoW, Such technology, much persistence)"
            };

            if (saveDiag.ShowDialog(Application.Current.MainWindow) != true) return;

            CurrentScene.Title = saveDiag.SafeFileName.Substring(0, saveDiag.SafeFileName.Length - 6);

            var ser = new XmlSerializer(typeof(Scene));

            using (var stream = saveDiag.OpenFile())
            {
                ser.Serialize(stream, CurrentScene);
            }
        }

        private void OpenScene()
        {
            var fileDiag = new OpenFileDialog()
            {
                Title = "Super open Dialog (WoW, Such technology, much agility)",
                Filter = "Benda Supreme file format (*.benda)|*.benda"
            };

            if (fileDiag.ShowDialog(Application.Current.MainWindow) == false) return;


            var ser = new XmlSerializer(typeof(Scene));

            using (var stream = fileDiag.OpenFile())
            {
                CurrentScene = (Scene)ser.Deserialize(stream);
            }

            CurrentScene.Redraw();
        }

        public event EventHandler<ValueChangedEventArgs<ObjectBase>> SelectedChanged;
    }
}