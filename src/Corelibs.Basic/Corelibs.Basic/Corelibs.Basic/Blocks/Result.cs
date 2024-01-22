using Corelibs.Basic.Collections;
using Newtonsoft.Json.Linq;

namespace Corelibs.Basic.Blocks
{
    public class Result
    {
        public static Result Create(Result result) => new Result(result);
        public static Result Create(IEnumerable<Result> subResults) => new Result(subResults);
        public static Task<Result> CreateTask(Result result) => Task.FromResult(new Result(result));
        public static Task<Result> CreateTask(IEnumerable<Result> subResults) => Task.FromResult(new Result(subResults));

        public static Result Failure() => new Result(Error.Create());
        public static Result Failure(Exception ex) => new Result(ex);
        public static Result Failure(Error error) => new Result(error);
        public static Result Failure(string errorMessage) => new Result(errorMessage);
        public static Result Failure(object value) => new Result(value, false);
        public static Task<Result> FailureTask() => Task.FromResult(Failure());
        public static Task<Result> FailureTask(Exception ex) => Task.FromResult(Failure(ex));
        public static Task<Result> FailureTask(Error error) => Task.FromResult(Failure(error));
        public static Task<Result> FailureTask(string errorMessage) => Task.FromResult(Failure(errorMessage));
        public static Task<Result> FailureTask(object value) => Task.FromResult(Failure(value));

        public static Result Success() => new Result();
        public static Result Success<TValueType>() where TValueType : new() => new Result(new TValueType());
        public static Result Success(object value) => new Result(value);
        public static Task<Result> SuccessTask() => Task.FromResult(Success());
        public static Task<Result> SuccessTask(object value) => Task.FromResult(Success(value));

        public IEnumerable<Result> SubResults => _subResults;

        public IEnumerable<Remark> LocalRemarks => _remarks;
        public IEnumerable<Remark> AllRemarks => _remarks.Concat(SubRemarks);
        public IEnumerable<Remark> SubRemarks => _subResults.SelectMany(r => r.AllRemarks);

        public IEnumerable<Error> LocalErrors => _errors;
        public IEnumerable<Error> AllErrors => _errors.Concat(SubErrors);
        public IEnumerable<Error> SubErrors => _subResults.SelectMany(r => r.AllErrors);
        public Exception Exception => AllErrors.Select(e => e.Exception).Where(e => e != null).FirstOrDefault();

        public IEnumerable<object> Values => _subResults.SelectMany(r => r.Values).Append(Value).Where(r => r != null);

        public IEnumerable<T> GetValues<T>() => Values.OfType<T>();

        public object Value { get; }

        public T Get<T>()
        {
            try
            {
                var desiredType = typeof(T);

                var valuesArray = Values.ToArray();
                foreach (var value in valuesArray)
                {
                    var valueType = value.GetType();

                    if (desiredType.FullName == valueType.FullName)
                        return (T) value;
                }

                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public object Get(Type type)
        {
            try
            {
                var valuesArray = Values.ToArray();
                var found = valuesArray.Where(v => v.GetType().Equals(type)).FirstOrDefault();
                return found;
            }
            catch (Exception)
            {
                return default;
            }
        }

        private List<Result> _subResults = new List<Result>();
        private List<Error> _errors = new List<Error>();
        private List<Remark> _remarks = new List<Remark>();

        public Result() { }

        public Result(Result result)
        {
            Value = result.Value;
            _subResults = result._subResults;
            _errors = result._errors;
            _remarks = result._remarks;
        }

        public Result(object value)
        {
            if (value == null)
                _errors.Add(Error.Create(new ArgumentNullException()));

            Value = value;
        }

        public Result(object value, bool isSuccess)
        {
            Value = value;
            if (!isSuccess)
                _errors.Add(Error.Create());
        }

        public Result(IEnumerable<Result> subResults)
        {
            _subResults = subResults.ToList();
        }

        public Result(Error error)
        {
            Add(error);
        }

        public Result(string errorMessage)
        {
            Add(Error.Create(errorMessage));
        }

        public Result(Exception exception)
        {
            Add(Error.Create(exception));
        }

        public static Result operator +(Result l, object r)
        {
            var result = new Result();

            result.Add(l);
            result.Add(new Result(r));

            return result;
        }

        public static Result operator+(Result l, Result r)
        {
            var result = new Result();

            result.Add(l);
            result.Add(r);

            return result;
        }

        public static Result operator +(Result l, IEnumerable<Result> r)
        {
            var result = new Result();

            result.Add(l);
            result.Add(r);

            return result;
        }

        public static implicit operator Task<Result>(Result result) => result.ToTask();
        public static implicit operator Result(Task<Result> result) => result.GetAwaiter().GetResult();

        public Result Fail()
        {
            if (!IsSuccess)
                return this;

            Add(Failure());
            return this;
        }

        public Result Fail(string errorMessage)
        {
            Add(Failure(errorMessage));
            return this;
        }

        public void Add(Result result)
        {
            _subResults.Add(result);
        }

        public void Add(IEnumerable<Result> results)
        {
            _subResults.AddRange(results);
        }

        public Result Add(object obj)
        {
            _subResults.Add(new Result(obj));
            return this;
        }

        public void Add(Remark remark) =>
            _remarks.Add(remark);

        public void Add(Error error) =>
            _errors.Add(error);

        public void AddError(string message) =>
            Add(Error.Create(message));

        public void LocalErrorsToRemarks()
        {
            var remarks = _errors.Select(e => new Remark(e.ToString()));
            _remarks.AddRange(remarks);
            _errors.Clear();
        }

        public Result AllErrorsToRemarks()
        {
            LocalErrorsToRemarks();
            _subResults.ForEach(r => r.AllErrorsToRemarks());

            return this;
        }

        public Result WithError(string message)
        {
            AddError(message);
            return this;
        }

        public Result WithErrors(IEnumerable<string> messages)
        {
            messages.ForEach(AddError);
            return this;
        }

        public Result With(params object[] values)
        {
            values.ToList().ForEach(v => Add(Result.Success(v)));
            return this;
        }

        public Result With(params Result[] results)
        {
            Add(results);
            return this;
        }

        public Task<Result> ToTask()
        {
            return Task.FromResult(this);
        }

        private List<Func<Result, Task<Result>>> _actions = new List<Func<Result, Task<Result>>>();
        private Action<Result> _onError;
        public async Task<Result> Run()
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                try
                {
                    _subResults.Add(await action(this));
                }
                catch (Exception ex)
                {
                    _subResults.Add(Failure(ex));
                }
                
                if (!IsSuccess)
                {
                    _onError?.Invoke(this);
                    _actions.Clear();
                    _onError = null;
                    return this;
                }
            }

            _actions.Clear();
            _onError = null;
            return this;
        }

