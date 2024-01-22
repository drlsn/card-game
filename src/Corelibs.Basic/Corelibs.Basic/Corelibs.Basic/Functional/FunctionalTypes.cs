using System;

namespace Corelibs.Basic.Functional
{
    public interface IResult<TValue, TError>
    {
        FunctionalResult<TValue, TError> OnSuccess(Action<TValue> action);
        FunctionalResult<TValue, TError> OnError(Action<TError> action);
    }

    public interface IResultDispatcher<TValue, TError>
    {
        void DispatchSuccess(TValue value);
        void DispatchError(TError value);
    }

    public class FunctionalResult<TValue, TError> : 
        IResult<TValue, TError>,
        IResultDispatcher<TValue, TError>
    {
        private Action<TValue> _onSuccess;
        private Action<TError> _onError;

        public FunctionalResult<TValue, TError> OnSuccess(Action<TValue> action)
        {
            _onSuccess = action;
            return this;
        }

        public FunctionalResult<TValue, TError> OnError(Action<TError> action)
        {
            _onError = action;
            return this;
        }

        public void DispatchSuccess(TValue value)
        {
            _onSuccess?.Invoke(value);
        }

        public void DispatchError(TError value)
        {
            _onError?.Invoke(value);
        }
    }
}
