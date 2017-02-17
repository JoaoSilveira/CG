using System;
using System.Windows;

namespace CG_Final.Commands
{
    public interface IObjectCommand
    {
        void Deactivate(IInputElement e);

        void Apply();

        event Action OnApply;
    }
}