        public Result OnError(Action<Result> action)
        {
            _onError = action;
            return this;
        }

        public Result Of(Func<Result, Task<Result>> action)
        {
            _actions.Add(action);
            return this;
        }

        public bool IsSuccess => !AllErrors.Any();
    }

    public class Result<T> : Result
    {
        public new static Result<T> Create(Result result) => new Result<T>(result);
        public new static Result<T> Create(IEnumerable<Result> subResults) => new Result<T>(subResults);
        public new static Task<Result<T>> CreateTask(Result result) => Task.FromResult(new Result<T>(result));
        public new static Task<Result<T>> CreateTask(IEnumerable<Result> subResults) => Task.FromResult(new Result<T>(subResults));

        public new static Result<T> Failure() => new Result<T>(Error.Create());
        public new static Result<T> Failure(Exception ex) => new Result<T>(ex);
        public new static Result<T> Failure(Error error) => new Result<T>(error);
        public new static Result<T> Failure(string errorMessage) => new Result<T>(errorMessage);
        public new static Task<Result<T>> FailureTask() => Task.FromResult(Failure());
        public new static Task<Result<T>> FailureTask(Exception ex) => Task.FromResult(Failure(ex));
        public new static Task<Result<T>> FailureTask(Error error) => Task.FromResult(Failure(error));
        public new static Task<Result<T>> FailureTask(string errorMessage) => Task.FromResult(Failure(errorMessage));

        public new static Result<T> Success() => new Result<T>();
        public new static Result<T> Success<TValueType>() where TValueType : T, new() => new Result<T>(new TValueType());
        public static Result<T> Success(T value) => new Result<T>(value);
        public new static Result<T> Success(object value) => new Result<T>(value);
        public new static Task<Result<T>> SuccessTask() => Task.FromResult(Success());
        public static Task<Result<T>> SuccessTask(T value) => Task.FromResult(Success(value));
        public new static Task<Result<T>> SuccessTask(object value) => Task.FromResult(Success(value));

        public T Get() => Get<T>();

        public new T Value
        {
            get
            {
                try
                {
                    return (T) base.Value;
                }
                catch (Exception)
                {
                    return default;
                }
            }
                
        }

        public static Result<T> operator +(Result<T> l, Result r)
        {
            var result = new Result<T>();

            result.Add(l);
            result.Add(r);

            return result;
        }

        public Result() { }

        public Result(T value) :
            base(value) {}

        public Result(object value) :
           base(value) {}

        public Result(Error error) :
            base(error) {}

        public Result(string errorMessage) :
            base(errorMessage) { }

        public Result(Exception exception) :
            base(exception) { }

        public Result(Result result) :
            base(result) {}

        public Result(IEnumerable<Result> subResults) :
            base(subResults) { }

        public static implicit operator Result<T>(Task<Result<T>> result) => result.GetAwaiter().GetResult();
        public static implicit operator Result<T>(Task<Result> result) => new Result<T>(result);

        public new Result<T> With(params Result[] results)
        {
            Add(results);
            return this;
        }

        public new Result<T> With(object value)
        {
            Add(new Result(value));
            return this;
        }

        public Result<T> With(IEnumerable<object> values)
        {
            Add(new Result(values));
            return this;
        }

        public new Result<T> Fail()
        {
            Add(Failure());
            return this;
        }

        public new Result<T> Fail(string errorMessage)
        {
            Add(Failure(errorMessage));
            return this;
        }
    }
}
