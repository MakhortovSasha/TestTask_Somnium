using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask_Somnium.Core
{
    class MultiThreadObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _context = SynchronizationContext.Current;

        private void ExecuteOnSyncContext(Action action)
        {
            if (_context.Equals(SynchronizationContext.Current))
                action();
            else
                _context.Send(_ => action(), null);
        }

        protected override void InsertItem(int index, T item) => ExecuteOnSyncContext(() => base.InsertItem(index, item));
        protected override void RemoveItem(int index) => ExecuteOnSyncContext(() => base.RemoveItem(index));
        protected override void SetItem(int index, T item) => ExecuteOnSyncContext(() => base.SetItem(index, item));
        protected override void MoveItem(int oldIndex, int newIndex) => ExecuteOnSyncContext(() => base.MoveItem(oldIndex, newIndex));
        protected override void ClearItems() => ExecuteOnSyncContext(() => base.ClearItems());
    }
}
