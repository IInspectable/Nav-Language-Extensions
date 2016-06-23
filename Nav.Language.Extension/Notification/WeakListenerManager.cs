#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Notification {

    class WeakListenerManager<T> where T :class {

        readonly List<WeakReference<T>> _listeners;

        public WeakListenerManager() {
            _listeners = new List<WeakReference<T>>();
        }

        public void InvokeListener(Action<T> action) {

            lock (_listeners) {

                var toRemove = new List<WeakReference<T>>();

                foreach (var entry in _listeners) {
                    T listener;
                    if (entry.TryGetTarget(out listener)) {
                        action(listener);
                    } else {
                        toRemove.Add(entry);
                    }
                }

                foreach (var entry in toRemove) {
                    _listeners.Remove(entry);
                }
            }
        }

        public void AddListener(T listener) {
            lock (_listeners) {
                _listeners.Add(new WeakReference<T>(listener));
            }
        }

        public void RemoveListener(T listener) {
            lock (_listeners) {

                _listeners.RemoveAll(entry => {
                    T target;
                    if(entry.TryGetTarget(out target)) {
                        return target == listener;
                    }
                    return false;
                });
            }
        }
    }
}