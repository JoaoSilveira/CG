using System.Windows;
using CG_Final.Commands;
using CG_Final.Util;

namespace CG_Final
{
    public class WindowVM : ModelBase
    {
        private static Scene _currentScene;

        private ObjectBase _selectedObject;

        public ObjectBase SelectedObject
        {
            get { return _selectedObject; }
            set { SetProperty(ref _selectedObject, value); }
        }

        #region Commands
        public Command NewObjectCommand { get; }
        public Command DeleteObjectCommand { get; }

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
            get { return _currentScene; }
            set { _currentScene = value; }
        }

        public WindowVM()
        {
            CurrentScene = Scene.CurrentScene;
            NewObjectCommand = new Command(NewObject);
            DeleteObjectCommand = new Command(DeleteObject, () => SelectedObject != null);

            SelectedObject = CurrentScene.Objects[0];
        }

        private void DeleteObject()
        {
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
            DeleteObjectCommand.RaiseCanExecuteChanged();
            CurrentScene.Redraw();
        }

        private void NewObject()
        {
            var o = CurrentScene.CreateNew();
            LastCommand = new CreateNewObject(Application.Current.MainWindow, o);
            SelectedObject = o;
            DeleteObjectCommand.RaiseCanExecuteChanged();
            CurrentScene.Redraw();
        }
    }
}