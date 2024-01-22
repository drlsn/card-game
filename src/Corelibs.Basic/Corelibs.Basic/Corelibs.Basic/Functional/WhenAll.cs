using System;
using System.Collections.Generic;
using System.Linq;

namespace Corelibs.Basic.Functional
{
    public class WhenAll
    {
        private readonly List<object> _doneResults = new List<object>();
        private Action<object[]> _onAllDoneHandler;

        private bool _onDoneSet;

        public Action<object> NewHandler
        {
            get
            {
                int newIndex = _doneResults.Count;
                _doneResults.Add(null);
                return result =>
                {
                    _doneResults[newIndex] = result;
                    HandleAllDone();
                };
            }
        }

        public Action<T> AddNewHandler<T>(Action<T> handler)
        {
            int newIndex = _doneResults.Count;
            _doneResults.Add(null);
            return result =>
            {
                handler(result);
                _doneResults[newIndex] = result;
                HandleAllDone();
            };
        }

        private void HandleAllDone()
        {
            if (!_onDoneSet)
                return;

            if (!_doneResults.Any(o => o == null))
            {
                _onAllDoneHandler(_doneResults.ToArray());
            }
        }

        public void OnDone<T>(Action<T[]> handler) where T : class
        {
            _onDoneSet = true;
            if (!_doneResults.Any(o => o == null))
            {
                handler(_doneResults.OfType<T>().ToArray());
                return;
            }

            _onAllDoneHandler = args =>
            {
                handler(args.OfType<T>().ToArray());
            };
        }

        public void OnDone(Action handler)
        {
            _onDoneSet = true;
            if (!_doneResults.Any(o => o == null))
            {
                handler();
                return;
            }

            _onAllDoneHandler = args =>
            {
                handler();
            };
        }
    }
}
